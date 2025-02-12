namespace Tortello
{
    public interface IBoardInputSystem
    {
        /// <summary>
        /// Initialise l'interpréteur d'entrée. <br/>
        /// </summary>
        public void Init();
        /// <summary>
        /// Lit la position du curseur en ScreenSpace. <br/>
        /// Converti toute les cases en ScreenSpace, <br/>
        /// et cherche la case à la même position que le curseur.
        /// <br/> Note : D'abbord vérifier si on est sur la même case qu'avant,
        /// <br/> cela évite de nombreux tests inutiles par frames.
        /// </summary>
        /// <returns>L'id de la case survolée par le curseur.(Id du sommet dans le graphe)</returns>
        public int GetTileHoveredID();
        public bool Place();
        /// <summary>
        /// Appelé à chaque frame. Vérifie si un clic à été fait(Si oui, notifie le Board.), met à jour la position de la caméra si néccéssaire, etc...
        /// </summary>
        public void Update();
        /// <summary>
        /// Désactive la potentielle couche d'action, libère les ressources.
        /// </summary>
        public void Destroy();

        public bool Reset();
    }
}