using System.Collections.Generic;

namespace Tortello
{
    public interface IPawnProccessor
    {
        /// <summary>
        /// Ajoute un pion sur le plateau de jeu.
        /// </summary>
        /// <param name="TileID">Emplacement du pion.</param>
        /// <param name="couleur">Couleur du pion.</param>
        public void SpawnPawn(int TileID, Couleur couleur);
        /// <summary>
        /// Notifie les pions pour qu'ils lancent leur animation.
        /// </summary>
        /// <param name="pawnFlipped"></param>
        public void FlipAnimSeq(List<List<int>> pawnFlipped);
        /// <summary>
        /// Detruit tout les pions.
        /// </summary>
        public void RemoveAllPawns();

        /// <summary>
        /// Démarre la partie
        /// </summary>
        public void StartGame();
        /// <summary>
        /// Initialise le gestionnaire de pions.<br/>
        /// </summary>
        public void Init();
        /// <summary>
        /// Syncronise la position des pions.
        /// </summary>
        public void Update();

        /// <summary>
        /// Supprime tout les pions. Et libère les ressources.
        /// </summary>
        public void Destroy();
    }

    public enum PawnModel
    {
        Default
    }
}