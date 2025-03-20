using UnityEngine;
using UnityEngine.InputSystem;
namespace Torthello
{
    public class TriangularBoardInputManager : FlatBoardInputSystem
    {
        public TriangularBoardInputManager(Settings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {
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
         public int GetTileHovered(Vector3 rayDir, Vector3 rayOrigin, Transform gameBoardTransform, int numCol, int numLine, float radius, float sectionRadius)
        {
            float dist = 1000f;
            int cand = -1;
            for (int i = 0; i < numCol; i++)
            {
                for (int j = 0; j < numLine; j++)
                {
                    Vector3[] tileCorners = IndexToTileCorners(i, j, numCol, numLine,settings.sideLength);
                    Vector3 c = tileCorners[0];
                    for(int t = 0;t < 6;t++){

                        Vector3 a = tileCorners[t];
                        Vector3 b = tileCorners[(t + 1)];
                        Vector3 n = Vector3.Cross(a - c, b - c);
                        if(Vector3.Dot(n,rayDir)==0)continue;
                        float d = Vector3.Dot(n, c);
                        float T = (d-Vector3.Dot(n,rayOrigin))/Vector3.Dot(n,rayDir);
                        Vector3 Q = rayOrigin + T * rayDir;

                        if(
                            Vector3.Dot(Vector3.Cross(a - c, Q - c), n) >= 0 &&
                            Vector3.Dot(Vector3.Cross(b - a, Q - a), n) >= 0 &&
                            Vector3.Dot(Vector3.Cross(c - b, Q - b), n) >= 0
                        ){
                            float cD = (IndexToPos(i,j,numCol,numLine,settings.sideLength) - rayOrigin).sqrMagnitude; 
                            if(cD < dist){
                                dist = cD;
                                cand = i * settings.BoardWidth + j;
                            }
                        }
                    }
                }
            }

            return cand;
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

        public static Vector3[] IndexToTileCorners(int i, int j, int BoardHeight, int BoardWidth,float sideLength)
        {
            Vector3 U = new(2f * Mathf.Sqrt(Mathf.Pow(sideLength * 0.5f, 2) - Mathf.Pow(sideLength * 0.25f, 2)), 0f, 0f);
            Vector3 V = Quaternion.Euler(0f, 120f, 0f) * U;
            Vector3 oU =BoardWidth * -0.5f * U;
            Vector3 oV =BoardHeight * -0.5f * V;

            Vector3 c = oU + oV + (j * U) + (i * V);

            Vector3 v = new Vector3(0f, 0f, sideLength * 0.5f);
            Vector3[] points = new Vector3[]{
                c,
                c + (Quaternion.Euler(0f, 0f, 0f) * v),
                c + (Quaternion.Euler(0f, 60f, 0f) * v),
                c + (Quaternion.Euler(0f, 120f, 0f) * v),
                c + (Quaternion.Euler(0f, 180f, 0f) * v),
                c + (Quaternion.Euler(0f, 240f, 0f) * v),
                c + (Quaternion.Euler(0f, 300f, 0f) * v),
            };

            return points;
        }
        public static Vector3 IndexToPos(int i, int j, int BoardHeight, int BoardWidth,float sideLength)   
        {
            Vector3 U = new(2f * Mathf.Sqrt(Mathf.Pow(sideLength * 0.5f, 2) - Mathf.Pow(sideLength * 0.25f, 2)), 0f, 0f);
            Vector3 V = Quaternion.Euler(0f, 120f, 0f) * U;
            Vector3 oU =BoardWidth * -0.5f * U;
            Vector3 oV =BoardHeight * -0.5f * V;

            return oU + oV + (j * U) + (i * V);
        }
    }
}