using UnityEngine;
namespace Torthello
{
    public interface IMeshGenerator
    {
        /// <summary>
        /// Créé le mesh, et initialise toute ressources utiles pour lui.
        /// </summary>
        /// <param name="meshFilter"></param>
        
        public void InitMesh(MeshFilter meshFilter);

        /// <summary>
        /// Mets à jour les ressources liées au mesh, et régénère le mesh.
        /// </summary>
        /// <param name="meshFilter"></param>
        public void UpdateMesh(MeshFilter meshFilter);

        /// <summary>
        /// Détruit les ressouces utilisées, ainsi que le mesh.
        /// </summary>
        /// <param name="mF"></param>
        public void Destroy(MeshFilter mF);
    }

}
