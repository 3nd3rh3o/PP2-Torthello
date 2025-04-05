using Unity.Mathematics;
using System.Collections.Generic;

namespace Torthello
{

    public class CyclesBoardGraph : FlatBoardGraph
    {

        public CyclesBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }

        //initialisation du Graph  
        public override void InitGraph()
        {
            videAdj = new List<int>();
            coupPossibleNoir = new List<int>();
            coupPossibleBlanc = new List<int>();

            prevHeight = settings.BoardHeight;
            prevWidth = settings.BoardWidth;

            int lCycle = settings.BoardHeight;
            int puissance = settings.BoardWidth;
            graph = new Graph();
            graph.sommets = new Sommets[lCycle];

            for (int i = 0; i < lCycle; i++)
            {
                Sommets node = new();
                node.couleur = Couleur.Vide;
                node.arretes = i == 0 && puissance < lCycle && puissance > 1 ? new Arretes[]
                {
                    new(){d = i, a = (i - 1 + lCycle ) % lCycle},
                    new(){d = i, a = (i + 1) % lCycle},
                    new(){d = i, a = (i - puissance + lCycle) % lCycle},
                    new(){d = i, a = (i + puissance) % lCycle}
                }
                 : new Arretes[]
                {
                    new Arretes(){d = i, a = (i - 1 + lCycle ) % lCycle},
                    new Arretes(){d = i, a = (i + 1) % lCycle},
                    null,
                    null
                };
                graph.sommets[i] = node;
            }

            if (puissance < lCycle && puissance > 1)
            {
                int partialEdge = 0;
                while (partialEdge != -1)
                {
                    int idCurrSommet = graph.sommets[partialEdge].arretes[3].a;
                    Sommets currSommet = graph.sommets[idCurrSommet];
                    currSommet.arretes[2] = new() { d = idCurrSommet, a = (idCurrSommet - puissance + lCycle) % lCycle };
                    currSommet.arretes[3] = new() { d = idCurrSommet, a = (idCurrSommet + puissance) % lCycle };
                    partialEdge = currSommet.arretes[3].a == 0 ? -1 : idCurrSommet;
                }
            }
        }
        public override void StartGame()
        {
            SetPawn(0, Couleur.Noir);
            SetPawn(1, Couleur.Blanc);
            SetPawn(2, Couleur.Noir);
            SetPawn(3, Couleur.Blanc);
            pawnBlanc = 2;
            pawnNoir = 2;
            foreach (int p in videAdj)
            {
                if (IsValidMove(p, Couleur.Blanc, new())) coupPossibleBlanc.Add(p);
                if (IsValidMove(p, Couleur.Noir, new())) coupPossibleNoir.Add(p);
            }
        }
    }
}