using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
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
    private Mesh highLightMesh;
    public InputActionReference playerLook;
    public InputActionReference rightClick;
    public InputActionReference zoomAction;

    private Vector2 mousePos;

    public Vector2 mouseSensitivity = new();
    public Vector2 camSensitivity = new();

    public GameObject cursor;

    public Vector2 camAngles;
    private Vector3 camPos;

    public int2 tileHovered = new();

    public Material[] materials;


    private Material[] boardMats;
    private Material[] boardMatsWithH;

    private float zoom = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {

        boardMats = new Material[] { materials[0], materials[1] };
        boardMatsWithH = materials;



        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        whiteMesh = new();
        blackMesh = new();
        highLightMesh = new();
        ci = new CombineInstance[3];
        ci[0].mesh = whiteMesh;
        ci[1].mesh = blackMesh;
        ci[2].mesh = highLightMesh;


        whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);
        blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);

        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            GetComponent<MeshFilter>().sharedMesh = new();
        }

        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(ci, false, true, false);

        camPos = new(0, 0, -10);

        camAngles = new(90, 0);

        Camera.main.transform.position = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0) * camPos;
        Camera.main.transform.rotation = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0);

        cursor.transform.rotation = Camera.main.transform.rotation;


        cursor.SetActive(true);




    }


    void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (whiteMesh == null) whiteMesh = new();
        if (blackMesh == null) blackMesh = new();
        if (highLightMesh == null) highLightMesh = new();
        if (ci == null)
        {
            ci = new CombineInstance[3];
            ci[0].mesh = whiteMesh;
            ci[1].mesh = blackMesh;
            ci[2].mesh = highLightMesh;
        }

        if (zoomAction != null)
        {
            float dz = zoomAction.action.ReadValue<Vector2>().y;
            if ((zoom < 20f && dz > 0) || zoom > 6f && dz < 0)
            {
                zoom += dz * 0.5f;
            }
        }

        if (playerLook != null)
        {
            //Mouse mouvement
            Vector2 mouseDelta = playerLook.action.ReadValue<Vector2>();

            mousePos.x = Mathf.Max(-1, Mathf.Min(mousePos.x + mouseDelta.x * mouseSensitivity.x, 1));
            mousePos.y = Mathf.Max(-1, Mathf.Min(mousePos.y + mouseDelta.y * mouseSensitivity.y, 1));



            if (rightClick != null && rightClick.action.ReadValue<float>() == 1f)
            {
                //Camera mouvement
                if (camAngles.x < 180 && mouseDelta.y < 0 || camAngles.x > 0 && mouseDelta.y > 0) camAngles.x += mouseDelta.y * camSensitivity.y;
                camAngles.y += mouseDelta.x * camSensitivity.x;
                camAngles.y = camAngles.y % 360;
            }
            else
            {
                if (!mouseDelta.Equals(Vector2.zero))
                {
                    tileHovered = MouseHelper.GetTileHovered((Camera.main.transform.position - MouseHelper.GetLerpedPosOnClipPlaneWS(Camera.main, mousePos)), Camera.main.transform.position, transform, numberOfSection * 2, pointsPerSection * 2, radius, sectionRadius);

                    if (tileHovered.Equals(new(-1, -1)))
                    {
                        highLightMesh.Clear();
                        GetComponent<MeshRenderer>().sharedMaterials = boardMats;
                    }
                    else
                    {
                        highLightMesh = TorusMeshGenerator.GenMeshOfTileByIndex(highLightMesh, tileHovered.x, tileHovered.y, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);
                        GetComponent<MeshRenderer>().sharedMaterials = boardMatsWithH;
                    }


                    whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);
                    blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);

                    GetComponent<MeshFilter>().sharedMesh.Clear();
                    GetComponent<MeshFilter>().sharedMesh.CombineMeshes(ci, false, false);
                }
            }

        }
        camPos = camPos.normalized * zoom;
        Camera.main.transform.position = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0) * camPos;
        Camera.main.transform.rotation = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0);

        cursor.transform.rotation = Camera.main.transform.rotation;
        cursor.transform.position = Camera.main.transform.position - (Camera.main.transform.position - MouseHelper.GetLerpedPosOnClipPlaneWS(Camera.main, mousePos)) * 1.1f;


    }
}
