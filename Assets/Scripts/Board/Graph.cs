using System.Collections.Generic;
using Unity.VisualScripting;

namespace Torthello
{
    /// <summary>
    /// Le graphe qui contient les pions et les cases du plateau.<br/>
    /// Permet au jeu de stocker l'état du plateau, et de gérer les placements de pions.
    /// </summary>
    public interface IGraph
    {
        /// <summary>
        /// Construit le graphe selon les paramètres du plateau.<br/>
        /// Doit créer les pions initiaux.
        /// </summary>
        public void InitGraph();
        /// <summary>
        /// Met à jour la structure du graphe si les paramêtres sont modifiés. <br/>
        /// Attention, supprime tout les pions.
        /// </summary>
        public void UpdateGraph();
        /// <summary>
        /// Ajoute un pion sur le graphe.<br/>
        /// </summary>
        /// <param name="idSommet"> L'identifiant du sommet où placer le pion. </param>
        /// <param name="couleur">La couleur du pion à placer.</param>
        /// <param name="pionsRetournes">La liste des pions retournés par le coup.</param>
        /// <returns>True si le coup est valide, false sinon.</returns>
        public bool AddPawn(int idSommet, Couleur couleur, List<List<int>> pionsRetournes);
        /// <summary>
        /// Retire un pion du graphe.<br/>
        /// Ne vérifie pas si le coup est valide.
        /// </summary>
        /// <param name="idSommets">L'identifiant du sommet où retirer le pion.</param>
        /// <param name="pawnsToFlip">La liste des pions retournés par le coup.</param>
        public void RemovePawn(int idSommets, List<List<int>> pawnsToFlip);

        /// <summary>
        /// Place un pion sur le graphe.<br/>
        /// Ne vérifie pas si le coup est valide.
        /// </summary>
        /// <param name="idSommet">L'identifiant du sommet où placer le pion.</param>
        /// <param name="couleur">La couleur du pion à placer.</param>
        public void SetPawn(int idSommet, Couleur couleur);

        /// <summary>
        /// Démarre la partie
        /// </summary>
        public void StartGame();

        /// <summary>
        /// Score de la partie.
        /// </summary>
        public List<int> GetScore();

        /// <summary>
        /// Vérifie si un joueur peut placer un pion.
        /// </summary>
        /// <param name="couleur"></param>
        /// <returns></returns>

        public bool NoPlacementAvailable(Couleur couleur);

        /// <summary>
        /// Supprime tout les pions du graphe.
        /// </summary>
        public void RemoveAllPawns();
        /// <summary>
        /// Supprime le graphe, et libère les ressources.
        /// </summary>
        public void DestroyGraph();
        /// <summary>
        /// Vérifie si un coup est valide.
        /// </summary>
        /// <param name="idSommet">L'identifiant du sommet où placer le pion.</param>
        /// <param name="couleur">La couleur du pion à placer.</param>
        /// <param name="pionsARetournes">La liste des pions retournés par le coup.</param>
        public bool IsValidMove(int idSommet, Couleur couleur, List<List<int>> pionsARetournes);
        /// <summary>
        /// Retourne les coups valides pour un joueur.
        /// Attention, renvoie un copie des coups valides.
        /// </summary>
        /// <param name="couleur"></param>
        /// <returns></returns>
        public List<int> GetValidMoves(Couleur couleur);
        /// <summary>
        /// Retourne la taille du plateau.
        /// </summary>
        public int GetBoardSize();
        /// <summary>
        /// Retourne si la partie est terminée.
        /// </summary>
        bool IsGameOver();

        public void SetValidMoves(List<int> b, List<int> n);


        /// <summary>
        /// Retourne la liste d'adjacence.
        /// </summary> 
        public List<int> GetVideAdj();

        public Graph GetGraph();
    }
    public class Graph
    {
        public Sommets[] sommets;
        
    }
    public class Sommets
    {
        public Couleur couleur;
        public Arretes[] arretes;
    }
    public class Arretes
    {
        public int d; //Id du sommet de départ
        public int a; //Id du sommet d'arrivée
    }
    public enum Couleur
    {
        Vide,
        Blanc,
        Noir
    }
}