using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    
    public float radius = 0.5f;
    public float sectionRadius = 0.25f;
    [Range(2, 100)]
    public int numberOfSection = 4;
    [Range(2, 100)]
    public int pointsPerSection = 4;

    public int testPointI = 0;
    public int testPointJ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<MeshFilter>().sharedMesh = TorusMeshGenerator.GenMesh(GetComponent<MeshFilter>().sharedMesh, radius, sectionRadius, numberOfSection, pointsPerSection);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshFilter>().sharedMesh = TorusMeshGenerator.GenMesh(GetComponent<MeshFilter>().sharedMesh, radius, sectionRadius, numberOfSection, pointsPerSection);
        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(TorusSpaceHelper.IndexToPos(testPointI, testPointJ, numberOfSection, pointsPerSection, radius, sectionRadius), 0.1f);
    }
}
