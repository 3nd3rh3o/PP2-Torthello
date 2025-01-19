using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GameManager : MonoBehaviour
{

    public Board.Settings bSettings = new();
    public Board.Manager bManager;


    public InputActionReference playerLook;
    public InputActionReference rightClick;
    public InputActionReference zoomAction;

    private Vector2 mousePos;

    public Vector2 mouseSensitivity = new();
    public Vector2 camSensitivity = new();


    private Vector2 camAngles;
    private Vector3 camPos;

    [HideInInspector]
    public int2 tileHovered = new(-1, -1);

    public Texture2D cursorTex;

    private float zoom = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        
        //Setup boardManager
        bManager = new(bSettings, this);
        bManager.Setup();
        bManager.DrawBase();
        bSettings.enableHoverEffect = true;

        

        camPos = new(0, 0, -10);

        camAngles = new(125, 0);

        Camera.main.transform.position = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0) * camPos;
        Camera.main.transform.rotation = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0);
    }


    void OnDisable()
    {
        bManager.Discard();
        bManager=null;
    }

    // Update is called once per frame
    void Update()
    {
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

            mousePos.x = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0, Camera.main.pixelWidth, Input.mousePosition.x));
            mousePos.y = Mathf.Lerp(1f, -1f, Mathf.InverseLerp(0, Camera.main.pixelHeight, Input.mousePosition.y));


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
                    tileHovered = MouseHelper.GetTileHovered((Camera.main.transform.position - MouseHelper.GetLerpedPosOnClipPlaneWS(Camera.main, mousePos)), Camera.main.transform.position, transform, bSettings.numberOfSection * 2, bSettings.pointsPerSection * 2, bSettings.radius, bSettings.sectionRadius);
                }
            }

        }
        
        bManager.DrawEffect();


        camPos = camPos.normalized * zoom;
        Camera.main.transform.position = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0) * camPos;
        Camera.main.transform.rotation = Quaternion.Euler(0, camAngles.y, 0) * Quaternion.Euler(camAngles.x - 90f, 0, 0);

    }
}
