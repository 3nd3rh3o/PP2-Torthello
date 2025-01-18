using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(MeshFilter))]
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
    public InputActionReference playerLook;
    public InputActionReference rightClick;

    private Vector2 mousePos;

    public Vector2 mouseSensitivity = new();
    public Vector2 camSensitivity = new();

    public GameObject cursor;

    public Vector2 camAngles;
    private Vector3 camPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        whiteMesh = new();
        blackMesh = new();
        ci = new CombineInstance[2];
        ci[0].mesh = whiteMesh;
        ci[1].mesh = blackMesh;

        whiteMesh = TorusMeshGenerator.GenMeshPair(whiteMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);
        blackMesh = TorusMeshGenerator.GenMeshImpair(blackMesh, radius, sectionRadius, numberOfSection * 2, pointsPerSection * 2);


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
        cursor?.SetActive(false);
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

                Camera.main.transform.position = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0) * camPos;
                Camera.main.transform.rotation = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0);

                cursor.transform.rotation = Camera.main.transform.rotation;
            }

        }

        cursor.transform.position = MouseHelper.GetLerpedPosOnClipPlaneWS(Camera.main, mousePos) * 0.999f;


    }
}
