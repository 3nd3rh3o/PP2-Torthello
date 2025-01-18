using System;
using UnityEngine;

public class TorusSpaceHelper
{
    public static Vector3 IndexToPos(int i, int j, int maxI, int maxJ, float radius, float sectionRadius)
    {
        Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
        Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;
        Vector3 section = Quaternion.Euler(new (0, 360f/maxI*i, 0)) * sectionCenter;
        Vector3 nextSection = Quaternion.Euler(new (0, 360f/maxI * (i + 1), 0)) * sectionCenter;
        Vector3 p0 = section + Quaternion.Euler(new (0, 360f/maxI*i, 0)) * Quaternion.Euler(new (360f/maxJ*j, 0, 0)) * subSectionVector;
        Vector3 p1 = section + Quaternion.Euler(new (0, (360f/maxI)*i, 0)) * Quaternion.Euler(new ((360f/maxJ)*(j+1), 0, 0)) * subSectionVector;
        Vector3 p2 = nextSection + Quaternion.Euler(new (0, 360f/maxI * (i + 1), 0)) * Quaternion.Euler(new (360f/maxJ*(j+1), 0, 0)) * subSectionVector;
        Vector3 p3 = nextSection + Quaternion.Euler(new (0, (360f/maxI) * (i + 1), 0)) * Quaternion.Euler(new ((360f/maxJ)*j, 0, 0)) * subSectionVector;
        return (p0+p1+p2+p3)*0.25f;
    }
    public static Vector3 IndexToTileNormal(int i, int j, int maxI, int maxJ, float radius, float sectionRadius)
    {
        Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
        Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;
        Vector3 p0 = Quaternion.Euler(new (0, 360f/maxI*i, 0)) * Quaternion.Euler(new (360f/maxJ*j, 0, 0)) * subSectionVector;
        Vector3 p1 = Quaternion.Euler(new (0, (360f/maxI)*i, 0)) * Quaternion.Euler(new ((360f/maxJ)*(j+1), 0, 0)) * subSectionVector;
        Vector3 p2 = Quaternion.Euler(new (0, 360f/maxI * (i + 1), 0)) * Quaternion.Euler(new (360f/maxJ*(j+1), 0, 0)) * subSectionVector;
        Vector3 p3 = Quaternion.Euler(new (0, (360f/maxI) * (i + 1), 0)) * Quaternion.Euler(new ((360f/maxJ)*j, 0, 0)) * subSectionVector;
        return ((p0+p1+p2+p3)*0.25f).normalized;
    }
    public static Tuple<int, int> GetLookedTile(Vector3 lookDirectionWS, ref Transform transform)
    {
        

        



        return new(0, 0);
    }

    public static Vector3 OSToWS(Transform transform, Vector3 vOS)
    {
        return transform.position + transform.rotation * Vector3.Scale(transform.localScale, vOS);
    }
}