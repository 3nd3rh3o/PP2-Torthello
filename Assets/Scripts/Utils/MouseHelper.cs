using Unity.Mathematics;
using UnityEngine;



public class MouseHelper
{
    public static Vector3[] GetNearClipPlaneWS(Camera camera)
    {
        Vector3 t = camera.transform.up * (Mathf.Tan((camera.fieldOfView/2f) * Mathf.Deg2Rad)  * camera.transform.forward * camera.nearClipPlane).magnitude;
        Vector3 l = -camera.transform.right * ((t.magnitude*2f) * camera.aspect)/2f;
        Vector3 p = camera.transform.position + (camera.transform.forward * (camera.nearClipPlane));
        return new Vector3[]{p+l+t, p-l+t, p-l-t, p+l-t};
    }

    public static Vector3 GetLerpedPosOnClipPlaneWS(Camera camera, Vector2 cursorPos)
    {
        Vector3[] WSCorners = GetNearClipPlaneWS(camera);
        Vector3 a = WSCorners[0];
        Vector3 b = WSCorners[1];
        Vector3 d = WSCorners[3];
        Vector3 x = a + (((b-a)*(cursorPos.x+1))/2f);
        Vector3 y = (((d-a)*(cursorPos.y+1))/2f);
        return (x+y);
    }

    public static Vector2 ConvMousePos(Vector2 initialCursorPos)
    {
        return initialCursorPos;
    }

    public static int2 GetTileHovered(Vector3 rayDir, Vector3 rayOrigin, Transform gameBoardTransform, int numCol, int numLine, float radius, float sectionRadius)
    {
        float dist = 1000f;
        int2 cand = new(-1, -1);
        for (int i = 0; i < numCol; i++)
        {
            for (int j = 0; j < numLine; j++)
            {
                Vector3[] tileCorners = TorusSpaceHelper.IndexToTileCorners(i, j, numCol, numLine, radius, sectionRadius);
                Vector3 A = tileCorners[0];
                Vector3 B = tileCorners[1];
                Vector3 C = tileCorners[2];
                Vector3 D = tileCorners[3];
                // two triangle chk 
                // ABD and CDB
                Vector3 n = Vector3.Cross((B-A), (D-A));
                Vector3 nB = Vector3.Cross((D-C), (B-C));

                //if tile parralel to ray, no intersection.
                if (Vector3.Dot(n, rayDir) == 0 || Vector3.Dot(nB, rayDir) == 0) continue;
                
                float det0 = Vector3.Dot(n, A);
                float det1 = Vector3.Dot(nB, C);
                
                float t0 = (det0 - Vector3.Dot(n, rayOrigin))/Vector3.Dot(n, rayDir);
                float t1 = (det1 - Vector3.Dot(nB, rayOrigin))/Vector3.Dot(nB, rayDir);

                Vector3 Q0 = rayOrigin + rayDir*t0;
                Vector3 Q1 = rayOrigin + rayDir*t1;

                if (// triangle 1
                    (
                        Vector3.Dot(Vector3.Cross(B-A, Q0-A), n) >= 0 
                        && Vector3.Dot(Vector3.Cross(D-B, Q0-B), n) >= 0 
                        && Vector3.Dot(Vector3.Cross(A-D, Q0-D), n) >= 0
                    )
                || // triangle 2
                    (
                        Vector3.Dot(Vector3.Cross(D-C, Q1-C), nB) >= 0 
                        && Vector3.Dot(Vector3.Cross(B-D, Q1-D), nB) >= 0 
                        && Vector3.Dot(Vector3.Cross(C-B, Q1-B), nB) >= 0
                    )
                )
                {
                    float cD = (TorusSpaceHelper.IndexToPos(i, j, numCol, numLine, radius, sectionRadius) - rayOrigin).sqrMagnitude;
                    if (cD < dist) {
                        cand = new(i, j);
                        dist = cD;
                    }
                }
            }
        }
        
        return cand;
    }
}