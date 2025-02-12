using System.Collections.Generic;
using UnityEngine;

namespace Tortello
{

    public class FlatBoardGraph : IGraph
    {

        private Graph graph;

        private int prevWidth;

        private int prevHeight;

        private FlatBoardSettings settings;

        public FlatBoardGraph(FlatBoardSettings settings){
            this.settings = settings;
        }
        public bool AddPawn(int idSommets, Couleur couleur, List<List<int>> pionsARetournes)
        {
            // on ajoute un pion selement si le coup est valide
            if(CoupEstValide(idSommets, couleur,pionsARetournes)){
                graph.sommets[idSommets].couleur = couleur;
                return true;
            }
            return false;
        }

        public void SetPawn(int idSommets, Couleur couleur)
        {
                // on initialise un pion
                graph.sommets[idSommets].couleur = couleur;
        }

        public void DestroyGraph()
        {
            // on detruit le graph
            graph = null;
        }

        //initialisation du Graph
        public void InitGraph()
        {
            graph = new Graph
            {
                sommets = new Sommets[settings.BoardWidth * settings.BoardHeight]
            };
            prevHeight= settings.BoardHeight;
            prevWidth = settings.BoardWidth;
            for (int v = 0; v < settings.BoardHeight; v++){
                for (int u = 0; u < settings.BoardWidth; u++){
                    graph.sommets[v * settings.BoardWidth + u] = new Sommets();
                    // listes d'arretes du sommet
                    if(SommetsEstUnCoin(u,v, settings.BoardWidth, settings.BoardHeight)){
                        graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[2];
                    }   
                    else if(SommetsEstUnBord(u,v, settings.BoardWidth, settings.BoardHeight)){
                        graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[3];
                    }   
                    else{
                        graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[4];
                    }
                    //le contenu du sommet
                    graph.sommets[v * settings.BoardWidth + u].couleur = Couleur.Vide;

                    // les arretes du sommet
                    int counter = 0;
                    // left ?
                    if(u > 0){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes
                        {
                            d = v * settings.BoardHeight + u,
                            a = v * settings.BoardHeight + u - 1
                        };
                        counter++;
                    }
                    // top ?
                    if(v > 0){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes
                        {
                            d = v * settings.BoardHeight + u,
                            a = (v - 1) * settings.BoardHeight + u
                        };
                        counter++;
                    }
                    // right ?
                    if(u < settings.BoardWidth - 1){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes
                        {
                            d = v * settings.BoardHeight + u,
                            a = v * settings.BoardHeight + u + 1
                        };
                        counter++;
                    }
                    // bottom ?
                    if(v < settings.BoardHeight - 1){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes
                        {
                            d = v * settings.BoardHeight + u,
                            a = (v + 1) * settings.BoardHeight + u
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
            if(prevHeight == settings.BoardHeight && prevWidth == settings.BoardWidth){
                return;
            }
            DestroyGraph();
            InitGraph();
        }

        // fonction qui retourne si le sommet est un coin
        public static bool SommetsEstUnCoin(int u, int v, int width, int height){

            return(u == 0 && v == 0) || (u == width -1 && v ==0)||(u == 0 && v == height -1)||(u == width-1 && v == height -1);
        }
        // fonction qui retourne si le sommet est un bord
        public bool SommetsEstUnBord(int u, int v, int boarwidth, int boarheight){
            return u == 0 || v ==0|| u == boarwidth -1|| v == boarheight -1;
        }
        // fonction qui retourne si le coup est valide
        public bool CoupEstValide(int idSommets, Couleur couleur,List<List<int>> pionsARetournes){
            bool CoupValide = false;
            Sommets sommetActuel = graph.sommets[idSommets];

            // si le sommet (case) est pas vide on ne peut pas jouer
            if(sommetActuel.couleur != Couleur.Vide){
                return false;
            }

            Couleur inverse = couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
            
            List<Arretes> arretes = new List<Arretes>();
            List<int> directions = new List<int>();
            
            foreach (Arretes arrete in sommetActuel.arretes)
            {
                if(graph.sommets[arrete.a].couleur == inverse){
                    arretes.Add(arrete);
                    directions.Add(arrete.a - arrete.d);
                    pionsARetournes.Add(new List<int>());
                }
            }
            if(arretes.Count == 0){
                return false;
            }

            while(arretes.Count > 0){
                for(int i = 0; i < arretes.Count; i++){
                    if(graph.sommets[arretes[i].a].couleur == inverse){
                        Arretes narrete = GetArreteDansMemoDirection(graph.sommets[arretes[i].a], directions[i]);
                        if(narrete == null){
                            arretes.RemoveAt(i);
                            directions.RemoveAt(i);
                            pionsARetournes.RemoveAt(i);
                            i--;
                        }
                        else{
                            arretes[i] = narrete;
                            pionsARetournes[i].Add(narrete.d);
                        }
                    }
                    else if(graph.sommets[arretes[i].a].couleur == couleur){
                        CoupValide = true;
                        arretes.RemoveAt(i);
                        directions.RemoveAt(i);
                    }
                    else{
                        arretes.RemoveAt(i);
                        directions.RemoveAt(i);
                        pionsARetournes.RemoveAt(i);
                        i--;
                    }
                }
            }
            return CoupValide;
        }
        // fonction qui retourne l'arrete dans la direction donnée
        public Arretes GetArreteDansMemoDirection(Sommets sommet, int direction){
            foreach (Arretes arrete in sommet.arretes)
            {
                if(arrete.a - arrete.d == direction){
                    return arrete;
                }
            }
            return null;
        }

        public void StartGame()
        {
            int u = Mathf.FloorToInt(settings.BoardWidth / 2f);
            int v = Mathf.FloorToInt(settings.BoardHeight / 2f);
            SetPawn(u + v * settings.BoardWidth, Couleur.Noir);
            SetPawn(u + 1 + v * settings.BoardWidth, Couleur.Blanc);
            SetPawn(u + (v + 1) * settings.BoardWidth, Couleur.Blanc);
            SetPawn(u + 1 + (v + 1) * settings.BoardWidth, Couleur.Noir);
        }
    }
}