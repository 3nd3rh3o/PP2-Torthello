using UnityEngine;

namespace Tortello
{
    public class FlatBoardMeshGenerator : MeshGenerator
    {
        [Range(4, 20)] public int BoardWidth = 8;
        [Range(4, 20)] public int BoardHeight = 8;
        
        public void InitMesh(MeshFilter meshFilter)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateMesh(MeshFilter meshFilter)
        {
            throw new System.NotImplementedException();
        }

        public void Destroy(MeshFilter mF)
        {
            throw new System.NotImplementedException();
        }
    }
}