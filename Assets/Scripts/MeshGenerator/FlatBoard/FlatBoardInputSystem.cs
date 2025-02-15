using UnityEngine;
using UnityEngine.InputSystem;

namespace Torthello
{
    public class FlatBoardInputSystem : IBoardInputSystem
    {
        private readonly FlatBoardSettings settings;
        private Transform boardTransform;
        private Vector3[][] tileCorners;
        private int previousHoveredTileID;
        private int previousWidth;
        private int previousHeight;
        private float previousSideLength;
        private InputActionAsset actionMap;
        private float yaw = 120f;
        private float pitch = 0f;

        public FlatBoardInputSystem(FlatBoardSettings settings, Transform boardTransform, InputActionAsset actionMap)
        {
            this.settings = settings;
            this.boardTransform = boardTransform;
            this.actionMap = actionMap;
        }

        public void Destroy()
        {
            tileCorners = null;
            actionMap.FindActionMap("InGame", false).Disable();
        }


        //TODO
        public int GetTileHoveredID()
        {
            if (!Camera.main || !Application.isFocused) return previousHoveredTileID;
            Vector2 mousePos = new(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            // is prev tile still hovered?
            if (previousHoveredTileID != -1 && IsTileHovered(previousHoveredTileID, mousePos)) return previousHoveredTileID;
            for (int i = 0; i < settings.BoardHeight * settings.BoardWidth; i++)
            {
                if (IsTileHovered(i, mousePos))
                {
                    previousHoveredTileID = i;
                    return i;
                }
            }
            previousHoveredTileID = -1;
            return -1;
        }

        public void Init()
        {
            actionMap.FindActionMap("InGame", false).Enable();
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            previousSideLength = settings.sideLength;
            previousHoveredTileID = -1;
            float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);
            tileCorners = new Vector3[settings.BoardHeight * settings.BoardWidth][];
            for (int v = 0; v < settings.BoardHeight; v++)
            {
                for (int u = 0; u < settings.BoardWidth; u++)
                {
                    Vector3 c = offset + new Vector3(u * settings.sideLength, 0f, v * settings.sideLength);
                    tileCorners[v * settings.BoardWidth + u] = new Vector3[]
                    {
                        c + 0.5f * new Vector3(-settings.sideLength,0f,-settings.sideLength),
                        c + 0.5f * new Vector3(settings.sideLength,0f,-settings.sideLength),
                        c + 0.5f * new Vector3(-settings.sideLength,0f,settings.sideLength),
                        c + 0.5f * new Vector3(settings.sideLength,0f,settings.sideLength)
                    };
                }
            }
        }

        public bool Place()
        {
            return previousHoveredTileID != -1 && actionMap.FindActionMap("InGame", false).FindAction("Place", false).WasReleasedThisFrame();
        }

        public bool Reset()
        {
            return actionMap.FindActionMap("InGame", false).FindAction("Reset", false).WasReleasedThisFrame();
        }

        public void Update()
        {
            // Camera controls
            if (actionMap.FindActionMap("InGame", false).FindAction("View").ReadValue<float>() == 1f)
            {
                yaw += Input.mousePositionDelta.y * 100f * Time.deltaTime;
                pitch += Input.mousePositionDelta.x * 130f * Time.deltaTime;
                pitch %= 360f;
                yaw = Mathf.Clamp(yaw, 100f, 165f);
            }
            Camera.main.transform.position = Quaternion.Euler(0f, pitch, 0f) * Quaternion.Euler(0f, 0f, -yaw) * (boardTransform.position - new Vector3(10f, 0f, 0f));
            Camera.main.transform.LookAt(boardTransform.position);

            // need to rebuild board map?
            if (previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth && previousSideLength == settings.sideLength) return;
            Destroy();
            Init();
            return;
        }

        private bool IsTileHovered(int id, Vector2 mousePosition)
        {
            Vector3[] corners = tileCorners[id];

            Vector2 A = new
            (
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[0]), Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[0]), Camera.MonoOrStereoscopicEye.Mono).y
            );
            Vector2 B = new
            (
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[1]), Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[1]), Camera.MonoOrStereoscopicEye.Mono).y
            );
            Vector2 C = new
            (
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[2]), Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[2]), Camera.MonoOrStereoscopicEye.Mono).y
            );
            Vector2 D = new
            (
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[3]), Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(boardTransform.position+boardTransform.rotation * Vector3.Scale(boardTransform.lossyScale,corners[3]), Camera.MonoOrStereoscopicEye.Mono).y
            );
            // On verifie si le vecteur de la sourie est entre les vecteurs formant les coins de la case 
            // projetée sur l'écran. (coordonnées de 0 à 1 sur un plan.)
            bool BAC = Vector2.Dot((B - A).normalized, (mousePosition - A).normalized) >= 0 && 
                Vector2.Dot((C - A).normalized, (mousePosition - A).normalized) >= 0;
            bool ABD = Vector2.Dot((A - B).normalized, (mousePosition - B).normalized) >= 0 &&
                Vector2.Dot((D - B).normalized, (mousePosition - B).normalized) >= 0;
            bool DCA = Vector2.Dot((D - C).normalized, (mousePosition - C).normalized) >= 0 &&
                Vector2.Dot((A - C).normalized, (mousePosition - C).normalized) >= 0;
            bool BDC = Vector2.Dot((B - D).normalized, (mousePosition - D).normalized) >= 0 &&
                Vector2.Dot((C - D).normalized, (mousePosition - B).normalized) >= 0;
            return BAC && ABD && DCA && BDC;
        }
    }
}