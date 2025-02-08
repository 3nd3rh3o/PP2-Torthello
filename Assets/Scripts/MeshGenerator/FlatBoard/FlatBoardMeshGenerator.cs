using UnityEngine;
using System.Linq;

namespace Tortello
{
    public class FlatBoardMeshGenerator : MeshGenerator
    {
        [Range(4, 20)] public int BoardWidth = 8;
        [Range(4, 20)] public int BoardHeight = 8;
        [Range(0f,10f)] public float sideLength = 1f;
        public Vector3 center;
        private CombineInstance[] combines;

        public void InitMesh(MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh=new();
            }
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();

        }

        public void UpdateMesh(MeshFilter meshFilter)
        {
            throw new System.NotImplementedException();
        }

        public void Destroy(MeshFilter mF)
        {
            throw new System.NotImplementedException();
        }




        public static void CreateSquareMesh(Vector3 center, float sideLength, Mesh mesh){
            Vector3[] points = new Vector3[4];
            points[0] = center + new Vector3(0.5f*sideLength,0f,0.5f*sideLength);
            points[1] = center + new Vector3(0.5f*sideLength,0f,-0.5f*sideLength);
            points[2] = center + new Vector3(-0.5f*sideLength,0f,-0.5f*sideLength);
            points[3] = center + new Vector3(-0.5f*sideLength,0f,0.5f*sideLength);

            int[] f = new int[2*3];
            //First triangle
            f[0]=0;
            f[1]=1;
            f[2]=2;

            //Second triangle
            f[3]=2;
            f[4]=3;
            f[5]=0;

            mesh.vertices = points;
            mesh.triangles = f;
            mesh.RecalculateNormals();
        }
    }
}