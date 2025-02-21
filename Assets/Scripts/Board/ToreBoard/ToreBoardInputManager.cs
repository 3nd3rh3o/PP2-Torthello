using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Tortello{
    public class ToreBoardInputManager : FlatBoardInputSystem
    {
        public ToreBoardInputManager(FlatBoardSettings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {
        }

        public override void Init()
        {
            actionMap.FindActionMap("InGame", false).Enable();
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            previousSideLength = settings.sideLength;
            previousHoveredTileID = -1;
            // float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            // float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            // Vector3 offset = new(offsetX, 0f, offsetZ);
            tileCorners = new Vector3[settings.BoardHeight * settings.BoardWidth][];
            /*
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
            */
            
            for (int v = 0; v < settings.BoardHeight; v++)
            {
                for (int u = 0; u < settings.BoardWidth; u++)
                {
                    float subradius = 1.5f*settings.BoardWidth/(2f*Mathf.PI);
                    float radius = (1.5f*settings.BoardHeight/(2f*Mathf.PI))+subradius;

                    tileCorners[v * settings.BoardWidth + u] = GetPointsOfTile(u,v,radius,subradius,settings.BoardHeight,settings.BoardWidth);
                }
            }

                        
        }

        private Vector3[] GetPointsOfTile(int i, int j, float radius, float sectionRadius, int numberOfSection, int pointsPerSection){
            Vector3[] points = new Vector3[4];

            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * (sectionRadius + 0.0001f);

            Vector3 section = Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * sectionCenter;

            Vector3 p0 = section + Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * Quaternion.Euler(new((360f / pointsPerSection) * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * Quaternion.Euler(new((360f / pointsPerSection) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new((360f / pointsPerSection) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new((360f / pointsPerSection) * j, 0, 0)) * subSectionVector;

            int p0i = 0;
            int p1i = 1;
            int p2i = 2;
            int p3i = 3;

            points[p0i] = p0;
            points[p1i] = p1;
            points[p2i] = p2;
            points[p3i] = p3;

            return points;
        }

        public override int GetTileHoveredID()
        {
             if (!Camera.main || !Application.isFocused) return previousHoveredTileID;
            Vector2 mousePos = new(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            // is prev tile still hovered?
            if (previousHoveredTileID != -1 && IsTileHovered(previousHoveredTileID, mousePos)) return previousHoveredTileID;
            float d = -1f;
            for (int i = 0; i < settings.BoardHeight * settings.BoardWidth; i++)
            {
                if (IsTileHovered(i, mousePos) && (d == -1f || d > (Camera.main.transform.position - tileCorners[i][0]).sqrMagnitude))
                {
                    previousHoveredTileID = i;
                    return i;
                }
            }
            previousHoveredTileID = -1;
            return -1;
        }
    }
}