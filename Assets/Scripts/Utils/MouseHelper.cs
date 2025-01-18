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
}