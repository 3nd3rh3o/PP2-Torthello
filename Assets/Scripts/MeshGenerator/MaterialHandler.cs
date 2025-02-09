using UnityEngine;

namespace Tortello
{
    public interface IMaterialHandler
    {
        /// <summary>
        /// Appellé pour initialiser le renderer. Effectue la création de ce qui est requis si null.
        /// </summary>
        /// <param name="renderer"></param>
        public void InitMeshRenderer(MeshRenderer renderer);
        
        /// <summary>
        /// Appellé pour mettre à jour les données du renderer, si elles on changé.
        /// </summary>
        /// <param name="renderer"></param>
        public void UpdateMeshRenderer(MeshRenderer renderer);

        /// <summary>
        /// Appellé pour nettoyer et detruire toutes ressources utilisées par le renderer.
        /// </summary>
        /// <param name="renderer"></param>
        public void Destroy(MeshRenderer renderer);
    }
}