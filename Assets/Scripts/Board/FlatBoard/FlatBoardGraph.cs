using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Torthello
{

    public class FlatBoardGraph : IGraph
    {

        protected Graph graph;

        protected int prevWidth;

        protected int prevHeight;

        protected List<int> videAdj;
        protected List<int> coupPossibleNoir;
        protected List<int> coupPossibleBlanc;

        protected int pawnBlanc;
        protected int pawnNoir;

        protected Settings settings;

        public FlatBoardGraph(Settings settings)
        {
            this.settings = settings;
        }
        public bool AddPawn(int idSommet, Couleur couleur, List<List<int>> pionsARetournes)
        {
            // on ajoute un pion selement si le coup est valide
            if (IsValidMove(idSommet, couleur, pionsARetournes))
            {
                if(couleur == Couleur.Noir){
                    pawnNoir++;
                }
                else{
                    pawnBlanc++;
                }
                graph.sommets[idSommet].couleur = couleur;

                foreach (List<int> pions in pionsARetournes)
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
                if(videAdj.Contains(idSommet)) videAdj.Remove(idSommet);

                foreach (Arretes arrete in graph.sommets[idSommet].arretes)
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
                Debug.Log($"Score: {pawnBlanc} - {pawnNoir}");
                return true;
            }
            return false;
        }

        public void RemovePawn(int idSommet, List<List<int>> pawnsToFlip)
        {
            //Réduire le compteur de pions pour le couleur du pion, retiré
            if (graph.sommets[idSommet].couleur == Couleur.Noir ) pawnNoir--; 
            else pawnBlanc--;

            // Remettre la case à vide
            graph.sommets[idSommet].couleur = Couleur.Vide;

            // Remettre les pions retournés à leur couleur d'origine
            pawnsToFlip.ForEach(l => l.ForEach(p => {if (graph.sommets[p].couleur == Couleur.Noir) {
                pawnNoir--;
                pawnBlanc++;
                graph.sommets[p].couleur = Couleur.Blanc;
            }
            else
            {
                pawnNoir++;
                pawnBlanc--;
                graph.sommets[p].couleur = Couleur.Noir;
            }}));

            // Repositionner l'id de la case dans les cases adjacentes vides
            if (!videAdj.Contains(idSommet)) videAdj.Add(idSommet);
            
            // Retirer les cases vides qui étaient uniquement adjacentes au pion retiré
            foreach (Arretes arrete in graph.sommets[idSommet].arretes)
            {
                if (arrete == null) continue;
                int idVide = arrete.a;
                bool isAdjacentToOther = false;
                foreach (Arretes adjArrete in graph.sommets[idVide].arretes)
                {
                    if (adjArrete == null) continue;
                    if (graph.sommets[adjArrete.a].couleur != Couleur.Vide)
                    {
                        isAdjacentToOther = true;
                        break;
                    }
                }
                if (!isAdjacentToOther)
                {
                    videAdj.Remove(idVide);
                }
            }

            // Mettre à jour les listes de coups possibles
            coupPossibleNoir.Clear();
            coupPossibleBlanc.Clear();

            foreach (int s in videAdj)
            {
                if (IsValidMove(s, Couleur.Blanc, new List<List<int>>()))
                {
                    coupPossibleBlanc.Add(s);
                }
                if (IsValidMove(s, Couleur.Noir, new List<List<int>>()))
                {
                    coupPossibleNoir.Add(s);
                }
            }
        }

        public void SetPawn(int idSommet, Couleur couleur)
        {
            // on initialise un pion
            graph.sommets[idSommet].couleur = couleur;
            if (videAdj.Contains(idSommet)) videAdj.Remove(idSommet);
            foreach (Arretes arrete in graph.sommets[idSommet].arretes)
            {
                if (graph.sommets[arrete.a].couleur == Couleur.Vide && !videAdj.Contains(arrete.a))
                {
                    videAdj.Add(arrete.a);
                }
            }
            
        }

        public void DestroyGraph()
        {
            // on detruit le graph
            videAdj = null;
            coupPossibleNoir = null;
            coupPossibleBlanc = null;
            graph = null;
        }

        //initialisation du Graph
        public virtual void InitGraph()
        {
            videAdj = new List<int>();
            coupPossibleNoir = new List<int>();
            coupPossibleBlanc = new List<int>();

            graph = new Graph
            {
                sommets = new Sommets[settings.BoardWidth * settings.BoardHeight]
            };

            prevHeight = settings.BoardHeight;
            prevWidth = settings.BoardWidth;

            for (int v = 0; v < settings.BoardHeight; v++)
            {

                for (int u = 0; u < settings.BoardWidth; u++)
                {

                    graph.sommets[v * settings.BoardWidth + u] = new Sommets();
                    // listes d'arretes du sommet
                    graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[8];
                    //le contenu du sommet
                    graph.sommets[v * settings.BoardWidth + u].couleur = Couleur.Vide;

                    // test des cas particuliers.
                    int idSommet = v * settings.BoardWidth + u;

                    // 0 1 2    C B C
                    // 7 _ 3 => B _ B
                    // 6 5 4    C B C

                    // (-1, -1) (0, -1) (+1, -1)
                    // (-1, 0)  _______  (+1, 0)
                    // (-1, +1) (0, +1) (+1, +1)

                    // (-1, -1) => 3,4,5,8
                    // (-1, 0) => 1,2,3,4,5,8
                    // (-1, +1) => 1,2,3,8

                    // (0, -1) => 3,4,5,6,7,8
                    // (0, +1) => 0,1,2,3,7,8

                    // (+1, -1) => 5,6,7,8
                    // (+1, 0) => 0,1,5,6,7,8
                    // (+1, +1) => 0,1,7,8

                    if (!(u == 0) && !(v == 0))
                    {
                        graph.sommets[idSommet].arretes[0] = new Arretes
                        {
                            a = (v - 1) * settings.BoardWidth + u - 1,
                            d = idSommet
                        };
                    }
                    if (!(v == 0))
                    {
                        graph.sommets[idSommet].arretes[1] = new Arretes
                        {
                            a = (v - 1) * settings.BoardWidth + u,
                            d = idSommet
                        };
                    }
                    if (!(u == settings.BoardWidth - 1) && !(v == 0))
                    {
                        graph.sommets[idSommet].arretes[2] = new Arretes
                        {
                            a = (v - 1) * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                    }
                    if (!(u == settings.BoardWidth - 1))
                    {
                        graph.sommets[idSommet].arretes[3] = new Arretes
                        {
                            a = v * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                    }
                    if (!(u == settings.BoardWidth - 1) && !(v == settings.BoardHeight - 1))
                    {
                        graph.sommets[idSommet].arretes[4] = new Arretes
                        {
                            a = (v + 1) * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                    }
                    if (!(v == settings.BoardHeight - 1))
                    {
                        graph.sommets[idSommet].arretes[5] = new Arretes
                        {
                            a = (v + 1) * settings.BoardWidth + u,
                            d = idSommet
                        };
                    }
                    if (!(u == 0) && !(v == settings.BoardHeight - 1))
                    {
                        graph.sommets[idSommet].arretes[6] = new Arretes
                        {
                            a = (v + 1) * settings.BoardWidth + u - 1,
                            d = idSommet
                        };
                    }
                    if (!(u == 0))
                    {
                        graph.sommets[idSommet].arretes[7] = new Arretes
                        {
                            a = v * settings.BoardWidth + u - 1,
                            d = idSommet
                        };
                    }
                }
            }
        }

        public void RemoveAllPawns()
        {
            // on enleve tous les pions
            foreach (Sommets sommet in graph.sommets)
            {
                sommet.couleur = Couleur.Vide;
            }
        }

        public void UpdateGraph()
        {
            if (settings.isInGame) settings.Score = ((float)pawnBlanc) / (float)(pawnNoir + pawnBlanc);
            if (prevHeight == settings.BoardHeight && prevWidth == settings.BoardWidth)
            {
                return;
            }
            DestroyGraph();
            InitGraph();
        }

        // fonction qui retourne si le coup est valide
        public bool IsValidMove(int idSommet, Couleur couleur, List<List<int>> pionsARetournes)
        {
            bool CoupValide = false;
            Sommets sommetActuel = graph.sommets[idSommet];

            // si le sommet (case) est pas vide on ne peut pas jouer
            if (sommetActuel.couleur != Couleur.Vide)
            {
                return false;
            }

            Couleur inverse = couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
            // (sommetAVisit, dir, pionsARetournes)
            List<Tuple<int, int, List<int>>> parcours = new();
            for (int i = 0; i < 8; i++)
            {
                if (sommetActuel.arretes[i] != null && graph.sommets[sommetActuel.arretes[i].a].couleur == inverse)
                {
                    parcours.Add(new Tuple<int, int, List<int>>(sommetActuel.arretes[i].a, i, new List<int>() { sommetActuel.arretes[i].a }));
                }
            }
            while (parcours.Count > 0)
            {
                for (int i = 0; i < parcours.Count; i++)
                {
                    Arretes nAr = graph.sommets[parcours[i].Item1].arretes[parcours[i].Item2];
                    if (nAr == null)
                    {
                        parcours.RemoveAt(i);
                        i--;
                    }
                    else if (graph.sommets[nAr.a].couleur == Couleur.Vide)
                    {
                        parcours.RemoveAt(i);
                        i--;
                    }
                    else if (graph.sommets[nAr.a].couleur == couleur)
                    {
                        CoupValide = true;
                        pionsARetournes.Add(parcours[i].Item3);
                        parcours.RemoveAt(i);
                        i--;
                    }
                    else if (graph.sommets[nAr.a].couleur == inverse)
                    {
                        parcours[i].Item3.Add(nAr.a);
                        parcours[i] = new(nAr.a, parcours[i].Item2, parcours[i].Item3);
                    }
                }
            }
            return CoupValide;
        }

        public void StartGame()
        {
            int u = Mathf.FloorToInt(settings.BoardWidth / 2f) - 1;
            int v = Mathf.FloorToInt(settings.BoardHeight / 2f) - 1;
            SetPawn(u + v * settings.BoardWidth, Couleur.Noir);
            SetPawn(u + 1 + v * settings.BoardWidth, Couleur.Blanc);
            SetPawn(u + (v + 1) * settings.BoardWidth, Couleur.Blanc);
            SetPawn(u + 1 + (v + 1) * settings.BoardWidth, Couleur.Noir);
            pawnBlanc = 2;
            pawnNoir = 2;
            foreach(int p in videAdj)
            {
                if (IsValidMove(p, Couleur.Blanc, new())) coupPossibleBlanc.Add(p);
                if (IsValidMove(p, Couleur.Noir, new())) coupPossibleNoir.Add(p);
            }
        }

        public List<int> GetScore()
        {
            return new List<int>() { pawnBlanc, pawnNoir };
        }

        public bool NoPlacementAvailable(Couleur couleur)
        {
            return couleur == Couleur.Blanc ? coupPossibleBlanc.Count == 0 : coupPossibleNoir.Count == 0;
        }

        public int GetBoardSize()
        {
            return settings.BoardWidth * settings.BoardHeight;
        }

        public List<int> GetValidMoves(Couleur couleur)
        {
            return couleur == Couleur.Blanc ? new List<int>(coupPossibleBlanc) : new List<int>(coupPossibleNoir);
        }
        public bool IsGameOver()
        {
            return NoPlacementAvailable(Couleur.Noir) && NoPlacementAvailable(Couleur.Blanc);
        }

        public int MINIMAX(int prof)
        {
            return Node.MM(graph, prof, videAdj);
        }
        private class Node
        {
            public int added;
            public List<int> flipped;
            public int nbBlanc;
            public int nbNoir;
            public List<int> coupPossible = new();
            public List<int> videAdj;
            public Couleur tour;

            public Node[] branches;
            

            public Node(Couleur tour, List<int> videAdj)
            {
                this.tour = tour;
                this.videAdj = new(videAdj);
                flipped = new();
                
            }

            public void PlayBranch(Graph graph, int id)
            {
                List<int> flip = new();
                IsValidMove(graph, id, tour, flip);
                if (tour == Couleur.Blanc)
                {
                    nbBlanc++;
                } else {
                    nbNoir++;
                }
                added = id;
                for (int i = 0; i < flip.Count; i++)
                {
                    flipped.Add(flip[i]);
                    if (graph.sommets[flip[i]].couleur == Couleur.Blanc)
                    {
                        nbBlanc--;
                        nbNoir++;
                        graph.sommets[flip[i]].couleur = Couleur.Noir;
                    } else {
                        nbBlanc++;
                        nbNoir--;
                        graph.sommets[flip[i]].couleur = Couleur.Blanc;
                    }
                }
                graph.sommets[id].couleur = tour;
                if (videAdj.Contains(id)) videAdj.Remove(id);
                foreach(int i in videAdj)
                {
                    if (IsValidMove(graph, id, tour == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc, new()))
                    {
                        coupPossible.Add(i);
                    }
                }

            }

            public static int MM(Graph graph, int p, List<int> videAdj)
            {
                Node node = new(Couleur.Noir, videAdj);
                videAdj.ForEach(i => {
                    if (node.IsValidMove(graph, i, Couleur.Noir, new())) node.coupPossible.Add(i);
                });
                node.branches = new Node[node.coupPossible.Count];
                int res = node.coupPossible[0];
                int bestD = 0;

                for (int i = 0; i < node.coupPossible.Count; i++)
                {
                    node.branches[i] = new(Couleur.Noir, videAdj);
                    node.branches[i].PlayBranch(graph, node.coupPossible[i]);
                    Tuple<int, int> r = node.branches[i].Next(graph, p-1);
                    if (bestD<r.Item2 - r.Item1)
                    {
                        res = node.coupPossible[i];
                        bestD = node.branches[i].nbNoir - node.branches[i].nbBlanc;
                    }
                    node.branches[i].UndoThis(graph);
                }
                node = null;
                return res;
            }
            public void UndoThis(Graph graph)
            {
                graph.sommets[added].couleur = Couleur.Vide;
                flipped.ForEach(i => 
                    graph.sommets[i].couleur = graph.sommets[i].couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc
                );
            }

            public Tuple<int, int> Next(Graph graph, int p)
            {
                int SSN = nbNoir;
                int SSB = nbBlanc;
                branches = new Node[coupPossible.Count];
                for (int i = 0; i < branches.Length; i++)
                {
                    branches[i] = new Node(tour == Couleur.Blanc? Couleur.Noir : Couleur.Blanc, videAdj);
                    

                    PlayBranch(graph, coupPossible[i]);
                    if (p > 1)
                    {
                        Tuple<int, int> r = branches[i].Next(graph, p - 1);
                        SSB += r.Item1;
                        SSN += r.Item2;
                    } else {
                        SSN+=branches[i].nbNoir;
                        SSB+=branches[i].nbBlanc;
                    }
                    branches[i].UndoThis(graph);
                }
                return new(SSB, SSN);
            }
            private bool IsValidMove(Graph graph, int idSommets, Couleur couleur, List<int> pionARetournes)
            {

                bool CoupValide = false;
                Sommets sommetActuel = graph.sommets[idSommets];

                // si le sommet (case) est pas vide on ne peut pas jouer
                if (sommetActuel.couleur != Couleur.Vide)
                {
                    return false;
                }

                Couleur inverse = couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
                // (sommetAVisit, dir, pionsARetournes)
                List<Tuple<int, int, List<int>>> parcours = new List<Tuple<int, int, List<int>>>();
                for (int i = 0; i < 8; i++)
                {
                    if (sommetActuel.arretes[i] != null && graph.sommets[sommetActuel.arretes[i].a].couleur == inverse)
                    {
                        parcours.Add(new Tuple<int, int, List<int>>(sommetActuel.arretes[i].a, i, new List<int>() { sommetActuel.arretes[i].a }));
                    }
                }
                while (parcours.Count > 0)
                {
                    for (int i = 0; i < parcours.Count; i++)
                    {
                        Arretes nAr = graph.sommets[parcours[i].Item1].arretes[parcours[i].Item2];
                        if (nAr == null)
                        {
                            parcours.RemoveAt(i);
                            i--;
                        }
                        else if (graph.sommets[nAr.a].couleur == Couleur.Vide)
                        {
                            parcours.RemoveAt(i);
                            i--;
                        }
                        else if (graph.sommets[nAr.a].couleur == couleur)
                        {
                            CoupValide = true;
                            pionARetournes.AddRange(parcours[i].Item3);
                            parcours.RemoveAt(i);
                            i--;
                        }
                        else if (graph.sommets[nAr.a].couleur == inverse)
                        {
                            parcours[i].Item3.Add(nAr.a);
                            parcours[i] = new(nAr.a, parcours[i].Item2, parcours[i].Item3);
                        }
                    }
                }

                return CoupValide;
            }
        }
    }
}