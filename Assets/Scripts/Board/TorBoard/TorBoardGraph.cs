using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    public class TorBoardGraph : IGraph
    {
        private Graph graph;
        private TorBoardSettings settings;

        private List<int> videAdj;
        private List<int> coupPossibleNoir;
        private List<int> coupPossibleBlanc;

        private int pawnBlanc;
        private int pawnNoir;

        private int previousSize;
        public TorBoardGraph(TorBoardSettings settings)
        {
            this.settings = settings;
        }

        //initialisation du Graph
        public void InitGraph()
        {
            videAdj = new List<int>();
            coupPossibleNoir = new List<int>();
            coupPossibleBlanc = new List<int>();

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
                    graph.sommets[v * settings.BoardSize + u].arretes[0] = new Arretes { d = v * settings.BoardSize + u, a = v * settings.BoardSize + (u + 1) % settings.BoardSize }; // right
                    graph.sommets[v * settings.BoardSize + u].arretes[1] = new Arretes { d = v * settings.BoardSize + u, a = v * settings.BoardSize + (u - 1 + settings.BoardSize) % settings.BoardSize }; // left
                    graph.sommets[v * settings.BoardSize + u].arretes[2] = new Arretes { d = v * settings.BoardSize + u, a = ((v + 1) % settings.BoardSize) * settings.BoardSize + u }; // bottom
                    graph.sommets[v * settings.BoardSize + u].arretes[3] = new Arretes { d = v * settings.BoardSize + u, a = ((v - 1 + settings.BoardSize) % settings.BoardSize) * settings.BoardSize + u }; // top
                }
            }
        }
        
        //ajout d'un pion et retournement des pions adverses
        public bool AddPawn(int idSommets, Couleur couleur, List<List<int>> pawnsToFlip)
        {
            if (couleur == Couleur.Noir)
            {
                pawnNoir++;
            }
            else
            {
                pawnBlanc++;
            }

            // si un coup est valide, on ajoute un pion et on retourne les pions adverses
            if (IsValidMove(idSommets, couleur, pawnsToFlip))
            {
                graph.sommets[idSommets].couleur = couleur;
                foreach (List<int> pions in pawnsToFlip)
                {
                    foreach (int p in pions)
                    {
                        graph.sommets[p].couleur = graph.sommets[p].couleur == Couleur.Noir ? Couleur.Blanc : Couleur.Noir;
                        if (couleur == Couleur.Noir)
                        {
                            pawnNoir++;
                            pawnBlanc--;
                        }
                        else
                        {
                            pawnBlanc++;
                            pawnNoir--;
                        }
                    }
                }

                if (videAdj.Contains(idSommets)) videAdj.Remove(idSommets);

                foreach (Arretes arrete in graph.sommets[idSommets].arretes)
                {
                    if (arrete == null) continue;
                    if (graph.sommets[arrete.a].couleur == Couleur.Vide && !videAdj.Contains(arrete.a))
                    {
                        videAdj.Add(arrete.a);
                    }
                }

                if (couleur == Couleur.Noir)
                {
                    coupPossibleBlanc = new List<int>();
                }
                else
                {
                    coupPossibleNoir = new List<int>();
                }

                foreach (int s in videAdj)
                {
                    if (couleur == Couleur.Noir && IsValidMove(s, Couleur.Blanc, new List<List<int>>()))
                    {
                        coupPossibleBlanc.Add(s);
                    }
                    else if (couleur == Couleur.Blanc && IsValidMove(s, Couleur.Noir, new List<List<int>>()))
                    {
                        coupPossibleNoir.Add(s);
                    }
                }
                return true;
            }
            return false;
        }

        public void RemovePawn(int idSommets, List<List<int>> pawnsToFlip)
        {
            graph.sommets[idSommets].couleur = Couleur.Vide;
            foreach (List<int> pions in pawnsToFlip)
            {
                foreach (int p in pions)
                {
                    graph.sommets[p].couleur = graph.sommets[p].couleur == Couleur.Noir ? Couleur.Blanc : Couleur.Noir;
                    if (graph.sommets[p].couleur == Couleur.Noir)
                    {
                        pawnNoir++;
                        pawnBlanc--;
                    }
                    else
                    {
                        pawnBlanc++;
                        pawnNoir--;
                    }
                }
            }
        }

        public void SetPawn(int idSommets, Couleur couleur)
        {
            graph.sommets[idSommets].couleur = couleur;
            if (videAdj.Contains(idSommets)) videAdj.Remove(idSommets);
            foreach (Arretes arrete in graph.sommets[idSommets].arretes)
            {
                if (arrete == null) continue;
                if (graph.sommets[arrete.a].couleur == Couleur.Vide && !videAdj.Contains(arrete.a))
                {
                    videAdj.Add(arrete.a);
                }
            }
        }

        public void DestroyGraph()
        {
            videAdj = null;
            coupPossibleNoir = null;
            coupPossibleBlanc = null;
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
            pawnBlanc = 2;
            pawnNoir = 2;
        }

        public List<int> GetScore()
        {
            return new List<int> { pawnBlanc, pawnNoir };
        }

        public bool NoPlacementAvailable(Couleur couleur)
        {
            return couleur == Couleur.Blanc ? coupPossibleBlanc.Count == 0 : coupPossibleNoir.Count == 0;
        }

        public int GetBoardSize()
        {
            return settings.BoardSize*settings.BoardSize;
        }
        // HORRIBLE METHODE: A REFAIRE, il faut calculer le nombre de pions de chaque couleur à chaque pion posé / retiré comme dans flatboard. PLACEHOLDER
        public bool IsGameOver()
        {
            return NoPlacementAvailable(Couleur.Noir) && NoPlacementAvailable(Couleur.Blanc);
        }
    }
}

