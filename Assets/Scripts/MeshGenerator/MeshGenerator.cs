using UnityEngine;
namespace Tortello
{
    public interface MeshGenerator
    {
        public void UpdateMesh(MeshFilter meshFilter);
        public void Destroy(MeshFilter mF);
    }

}
