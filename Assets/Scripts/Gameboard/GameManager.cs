using NUnit.Framework.Constraints;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{

    public float radius = 0.5f;
    public float sectionRadius = 0.25f;
    [Range(1, 50)]
    public int numberOfSection = 4;
    [Range(1, 50)]
    public int pointsPerSection = 4;

    public int testPointI = 0;
    public int testPointJ = 0;

    private CombineInstance[] ci;
    private Mesh whiteMesh;
    private Mesh blackMesh;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        whiteMesh = new();
        blackMesh = new();
        ci = new CombineInstance[2];
        ci[0].mesh = whiteMesh;
        ci[1].mesh = blackMesh;

        whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);
        blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);


        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(ci, false, true, false);

    }

    // Update is called once per frame
    void Update()
    {
        if (whiteMesh == null) whiteMesh = new();
        if (blackMesh == null) blackMesh = new();
        if (ci == null)
        {
            ci = new CombineInstance[2];
            ci[0].mesh = whiteMesh;
            ci[1].mesh = blackMesh;
        }
        whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);
        blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);

        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(ci, false, false);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(TorusSpaceHelper.OSToWS(transform, TorusSpaceHelper.IndexToPos(testPointI, testPointJ, numberOfSection * 2, pointsPerSection * 2, radius, sectionRadius)), 0.1f);
        for (int i = 0; i < numberOfSection * 2; i++)
        {
            for (int j = 0; j < pointsPerSection * 2; j++)
            {
                Vector3 centerOfTileOS = TorusSpaceHelper.IndexToPos(i, j, numberOfSection * 2, pointsPerSection * 2, radius, sectionRadius);
                Vector3 normalOfTileOS = TorusSpaceHelper.IndexToTileNormal(i, j, numberOfSection * 2, pointsPerSection * 2, radius, sectionRadius);
                
                Gizmos.DrawLine(TorusSpaceHelper.OSToWS(transform, centerOfTileOS), TorusSpaceHelper.OSToWS(transform, centerOfTileOS + (normalOfTileOS * 0.2f)));
            }
        }

    }
}
