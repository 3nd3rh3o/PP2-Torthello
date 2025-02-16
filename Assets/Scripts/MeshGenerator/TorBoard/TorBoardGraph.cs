using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    public class TorBoardGraph : IGraph
    {
        private Graph graph;
        private TorBoardSettings settings;

        private int previousSize;
        public TorBoardGraph(TorBoardSettings settings)
        {
            this.settings = settings;
        }

        //initialisation du Graph
        public void InitGraph()
        {
            graph = new Graph
            {
                sommets = new Sommets[settings.BoardSize * settings.BoardSize]
            };

            for (int v = 0; v < settings.BoardSize; v++)
            {
                for (int u = 0; u < settings.BoardSize; u++)
                {
                    graph.sommets[v * settings.BoardSize + u] = new Sommets
                    {
                        arretes = new Arretes[4],
                        couleur = Couleur.Vide
                    };

                    // Define edges for toroidal connectivity
                    graph.sommets[v * settings.BoardSize + u].arretes[0] = new Arretes { d = v * settings.BoardSize + u, a = v * settings.BoardSize + (u + 1) % settings.BoardSize };
                    graph.sommets[v * settings.BoardSize + u].arretes[1] = new Arretes { d = v * settings.BoardSize + u, a = v * settings.BoardSize + (u - 1 + settings.BoardSize) % settings.BoardSize };
                    graph.sommets[v * settings.BoardSize + u].arretes[2] = new Arretes { d = v * settings.BoardSize + u, a = ((v + 1) % settings.BoardSize) * settings.BoardSize + u };
                    graph.sommets[v * settings.BoardSize + u].arretes[3] = new Arretes { d = v * settings.BoardSize + u, a = ((v - 1 + settings.BoardSize) % settings.BoardSize) * settings.BoardSize + u };
                }
            }
        }
        
        //ajout d'un pion et retournement des pions adverses
        public bool AddPawn(int idSommets, Couleur couleur, List<List<int>> pawnsToFlip)
        {
            // si un coup est valide, on ajoute un pion et on retourne les pions adverses
            if (IsValidMove(idSommets, couleur, pawnsToFlip))
            {
                graph.sommets[idSommets].couleur = couleur;
                pawnsToFlip.ForEach(l => l.ForEach(p => graph.sommets[p].couleur = graph.sommets[p].couleur == Couleur.Noir ? Couleur.Blanc : Couleur.Noir));
                
                //Peut être pas necessaire: peux-t-on encore se bloquer avec les diagonales ?
                // Vérifier si l'adversaire a des coups valides
                Couleur adversaire = (couleur == Couleur.Noir) ? Couleur.Blanc : Couleur.Noir;
                if (!HasValidMove(adversaire))
                {
                    Debug.Log($"Le joueur {adversaire} ne peut pas jouer. Son tour est passé.");
                }
                return true;
            }
            return false;
        }

        public void SetPawn(int idSommets, Couleur couleur)
        {
            graph.sommets[idSommets].couleur = couleur;
        }

        public void DestroyGraph()
        {
            graph = null;
        }

        public void RemoveAllPawns()
        {
            foreach (var sommet in graph.sommets)
            {
                sommet.couleur = Couleur.Vide;
            }
        }

        public void UpdateGraph()
        {
            if(previousSize == settings.BoardSize){
                    return;
                }
                DestroyGraph();
                InitGraph();
        }

        //vérification de la validité du coup 
        public bool IsValidMove(int idSommet, Couleur color, List<List<int>> pawnsToFlip)
        {
            // Vider la liste des pions à retourner
            pawnsToFlip.Clear();
            
            // Vérifier si la case est déjà occupée
            if (graph.sommets[idSommet].couleur != Couleur.Vide)
                return false;
            
            bool validMove = false;
            int[] directionsX = { -1, 0, 1 };
            int[] directionsY = { -1, 0, 1 };
            
            // Parcourir toutes les directions possibles
            foreach (int dx in directionsX)
            {
                foreach (int dy in directionsY)
                {
                    // Ignorer la direction (0,0) car elle ne change pas la position
                    if (dx == 0 && dy == 0)
                        continue;
                    
                    List<int> currentFlip = new List<int>();
                    int x = idSommet % settings.BoardSize;
                    int y = idSommet / settings.BoardSize;
                    int nx = (x + dx + settings.BoardSize) % settings.BoardSize;
                    int ny = (y + dy + settings.BoardSize) % settings.BoardSize;
                    bool hasOpponentBetween = false;
                    int steps = 0;
                    
                    // Parcourir dans la direction jusqu'à la taille maximale du plateau
                    while (steps < settings.BoardSize)
                    {
                        int neighborIndex = ny * settings.BoardSize + nx;
                        
                        // Si la case est vide, arrêter la recherche dans cette direction
                        if (graph.sommets[neighborIndex].couleur == Couleur.Vide)
                            break;
                        
                        // Si la case contient un pion de l'adversaire, ajouter à la liste des pions à retourner
                        if (graph.sommets[neighborIndex].couleur != color)
                        {
                            currentFlip.Add(neighborIndex);
                            hasOpponentBetween = true;
                        }
                        else
                        {
                            // Si un pion de la même couleur est trouvé après des pions adverses, le coup est valide
                            if (hasOpponentBetween)
                            {
                                pawnsToFlip.Add(currentFlip);
                                validMove = true;
                            }
                            break;
                        }
                        
                        // Passer à la case suivante dans la direction
                        nx = (nx + dx + settings.BoardSize) % settings.BoardSize;
                        ny = (ny + dy + settings.BoardSize) % settings.BoardSize;
                        steps++;
                    }
                }
            }
            return validMove;
        }
        public void StartGame()
        {
            int u = Mathf.FloorToInt(settings.BoardSize / 2f) - 1;
            int v = Mathf.FloorToInt(settings.BoardSize / 2f) - 1;
            SetPawn(u + v * settings.BoardSize, Couleur.Noir);
            SetPawn(u + 1 + v * settings.BoardSize, Couleur.Blanc);
            SetPawn(u + (v + 1) * settings.BoardSize, Couleur.Blanc);
            SetPawn(u + 1 + (v + 1) * settings.BoardSize, Couleur.Noir);
        }

        public List<int> GetScore()
        {
            // Implementation for getting the score
            return new List<int>();
        }

        public bool HasValidMove(Couleur couleur)
        {
            List<List<int>> dummyList = new List<List<int>>();
            for (int i = 0; i < graph.sommets.Length; i++)
            {
                if (IsValidMove(i, couleur, dummyList))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

