using UnityEngine;
using System.Linq;

namespace Torthello
{
    public class FlatBoardMeshGenerator : IMeshGenerator
    {

        protected FlatBoardSettings settings;
        protected int PreviousWidth;

        protected int PreviousHeight;

        protected float PreviousLength;

        protected CombineInstance[] combines;

        public FlatBoardMeshGenerator(Settings settings)
        {
            this.settings = settings;
        }

        public virtual void InitMesh(MeshFilter meshFilter)
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

        public virtual void UpdateMesh(MeshFilter meshFilter)
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

        public void Destroy(MeshFilter mF)
        {
#if UNITY_EDITOR
            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));
            MonoBehaviour.DestroyImmediate(mF.sharedMesh);
#else
            combines.ToList().ForEach(c => MonoBehaviour.Destroy(c.mesh));
            MonoBehaviour.Destroy(mF.sharedMesh);
#endif
            combines = null;

        }

        private static void CreateSquareMesh(Vector3 center, float sideLength, Mesh mesh)
        {
            Vector3[] points = new Vector3[]{
                center + new Vector3(0.5f*sideLength,0f,0.5f*sideLength),
                center + new Vector3(0.5f*sideLength,0f,-0.5f*sideLength),
                center + new Vector3(-0.5f*sideLength,0f,-0.5f*sideLength),
                center + new Vector3(-0.5f*sideLength,0f,0.5f*sideLength)
            };

            int[] f = new int[]{0, 1, 2, 2, 3, 0};
            

            Vector2[] uv = new Vector2[]{new (1, 1), new(1, 0), new(0, 0), new(0, 1)};

            mesh.vertices = points;
            mesh.triangles = f;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }

        private void CreateBoardMesh()
        {
            //Calcul du centre de la premiere case
            float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);

            //generation des mesh en fonction de l'offset
            for (int i = 0; i < settings.BoardHeight; i++)
            {
                for (int j = 0; j < settings.BoardWidth; j++)
                {
                    Vector3 c = offset + new Vector3(j * settings.sideLength, 0f, i * settings.sideLength);
                    Mesh mesh = new();
                    CreateSquareMesh(c, settings.sideLength, mesh);
                    combines[i * settings.BoardWidth + j].mesh = mesh;
                }
            }

        }

    }
}