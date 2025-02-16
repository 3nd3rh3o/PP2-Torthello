using UnityEngine;
using System.Linq;

namespace Torthello {
    public class TorBoardMeshGenerator : IMeshGenerator
    {
        private TorBoardSettings settings;
        private int previousSize;
        private float previousSideLength;
        private CombineInstance[] combines;

        public TorBoardMeshGenerator(TorBoardSettings settings)
        {
            this.settings = settings;
        }

// Initialiser le maillage
        public void InitMesh(MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new Mesh();
            }
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();
            previousSize = settings.BoardSize;
            previousSideLength = settings.SideLength;
            combines = new CombineInstance[settings.BoardSize * settings.BoardSize];
            CreateBoardMesh();
            mesh.CombineMeshes(combines, false, false, false);

            // Appliquer le matériau
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = settings.TileMaterial;
            }
        }

// Mettre à jour le maillage si les paramètres ont changé
        public void UpdateMesh(MeshFilter meshFilter)
        {
            if (previousSize == settings.BoardSize && previousSideLength == settings.SideLength) return;
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();
            previousSize = settings.BoardSize;
            previousSideLength = settings.SideLength;
            combines = new CombineInstance[settings.BoardSize * settings.BoardSize];
            CreateBoardMesh();
            mesh.CombineMeshes(combines, false, false, false)// Détruire le maillage et libérer les ressources
;
        }

// Détruire le maillage et libérer les ressources
        public void Destroy(MeshFilter meshFilter)
        {
    #if UNITY_EDITOR
            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));
            MonoBehaviour.DestroyImmediate(meshFilter.sharedMesh);
    #else
            combines.ToList().ForEach(c => MonoBehaviour.Destroy(c.mesh));
            MonoBehaviour.Destroy(meshFilter.sharedMesh);
    #endif
            combines = null;
        }

// Créer le maillage du plateau
        private void CreateBoardMesh()
        {
            int boardSize = settings.BoardSize;

            // Calculer majorRadius et minorRadius en fonction de boardSize
            float majorRadius = boardSize / (2 * Mathf.PI);
            float minorRadius = majorRadius / 3;

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    float u = (float)i / boardSize * 2 * Mathf.PI;
                    float v = (float)j / boardSize * 2 * Mathf.PI;

                    Vector3 center = new Vector3(
                        (majorRadius + minorRadius * Mathf.Cos(v)) * Mathf.Cos(u),
                        minorRadius * Mathf.Sin(v),
                        (majorRadius + minorRadius * Mathf.Cos(v)) * Mathf.Sin(u)
                    );

                    Mesh mesh = new Mesh();
                    CreateTrapezoidMesh(center, u, v, majorRadius, minorRadius, boardSize, mesh);
                    combines[i * boardSize + j].mesh = mesh;
                    combines[i * boardSize + j].transform = Matrix4x4.identity;

                }
            }
        }

// Créer un maillage de trapèze pour chaque tuile
        private static void CreateTrapezoidMesh(Vector3 center, float u, float v, float majorRadius, float minorRadius, int boardSize, Mesh mesh)
        {
            Vector3[] points = new Vector3[4];

            // Définir les angles de rotation pour chaque sommet du trapèze en fonction de la taille du plateau
            float angleStepU = 2 * Mathf.PI / boardSize;
            float angleStepV = 2 * Mathf.PI / boardSize;

            // Calculer les sommets du trapèze en tenant compte de la courbure du tore
            points[0] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Cos(u - angleStepU / 2),
                minorRadius * Mathf.Sin(v - angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Sin(u - angleStepU / 2)
            );

            points[1] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Cos(u - angleStepU / 2),
                minorRadius * Mathf.Sin(v + angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Sin(u - angleStepU / 2)
            );

            points[2] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Cos(u + angleStepU / 2),
                minorRadius * Mathf.Sin(v + angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Sin(u + angleStepU / 2)
            );

            points[3] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Cos(u + angleStepU / 2),
                minorRadius * Mathf.Sin(v - angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Sin(u + angleStepU / 2)
            );

            int[] f = new int[] { 0, 1, 2, 2, 3, 0 };
            Vector2[] uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            mesh.vertices = points;
            mesh.triangles = f;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
    }
}