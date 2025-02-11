
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tortello
{
    public class FlatBoardInputSystem : IBoardInputSystem
    {
        private FlatBoardSettings settings;
        private Transform boardTransform;
        private Vector3[][] tileCorners;
        private int previousHoveredTileID;

        public FlatBoardInputSystem(FlatBoardSettings settings, Transform boardTransform)
        {
            this.settings = settings;
            this.boardTransform = boardTransform;
        }

        public void Destroy()
        {


        }


        //TODO
        public int GetTileHoveredID()
        {
            if (!Camera.main || !Application.isFocused) return -1;
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
            return -1;
        }

        public void Init()
        {
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

        public void Update()
        {
            
        }

        private bool IsTileHovered(int id, Vector2 mousePosition)
        {
            Vector3[] corners = tileCorners[id];

            Vector2 A = new
            (
                Camera.main.WorldToViewportPoint(corners[0], Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(corners[0], Camera.MonoOrStereoscopicEye.Mono).y
            );
            Vector2 B = new
            (
                Camera.main.WorldToViewportPoint(corners[1], Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(corners[1], Camera.MonoOrStereoscopicEye.Mono).y
            );
            Vector2 C = new
            (
                Camera.main.WorldToViewportPoint(corners[2], Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(corners[2], Camera.MonoOrStereoscopicEye.Mono).y
            );
            Vector2 D = new
            (
                Camera.main.WorldToViewportPoint(corners[3], Camera.MonoOrStereoscopicEye.Mono).x,
                Camera.main.WorldToViewportPoint(corners[3], Camera.MonoOrStereoscopicEye.Mono).y
            );
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