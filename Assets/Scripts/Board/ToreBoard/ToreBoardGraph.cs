using System.Collections.Generic;

namespace Tortello
{

    public class ToreBoardGraph : FlatBoardGraph
    {
        public ToreBoardGraph(FlatBoardSettings settings) : base(settings)
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

            for (int v = 1; v <= settings.BoardHeight; v++)
            {

                for (int u = 1; u <= settings.BoardWidth; u++)
                {

                    graph.sommets[v % settings.BoardHeight * settings.BoardWidth + (u % settings.BoardWidth)] = new Sommets();
                    // listes d'arretes du sommet
                    graph.sommets[v % settings.BoardHeight * settings.BoardWidth + (u % settings.BoardWidth)].arretes = new Arretes[8];
                    //le contenu du sommet
                    graph.sommets[v % settings.BoardHeight * settings.BoardWidth + (u % settings.BoardWidth)].couleur = Couleur.Vide;

                    // test des cas particuliers.
                    int idSommet = v % settings.BoardHeight * settings.BoardWidth + (u % settings.BoardWidth);


                        graph.sommets[idSommet].arretes[0] = new Arretes
                        {
                            a = (v - 1) % settings.BoardHeight * settings.BoardWidth + ((u - 1) % settings.BoardWidth),
                            d = idSommet
                        };

                        graph.sommets[idSommet].arretes[1] = new Arretes
                        {
                            a = (v - 1) % settings.BoardHeight * settings.BoardWidth + (u % settings.BoardWidth),
                            d = idSommet
                        };

                        graph.sommets[idSommet].arretes[2] = new Arretes
                        {
                            a = (v - 1) % settings.BoardHeight * settings.BoardWidth + ((u + 1) % settings.BoardWidth),
                            d = idSommet
                        };

                        graph.sommets[idSommet].arretes[3] = new Arretes
                        {
                            a = v % settings.BoardHeight * settings.BoardWidth + ((u + 1) % settings.BoardWidth),
                            d = idSommet
                        };

                        graph.sommets[idSommet].arretes[4] = new Arretes
                        {
                            a = (v + 1) % settings.BoardHeight * settings.BoardWidth + ((u + 1) % settings.BoardWidth),
                            d = idSommet
                        };

                        graph.sommets[idSommet].arretes[5] = new Arretes
                        {
                            a = (v + 1) % settings.BoardHeight * settings.BoardWidth + (u % settings.BoardWidth),
                            d = idSommet
                        };

                        graph.sommets[idSommet].arretes[6] = new Arretes
                        {
                            a = (v + 1) % settings.BoardHeight * settings.BoardWidth + ((u - 1) % settings.BoardWidth),
                            d = idSommet
                        };
 
                        graph.sommets[idSommet].arretes[7] = new Arretes
                        {
                            a = v % settings.BoardHeight * settings.BoardWidth + ((u - 1) % settings.BoardWidth),
                            d = idSommet
                        };
                }
            }
        }
    }
}