using UnityEngine;

namespace Tortello
{
    public class FlatBoardMeshGenerator : MeshGenerator
    {
        [Range(4, 20)] public int boardWidth = 8;
        [Range(4, 20)] public int boardHeight = 8;

        public void Destroy(MeshFilter mF)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateMesh(MeshFilter meshFilter)
        {
            throw new System.NotImplementedException();
        }
    }
}