using System;
using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    /// <summary>
    /// Implémentation d'un joueur IA qui utilise une variante l'algorithme minimax pour choisir le meilleur coup.
    /// Il s'agit d'une implémentation optimisée de l'algorithme minimax.
    /// </summary>
    public class PlayerMiniMaxV2 : IPlayerAI
    {
        private IGraph graph;
        private Couleur couleur;
        private int prof;
        public PlayerMiniMaxV2(IGraph graph, Couleur couleur, int prof)
        {
            this.graph = graph;
            this.couleur = couleur;
            this.prof = prof;
        }
        public int GetBestMove()
        {
            //placeholder le temps de l'implémentation
            return graph.GetValidMoves(couleur)[0];
            //return Node.MM(graph, prof, videAdj);
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