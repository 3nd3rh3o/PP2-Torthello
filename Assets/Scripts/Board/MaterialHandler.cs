using UnityEngine;

namespace Torthello
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
        /// Utilisé pour indiqué l'id de la case sur laquelle est la sourie.
        /// </summary>
        /// <param name="id">L'identifiant de la case survolée</param>
        public void SetHoveredTile(int id);

        /// <summary>
        /// Appelée si le placement d'un pion à échoué.
        /// </summary>
        public void FailedPlacement();

        /// <summary>
        /// Appellé pour nettoyer et detruire toutes ressources utilisées par le renderer.
        /// </summary>
        /// <param name="renderer"></param>
        public void Destroy(MeshRenderer renderer);
    }
}