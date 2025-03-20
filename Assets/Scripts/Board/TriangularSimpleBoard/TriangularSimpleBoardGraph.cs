using System;
using System.Collections.Generic;

namespace Torthello
{
    
    public class TriangularSimpleBoardGraph : TriangularBoardGraph
    {
        public TriangularSimpleBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }
        //initialisation du Graph  
        public override void InitGraph(){
             
            videAdj = new List<int>();
            coupPossibleNoir = new List<int>();
            coupPossibleBlanc = new List<int>();

            graph = new Graph
            {
                sommets = new Sommets[Somme(settings.BoardHeight)]
            };

            prevHeight = settings.BoardHeight;
            prevWidth = settings.BoardWidth;
            int sommeid = 0;
            for (int v = 1; v <= settings.BoardHeight; v++)
            {
                int reste = v;
                while(reste>0)
                {                   
                    graph.sommets[sommeid] = new Sommets();
                    // listes d'arretes du sommet
                    graph.sommets[sommeid].arretes = new Arretes[6];
                    //le contenu du sommet
                    graph.sommets[sommeid].couleur = Couleur.Vide;

                    // test des cas particuliers.
                    int idSommet = sommeid;

                    if(idSommet==0){
                        graph.sommets[idSommet].arretes[3] = new Arretes
                        {
                            a = idSommet + v+1,
                            d = idSommet
                        };
                        graph.sommets[idSommet].arretes[4] = new Arretes
                        {
                            a = idSommet + v,
                            d = idSommet
                        };
                    }
                    else if(v==settings.BoardHeight){
                        if(idSommet==Somme(v-1)){
                            graph.sommets[idSommet].arretes[1] = new Arretes
                            {
                                a = idSommet - (v-1),
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[2] = new Arretes
                            {
                                a = idSommet + 1,
                                d = idSommet
                            };
                        }
                        else if(idSommet==Somme(v-1)+v-1){
                            graph.sommets[idSommet].arretes[0] = new Arretes
                            {
                                a = idSommet - v,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[5] = new Arretes
                            {
                                a = idSommet - 1,
                                d = idSommet
                            };
                        }
                        else{
                            graph.sommets[idSommet].arretes[0] = new Arretes
                            {
                                a = idSommet - v,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[1] = new Arretes
                            {
                                a = idSommet - (v-1),
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[2] = new Arretes
                            {
                                a = idSommet + 1,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[5] = new Arretes
                            {
                                a = idSommet - 1,
                                d = idSommet
                            };
                        }
                    }
                    else{
                        if(idSommet==Somme(v-1)){
                            graph.sommets[idSommet].arretes[1] = new Arretes
                            {
                                a = idSommet - (v-1),
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[2] = new Arretes
                            {
                                a = idSommet + 1,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[3] = new Arretes
                            {
                                a = idSommet + v+1,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[4] = new Arretes
                            {
                                a = idSommet + v,
                                d = idSommet
                            };
                        }
                        if(idSommet==Somme(v-1)+v-1){
                            graph.sommets[idSommet].arretes[0] = new Arretes
                            {
                                a = idSommet - v,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[3] = new Arretes
                            {
                                a = idSommet + v+1,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[4] = new Arretes
                            {
                                a = idSommet + v,
                                d = idSommet
                            };
                            graph.sommets[idSommet].arretes[5] = new Arretes
                            {
                                a = idSommet - 1,
                                d = idSommet
                            };
                        }
                    }
                    //regarde si toutes les arretes sont null
                    int fait = 0;
                    for(int i=0; i<6; i++){
                        if(graph.sommets[idSommet].arretes[i] != null){
                            fait = 1;
                        }
                    }
                    if(fait==0){
                        graph.sommets[idSommet].arretes[0] = new Arretes
                        {
                            a = idSommet - v,
                            d = idSommet
                        };
                        graph.sommets[idSommet].arretes[1] = new Arretes
                        {
                            a = idSommet - (v-1),
                            d = idSommet
                        };
                        graph.sommets[idSommet].arretes[2] = new Arretes
                        {
                            a = idSommet + 1,
                            d = idSommet
                        };
                        graph.sommets[idSommet].arretes[3] = new Arretes
                        {
                            a = idSommet + v+1,
                            d = idSommet
                        };
                        graph.sommets[idSommet].arretes[4] = new Arretes
                        {
                            a = idSommet + v,
                            d = idSommet
                        };
                        graph.sommets[idSommet].arretes[5] = new Arretes
                        {
                            a = idSommet - 1,
                            d = idSommet
                        };
                    }
                    sommeid++;
                    reste--;
                }
            }
        }


        public int Somme(int v){
            int somme = 0;
            while(v > 0){
                somme += v;
                v--;
            }
            return somme;
        }
    }

}