using UnityEngine;
using UnityEngine.TextCore;

public class TorusMeshGenerator
{
    public static Mesh GenMesh(Mesh mesh, float radius, float sectionRadius, int numberOfSection , int pointsPerSection)
    {
       
        Vector3[] points = new Vector3[4 * numberOfSection * pointsPerSection];
        int[] triangles = new int[6 * numberOfSection * pointsPerSection];

        Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
        Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;
        // Gen of sections
        for (int i = 0; i < numberOfSection; i++)
        {
            Vector3 section = Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * sectionCenter;
            
            // Gen of one section(circle);
            for (int j = 0; j < pointsPerSection; j++)
            {
                Vector3 p0 = section + Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*j, 0, 0)) * subSectionVector;
                Vector3 p1 = section + Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*(j+1), 0, 0)) * subSectionVector;
                Vector3 p2 = nextSection + Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*(j+1), 0, 0)) * subSectionVector;
                Vector3 p3 = nextSection + Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*j, 0, 0)) * subSectionVector;

                int p0i = (i * pointsPerSection + j) * 4;
                int p1i = (i * pointsPerSection + j) * 4 + 1;
                int p2i = (i * pointsPerSection + j) * 4 + 2;
                int p3i = (i * pointsPerSection + j) * 4 + 3;

                points[p0i] = p0;
                points[p1i] = p1;
                points[p2i] = p2;
                points[p3i] = p3;
                
                triangles[(i * pointsPerSection + j) * 6] = p0i;
                triangles[(i * pointsPerSection + j) * 6 + 1] = p1i;
                triangles[(i * pointsPerSection + j) * 6 + 2] = p3i;
                triangles[(i * pointsPerSection + j) * 6 + 3] = p1i;
                triangles[(i * pointsPerSection + j) * 6 + 4] = p2i;
                triangles[(i * pointsPerSection + j) * 6 + 5] = p3i;
            }
        }
        // assigning computed points for each face to the mesh
        mesh.Clear();
        mesh.vertices = points;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        
        return mesh;
    }

    public static Mesh GenMeshPair(Mesh mesh, float radius, float sectionRadius, int numberOfSection , int pointsPerSection){
        
        Vector3[] points = new Vector3[2 * numberOfSection * pointsPerSection];
        int[] triangles = new int[3 * numberOfSection * pointsPerSection];

        Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
        Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;

        // Gen of sections
        for (int i = 0; i < numberOfSection; i++)
        {
            Vector3 section = Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * sectionCenter;
            
            // Gen of one section(circle);
            for (int j = 0; j < pointsPerSection; j++)
            {
                if (i%2 != j%2 ) continue;

                Vector3 p0 = section + Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*j, 0, 0)) * subSectionVector;
                Vector3 p1 = section + Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*(j+1), 0, 0)) * subSectionVector;
                Vector3 p2 = nextSection + Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*(j+1), 0, 0)) * subSectionVector;
                Vector3 p3 = nextSection + Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*j, 0, 0)) * subSectionVector;

                int p0i = ((i * pointsPerSection + j)/2) * 4;
                int p1i = ((i * pointsPerSection + j)/2) * 4 + 1;
                int p2i = ((i * pointsPerSection + j)/2) * 4 + 2;
                int p3i = ((i * pointsPerSection + j)/2) * 4 + 3;

                points[p0i] = p0;
                points[p1i] = p1;
                points[p2i] = p2;
                points[p3i] = p3;
                
                triangles[((i * pointsPerSection + j)/2) * 6] = p0i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 1] = p1i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 2] = p3i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 3] = p1i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 4] = p2i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 5] = p3i;
            }
        }
        // assigning computed points for each face to the mesh
        mesh.Clear();
        mesh.vertices = points;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        
        return mesh;

    }

    public static Mesh GenMeshImpair(Mesh mesh, float radius, float sectionRadius, int numberOfSection , int pointsPerSection){
       Vector3[] points = new Vector3[2 * numberOfSection * pointsPerSection];
        int[] triangles = new int[3 * numberOfSection * pointsPerSection];

        Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
        Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;

        // Gen of sections
        for (int i = 0; i < numberOfSection; i++)
        {
            Vector3 section = Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * sectionCenter;
            
            // Gen of one section(circle);
            for (int j = 0; j < pointsPerSection; j++)
            {
                if (i%2 == j%2 ) continue;

                Vector3 p0 = section + Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*j, 0, 0)) * subSectionVector;
                Vector3 p1 = section + Quaternion.Euler(new (0, (360f/numberOfSection)*i, 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*(j+1), 0, 0)) * subSectionVector;
                Vector3 p2 = nextSection + Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*(j+1), 0, 0)) * subSectionVector;
                Vector3 p3 = nextSection + Quaternion.Euler(new (0, (360f/numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new ((360f/pointsPerSection)*j, 0, 0)) * subSectionVector;

                int p0i = ((i * pointsPerSection + j)/2) * 4;
                int p1i = ((i * pointsPerSection + j)/2) * 4 + 1;
                int p2i = ((i * pointsPerSection + j)/2) * 4 + 2;
                int p3i = ((i * pointsPerSection + j)/2) * 4 + 3;

                points[p0i] = p0;
                points[p1i] = p1;
                points[p2i] = p2;
                points[p3i] = p3;
                
                triangles[((i * pointsPerSection + j)/2) * 6] = p0i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 1] = p1i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 2] = p3i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 3] = p1i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 4] = p2i;
                triangles[((i * pointsPerSection + j)/2) * 6 + 5] = p3i;
            }
        }
        // assigning computed points for each face to the mesh
        mesh.Clear();
        mesh.vertices = points;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        
        return mesh;
    }
}
