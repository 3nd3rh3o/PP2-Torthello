using Unity.Mathematics;
using System.Collections.Generic;

namespace Torthello
{
    
    public class CyclesBoardGraph : FlatBoardGraph
    {
        protected int[] edges;
        protected float3[] edgesColors;

        public CyclesBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }

        //initialisation du Graph  
        public override void InitGraph(){
            videAdj = new List<int>();
            coupPossibleNoir = new List<int>();
            coupPossibleBlanc = new List<int>();

            if(settings.BoardWidth == 1)
            {
            edges = new int[settings.BoardHeight];
            edgesColors = new float3[settings.BoardHeight];
            }
            else
            {
            edges = new int[settings.BoardHeight*2];
            edgesColors = new float3[settings.BoardHeight*2];
            }

            graph = new Graph
            {
                sommets = new Sommets[settings.BoardHeight]
            };

            prevHeight = settings.BoardHeight;
            prevWidth = settings.BoardWidth;
            int sommeid = 0;
            int debutedges = 0;

            for (int v = 1; v <= settings.BoardWidth; v++)
            {
                int restesommets = settings.BoardHeight;
                while(restesommets>0)
                {                   
                    graph.sommets[sommeid] = new Sommets();

                    // listes d'arretes du sommet
                    if( settings.BoardWidth == 1){
                        graph.sommets[sommeid].arretes = new Arretes[2];  
                    }
                    else{
                        graph.sommets[sommeid].arretes = new Arretes[4];
                    }
                    //le contenu du sommet
                    graph.sommets[sommeid].couleur = Couleur.Vide;

                    // test des cas particuliers.
                    int idSommet = sommeid;
                    if(settings.BoardWidth == 1){
                        graph.sommets[idSommet].arretes[0] = new Arretes
                        {
                            a = (idSommet + 1 ) % settings.BoardHeight,
                            d = idSommet
                        };

                        edges[debutedges] = graph.sommets[idSommet].arretes[0].a;
                        edgesColors[idSommet] = new float3(.2f, .2f, .2f); //TODO: mettre la couleur en NOIR
                        debutedges++;
                        edges[debutedges] = graph.sommets[idSommet].arretes[0].d;
                        debutedges++;

                        if(idSommet == 0){
                            graph.sommets[idSommet].arretes[1] = new Arretes
                            {
                                a = settings.BoardHeight - 1,
                                d = idSommet
                            };
                        }
                        else{
                            graph.sommets[idSommet].arretes[1] = new Arretes
                            {
                                a = (idSommet - 1) % settings.BoardHeight,
                                d = idSommet
                            };
                        }
                        }
                    else{

                        graph.sommets[idSommet].arretes[0] = new Arretes
                        {
                            a = (idSommet + 1 ) % settings.BoardHeight,
                            d = idSommet
                        };
                        //TODO: mettre la couleur en NOIR
                        edges[debutedges] = graph.sommets[idSommet].arretes[0].a;
                        edgesColors[idSommet] = new float3(.2f, .2f, .2f);
                        debutedges++;
                        edges[debutedges] = graph.sommets[idSommet].arretes[0].d;
                        debutedges++;

                        graph.sommets[idSommet].arretes[1] = new Arretes
                        {
                            a = (idSommet + settings.BoardWidth) % settings.BoardHeight,
                            d = idSommet
                        };
                        //TODO: mettre la couleur en une autre couleur mais pas noir ou rouge   
                        edges[debutedges] = graph.sommets[idSommet].arretes[1].a;
                        edgesColors[idSommet] = new float3(.2f, .2f, .2f);
                        debutedges++;
                        edges[debutedges] = graph.sommets[idSommet].arretes[1].d;
                        debutedges++;

                        graph.sommets[idSommet].arretes[2] = new Arretes
                        {
                            a = (idSommet + settings.BoardHeight - settings.BoardWidth) % settings.BoardHeight,
                            d = idSommet
                        };
                        if(idSommet == 0){
                            graph.sommets[idSommet].arretes[3] = new Arretes
                            {
                                a = settings.BoardHeight - 1,
                                d = idSommet
                            };
                        }
                        else{
                            graph.sommets[idSommet].arretes[3] = new Arretes
                            {
                                a = (idSommet - 1) % settings.BoardHeight,
                                d = idSommet
                            };
                        }

                    }

                    sommeid++;
                    restesommets--;
                }   
            }
        }

        public int[] GetEdges()
        {
            return edges;
        }
        public float3[] GetEdgesColors()
        {
            return edgesColors;
        }
    }
}