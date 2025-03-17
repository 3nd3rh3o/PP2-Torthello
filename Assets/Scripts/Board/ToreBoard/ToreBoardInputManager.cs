using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Torthello
{
    public class ToreBoardInputManager : FlatBoardInputSystem
    {
        public ToreBoardInputManager(Settings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {
        }

        public override void Init()
        {
            actionMap.FindActionMap("InGame", false).Enable();
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            previousSideLength = settings.sideLength;


            Camera.main.transform.position = Quaternion.Euler(0f, settings.yaw, 0f) * Quaternion.Euler(0f, 0f, -settings.pitch) * (boardTransform.position - new Vector3(settings.zoom, 0f, 0f));
            Camera.main.transform.LookAt(boardTransform.position);
        }

        

        public override int GetTileHoveredID()
        {
            if (!Camera.main || !Application.isFocused) return previousHoveredTileID;
            Vector2 mousePos = Input.mousePosition;
            mousePos.x = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0, Camera.main.pixelWidth, Input.mousePosition.x));
            mousePos.y = Mathf.Lerp(1f, -1f, Mathf.InverseLerp(0, Camera.main.pixelHeight, Input.mousePosition.y));
            float subradius = 1.5f * settings.BoardWidth / (2f * Mathf.PI);
            float radius = (1.5f * settings.BoardHeight / (2f * Mathf.PI)) + subradius;
            
            previousHoveredTileID = GetTileHovered((Camera.main.transform.position - GetLerpedPosOnClipPlaneWS(Camera.main, mousePos)), Camera.main.transform.position, boardTransform, settings.BoardHeight, settings.BoardWidth, radius, subradius);
            
            return previousHoveredTileID;
        }

        //Raycasts from the camera to the board to find the tile that is being hovered by the mouse
        public int GetTileHovered(Vector3 rayDir, Vector3 rayOrigin, Transform gameBoardTransform, int numCol, int numLine, float radius, float sectionRadius)
        {
            float dist = 1000f;
            int cand = -1;
            for (int i = 0; i < numCol; i++)
            {
                for (int j = 0; j < numLine; j++)
                {
                    Vector3[] tileCorners = IndexToTileCorners(i, j, numCol, numLine, radius, sectionRadius,settings.rotationOffset);
                    Vector3 A = tileCorners[0];
                    Vector3 B = tileCorners[1];
                    Vector3 C = tileCorners[2];
                    Vector3 D = tileCorners[3];
                    // two triangle chk 
                    // ABD and CDB
                    Vector3 n = Vector3.Cross((B - A), (D - A));
                    Vector3 nB = Vector3.Cross((D - C), (B - C));

                    //if tile parralel to ray, no intersection.
                    if (Vector3.Dot(n, rayDir) == 0 || Vector3.Dot(nB, rayDir) == 0) continue;

                    float det0 = Vector3.Dot(n, A);
                    float det1 = Vector3.Dot(nB, C);

                    float t0 = (det0 - Vector3.Dot(n, rayOrigin)) / Vector3.Dot(n, rayDir);
                    float t1 = (det1 - Vector3.Dot(nB, rayOrigin)) / Vector3.Dot(nB, rayDir);

                    Vector3 Q0 = rayOrigin + rayDir * t0;
                    Vector3 Q1 = rayOrigin + rayDir * t1;

                    if (// triangle 1
                        (
                            Vector3.Dot(Vector3.Cross(B - A, Q0 - A), n) >= 0
                            && Vector3.Dot(Vector3.Cross(D - B, Q0 - B), n) >= 0
                            && Vector3.Dot(Vector3.Cross(A - D, Q0 - D), n) >= 0
                        )
                    || // triangle 2
                        (
                            Vector3.Dot(Vector3.Cross(D - C, Q1 - C), nB) >= 0
                            && Vector3.Dot(Vector3.Cross(B - D, Q1 - D), nB) >= 0
                            && Vector3.Dot(Vector3.Cross(C - B, Q1 - B), nB) >= 0
                        )
                    )
                    {
                        float cD = (IndexToPos(i, j, numCol, numLine, radius, sectionRadius, settings.rotationOffset) - rayOrigin).sqrMagnitude;
                        if (cD < dist)
                        {
                            cand = i * settings.BoardWidth + j;
                            dist = cD;
                        }
                    }
                }
            }

            return cand;
        }

        public bool rotate()
        {
            return actionMap.FindActionMap("InGame", false).FindAction("Rotate").ReadValue<float>() == 1f;
        }
        public override void Update()
        {
            // Camera controls
            if (settings.isInGame && actionMap.FindActionMap("InGame", false).FindAction("View").ReadValue<float>() == 1f)
            {
                Cursor.lockState = CursorLockMode.Confined;
                settings.pitch += Input.mousePositionDelta.y * 100f * Time.deltaTime * settings.CamSentivity;
                settings.yaw += Input.mousePositionDelta.x * 130f * Time.deltaTime * settings.CamSentivity;
                settings.yaw %= 360f;
                settings.pitch = Mathf.Clamp(settings.pitch, 100f, 220f); 
            } else {
                Cursor.lockState = CursorLockMode.None;
            }
            if (settings.isInGame)
            {
                Vector2 zoom = actionMap.FindActionMap("InGame", false).FindAction("Zoom").ReadValue<Vector2>();
                settings.zoom = Mathf.Clamp(settings.zoom+zoom.y, 10f, 30f);
            }
            Camera.main.transform.position = Quaternion.Euler(0f, settings.yaw, 0f) * Quaternion.Euler(0f, 0f, -settings.pitch) * (boardTransform.position - new Vector3(settings.zoom, 0f, 0f));
            Camera.main.transform.LookAt(boardTransform.position);

            // need to rebuild board map?
            if (previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth && previousSideLength == settings.sideLength) return;
            Destroy();
            Init();
            return;
        }

        // TODO Add offset handling here
        //recalculates the position of a tile based on its index: this is already calculated in the mesh generator, maybe we can reuse it instead?
        public static Vector3[] IndexToTileCorners(int i, int j, int maxI, int maxJ, float radius, float sectionRadius, float rotationOffset)
        {
            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;
            Vector3 section = Quaternion.Euler(new(0, 360f / maxI * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * sectionCenter;

            // Appliquer l'offset pour faire rouler les cases sur le petit cercle
            Quaternion minorCircleRotation = Quaternion.Euler(new(rotationOffset, 0, 0));

            Vector3 p0 = section + Quaternion.Euler(new(0, 360f / maxI * i, 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, 360f / maxI * i, 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * j, 0, 0)) * subSectionVector;

            return new Vector3[] { p0, p1, p2, p3 };
        }

        public static Vector3[] GetNearClipPlaneWS(Camera camera)
        {
            Vector3 t = camera.transform.up * (Mathf.Tan((camera.fieldOfView / 2f) * Mathf.Deg2Rad) * camera.transform.forward * camera.nearClipPlane).magnitude;
            Vector3 l = -camera.transform.right * ((t.magnitude * 2f) * camera.aspect) / 2f;
            Vector3 p = camera.transform.position + (camera.transform.forward * (camera.nearClipPlane));
            return new Vector3[] { p + l + t, p - l + t, p - l - t, p + l - t };
        }

        public static Vector3 GetLerpedPosOnClipPlaneWS(Camera camera, Vector2 cursorPos)
        {
            Vector3[] WSCorners = GetNearClipPlaneWS(camera);
            Vector3 a = WSCorners[0];
            Vector3 b = WSCorners[1];
            Vector3 d = WSCorners[3];
            Vector3 x = a + (((b - a) * (cursorPos.x + 1)) / 2f);
            Vector3 y = (((d - a) * (cursorPos.y + 1)) / 2f);
            return (x + y);
        }


        
        //recalculates the position of (the center?) a tile based on its index: this is already calculated in the mesh generator, maybe we can reuse it instead?
        public static Vector3 IndexToPos(int i, int j, int maxI, int maxJ, float radius, float sectionRadius, float rotationOffset)
        {
            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;

            // Calcul de la position de la section sur le grand cercle
            Vector3 section = Quaternion.Euler(new(0, (360f / maxI) * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, (360f / maxI) * (i + 1), 0)) * sectionCenter;

            // Appliquer l'offset pour faire rouler les cases sur le petit cercle
            Quaternion minorCircleRotation = Quaternion.Euler(new(rotationOffset, 0, 0));

            Vector3 p0 = section + Quaternion.Euler(new(0, (360f / maxI) * i, 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, (360f / maxI) * i, 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, (360f / maxI) * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, (360f / maxI) * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * j, 0, 0)) * subSectionVector;

            return (p0 + p1 + p2 + p3) * 0.25f;
        }

        
    }
}