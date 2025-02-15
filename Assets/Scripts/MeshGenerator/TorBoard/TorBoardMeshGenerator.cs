using UnityEngine;
using System.Linq;

namespace Tortello {
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
        }

        public void UpdateMesh(MeshFilter meshFilter)
        {
            if (previousSize == settings.BoardSize && previousSideLength == settings.SideLength) return;
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();
            previousSize = settings.BoardSize;
            previousSideLength = settings.SideLength;
            combines = new CombineInstance[settings.BoardSize * settings.BoardSize];
            CreateBoardMesh();
            mesh.CombineMeshes(combines, false, false, false);
        }

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

        private void CreateBoardMesh()
        {
            float offsetX = (-settings.SideLength * settings.BoardSize + settings.SideLength) * 0.5f;
            float offsetZ = (-settings.SideLength * settings.BoardSize + settings.SideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);
            for (int i = 0; i < settings.BoardSize; i++)
            {
                for (int j = 0; j < settings.BoardSize; j++)
                {
                    Vector3 c = offset + new Vector3(j * settings.SideLength, 0f, i * settings.SideLength);
                    Mesh mesh = new();
                    CreateSquareMesh(c, settings.SideLength, mesh);
                    combines[i * settings.BoardSize + j].mesh = mesh;
                }
            }
        }

        private static void CreateSquareMesh(Vector3 center, float sideLength, Mesh mesh)
        {
            Vector3[] points = new Vector3[]
            {
                center + new Vector3(0.5f * sideLength, 0f, 0.5f * sideLength),
                center + new Vector3(0.5f * sideLength, 0f, -0.5f * sideLength),
                center + new Vector3(-0.5f * sideLength, 0f, -0.5f * sideLength),
                center + new Vector3(-0.5f * sideLength, 0f, 0.5f * sideLength)
            };
            int[] f = new int[] { 0, 1, 2, 2, 3, 0 };
            Vector2[] uv = new Vector2[] { new(1, 1), new(1, 0), new(0, 0), new(0, 1) };
            mesh.vertices = points;
            mesh.triangles = f;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
    }
}