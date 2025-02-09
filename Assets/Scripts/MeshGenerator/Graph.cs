namespace Tortello
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
        /// Ajoute un pion sur le graphe.
        /// </summary>
        public void AddPawn();
        /// <summary>
        /// Supprime tout les pions du graphe.
        /// </summary>
        public void RemoveAllPawns(int idsommet, Couleur couleur);
        /// <summary>
        /// Supprime le graphe, et libère les ressources.
        /// </summary>
        public void DestroyGraph();
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
        public int d;
        public int a;
    }
    public enum Couleur
    {
        Vide,
        Blanc,
        Noir
    }
}

