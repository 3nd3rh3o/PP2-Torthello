using UnityEngine;
using System.Linq;

namespace Torthello
{
    public class TriangularBoardMeshGenerator : FlatBoardMeshGenerator
    {
        public TriangularBoardMeshGenerator(Settings settings) : base(settings)
        {
        }

        public override void InitMesh(MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new();
            }
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();

            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;
            PreviousLength = settings.sideLength;

            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];

            CreateBoardMesh();

            mesh.CombineMeshes(combines, false, false, false);
        }

        public override void UpdateMesh(MeshFilter meshFilter)
        {
            //on teste si les parametres in changes
            if (PreviousHeight == settings.BoardHeight && PreviousWidth == settings.BoardWidth && PreviousLength == settings.sideLength) return;

            Mesh mesh = meshFilter.sharedMesh;

            mesh.Clear();

            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;
            PreviousLength = settings.sideLength;

#if UNITY_EDITOR
            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));
#else
            combines.ToList().ForEach(c => MonoBehaviour.Destroy(c.mesh));
#endif

            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];

            CreateBoardMesh();

            mesh.CombineMeshes(combines, false, false, false);
        }

        private void CreateBoardMesh()
        {
            //Calcul du centre de la premiere case
            Vector3 U = new(2f * Mathf.Sqrt(Mathf.Pow(settings.sideLength * 0.5f, 2) - Mathf.Pow(settings.sideLength * 0.25f, 2)), 0f, 0f);
            Vector3 V = Quaternion.Euler(0f, 120f, 0f) * U;
            Vector3 oU = settings.BoardWidth * -0.5f * U;
            Vector3 oV = settings.BoardHeight * -0.5f * V;
            //generation des mesh en fonction de l'offset
            for (int i = 0; i < settings.BoardHeight; i++)
            {
                for (int j = 0; j < settings.BoardWidth; j++)
                {
                    Vector3 c = oU + oV + (j * U) + (i * V);
                    Mesh mesh = new();
                    CreateHexagonMesh(c, settings.sideLength, mesh);
                    combines[i * settings.BoardWidth + j].mesh = mesh;
                }
            }

        }

        private static void CreateHexagonMesh(Vector3 center, float sideLength, Mesh mesh)
        {
            Vector3 v = new Vector3(0f, 0f, sideLength * 0.5f);
            Vector3[] points = new Vector3[]{
                center,
                center + (Quaternion.Euler(0f, 0f, 0f) * v),
                center + (Quaternion.Euler(0f, 60f, 0f) * v),
                center + (Quaternion.Euler(0f, 120f, 0f) * v),
                center + (Quaternion.Euler(0f, 180f, 0f) * v),
                center + (Quaternion.Euler(0f, 240f, 0f) * v),
                center + (Quaternion.Euler(0f, 300f, 0f) * v),
            };


            int[] f = new int[]{
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5,
                0, 5, 6,
                0, 6, 1
            };

            float tex = 0.45f;
            Vector2 texD = new(tex, 0f);

            Vector2[] uv = new Vector2[]{
                new(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f) + rotate(texD, 0f),
                new Vector2(0.5f, 0.5f) + rotate(texD, Mathf.PI / 3f),
                new Vector2(0.5f, 0.5f) + rotate(texD, 2f * Mathf.PI / 3f),
                new Vector2(0.5f, 0.5f) + rotate(texD, Mathf.PI),
                new Vector2(0.5f, 0.5f) + rotate(texD, 4f * Mathf.PI / 3f),
                new Vector2(0.5f, 0.5f) + rotate(texD, 5f * Mathf.PI / 3f)
            };

            mesh.vertices = points;
            mesh.triangles = f;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }

        //RADIANS !!!
        private static Vector2 rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }
    }
}
