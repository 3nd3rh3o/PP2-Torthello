
using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    
    public class TriangularSimpleBoardGraph : TriangularBoardGraph
    {
        public TriangularSimpleBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }

        public override void StartGame()
        {
           int r0 = settings.BoardHeight / 2;
            int r1 = settings.BoardHeight / 2 - 1;
            int c0 = r0 * (r0 + 1) / 2 + (r0 / 2);
            int c1 = c0 + 1;
            int c2 = r1 * (r1 + 1) / 2 + (r1 / 2);
            int c3 = c2 + 1;
            SetPawn(c0, Couleur.Blanc);
            SetPawn(c1, Couleur.Noir);
            SetPawn(c2, Couleur.Noir);
            SetPawn(c3, Couleur.Blanc);
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
                    foreach(Arretes arrete in graph.sommets[idSommet].arretes){
                        if(arrete != null) fait = 1;
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
            return v*(v+1)/2;
        }
    }

}