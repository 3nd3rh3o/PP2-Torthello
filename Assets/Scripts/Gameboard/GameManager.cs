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

        whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection*2, pointsPerSection*2);
        blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection*2, pointsPerSection*2);


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
        whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection*2, pointsPerSection*2);
        blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection*2, pointsPerSection*2);

        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(ci, false, false);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.rotation * Vector3.Scale(transform.localScale, TorusSpaceHelper.IndexToPos(testPointI, testPointJ, numberOfSection*2, pointsPerSection*2, radius, sectionRadius)), 0.1f);
    }
}
