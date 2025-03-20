using System;
using System.Collections.Generic;

namespace Torthello
{
    
    public class TriangularBoardGraph : FlatBoardGraph
    {
        public TriangularBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }
        //initialisation du Graph
        public override void InitGraph()
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
                    graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[6];
                    //le contenu du sommet
                    graph.sommets[v * settings.BoardWidth + u].couleur = Couleur.Vide;

                    // test des cas particuliers.
                    int idSommet = v * settings.BoardWidth + u;




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
                    if (!(u == settings.BoardWidth - 1))
                    {
                        graph.sommets[idSommet].arretes[2] = new Arretes
                        {
                            a = v * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                    }
                    if (!(u == settings.BoardWidth - 1) && !(v == settings.BoardHeight - 1))
                    {
                        graph.sommets[idSommet].arretes[3] = new Arretes
                        {
                            a = (v + 1) * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                    }
                    if (!(v == settings.BoardHeight - 1))
                    {
                        graph.sommets[idSommet].arretes[4] = new Arretes
                        {
                            a = (v + 1) * settings.BoardWidth + u,
                            d = idSommet
                        };
                    }
                    if (!(u == 0))
                    {
                        graph.sommets[idSommet].arretes[5] = new Arretes
                        {
                            a = v * settings.BoardWidth + u - 1,
                            d = idSommet
                        };
                    }
                }
            }
        }
        public override bool IsValidMove(int idSommet, Couleur couleur, List<List<int>> pionsARetournes)
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
            for (int i = 0; i < 6; i++)
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
    }

}