using UnityEngine;
using UnityEngine.InputSystem;

namespace Torthello
{
    public class TorBoardInputSystem : IBoardInputSystem
    {
        private readonly TorBoardSettings settings;
        private Transform boardTransform;
        private Vector3[][] tileCorners;
        private int previousHoveredTileID;
        private int previousSize;
        private float previousSideLength;
        private InputActionAsset actionMap;
        private float yaw = 120f;
        private float pitch = 0f;
        private TorBoardMeshGenerator meshGenerator;

        // Constructeur initialisant les paramètres de la classe
        public TorBoardInputSystem(TorBoardSettings settings, Transform boardTransform, InputActionAsset actionMap, TorBoardMeshGenerator meshGenerator)
        {
            this.settings = settings;
            this.boardTransform = boardTransform;
            this.actionMap = actionMap;
            this.meshGenerator = meshGenerator;
        }

        // Méthode pour nettoyer les ressources et désactiver les actions
        public void Destroy()
        {
            tileCorners = null;
            actionMap.FindActionMap("InGame", false).Disable();
        }

        // Méthode pour obtenir l'ID de la tuile survolée par la souris
        public int GetTileHoveredID()
        {
            if (!Camera.main || !Application.isFocused) return previousHoveredTileID;

            // Convertir la position de la souris en un rayon dans l'espace 3D
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int closestTileID = -1;
            float closestDistance = float.MaxValue;

            // Parcourir toutes les cases pour trouver celle qui est survolée
            for (int i = 0; i < settings.BoardSize * settings.BoardSize; i++)
            {
                Vector3[] corners = meshGenerator.GetTileCorners(i % settings.BoardSize, i / settings.BoardSize);
                if (RayIntersectsTile(ray, corners, out float distance))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTileID = i;
                    }
                }
            }

            previousHoveredTileID = closestTileID;
            return closestTileID;
        }

        // Méthode pour initialiser le système d'entrée
        public void Init()
        {
            actionMap.FindActionMap("InGame", false).Enable();
            previousSize = settings.BoardSize;
            previousSideLength = settings.SideLength;
            previousHoveredTileID = -1;

            tileCorners = new Vector3[settings.BoardSize * settings.BoardSize][];
            for (int v = 0; v < settings.BoardSize; v++)
            {
                for (int u = 0; u < settings.BoardSize; u++)
                {
                    tileCorners[v * settings.BoardSize + u] = meshGenerator.GetTileCorners(u, v);
                }
            }
        }

        // Méthode pour vérifier si l'action "Place" a été déclenchée
        public bool Place()
        {
            return previousHoveredTileID != -1 && actionMap.FindActionMap("InGame", false).FindAction("Place", false).WasReleasedThisFrame();
        }

        // Méthode pour vérifier si l'action "Reset" a été déclenchée
        public bool Reset()
        {
            return actionMap.FindActionMap("InGame", false).FindAction("Reset", false).WasReleasedThisFrame();
        }

        // Méthode pour mettre à jour l'état du système d'entrée
        public void Update()
        {
            if (actionMap.FindActionMap("InGame", false).FindAction("View").ReadValue<float>() == 1f)
            {
                yaw += Input.mousePositionDelta.y * 100f * Time.deltaTime;
                pitch += Input.mousePositionDelta.x * 130f * Time.deltaTime;
                pitch %= 360f;
                yaw = Mathf.Clamp(yaw, 100f, 165f);
            }
            Camera.main.transform.position = Quaternion.Euler(0f, pitch, 0f) * Quaternion.Euler(0f, 0f, -yaw) * (boardTransform.position - new Vector3(10f, 0f, 0f));
            Camera.main.transform.LookAt(boardTransform.position);

            if (previousSize == settings.BoardSize && previousSideLength == settings.SideLength) return;
            Destroy();
            Init();
            return;
        }

        // Méthode pour vérifier si un rayon intersecte une tuile
        private bool RayIntersectsTile(Ray ray, Vector3[] corners, out float distance)
        {
            distance = float.MaxValue;
            Plane plane = new Plane(corners[0], corners[1], corners[2]);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                if (PointInPolygon(hitPoint, corners))
                {
                    distance = enter;
                    return true;
                }
            }
            return false;
        }

        // Méthode pour vérifier si un point est à l'intérieur d'un polygone
        private bool PointInPolygon(Vector3 point, Vector3[] polygon)
        {
            int i, j;
            bool result = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].z > point.z) != (polygon[j].z > point.z) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
                {
                    result = !result;
                }
            }
            return result;
        }
    }
}
