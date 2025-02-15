using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Tortello
{

    public class FlatBoardGraph : IGraph
    {

        private Graph graph;

        private int prevWidth;

        private int prevHeight;

        private List<int> videAdj;
        private List<int> coupPossibleNoir;
        private List<int> coupPossibleBlanc;



        private FlatBoardSettings settings;

        public FlatBoardGraph(FlatBoardSettings settings){
            this.settings = settings;
        }
        public bool AddPawn(int idSommets, Couleur couleur, List<List<int>> pionsARetournes)
        {
            // on ajoute un pion selement si le coup est valide
            if(CoupEstValide(idSommets, couleur,pionsARetournes)){
                graph.sommets[idSommets].couleur = couleur;
                pionsARetournes.ForEach(l => l.ForEach(p => graph.sommets[p].couleur = graph.sommets[p].couleur == Couleur.Noir ? Couleur.Blanc : Couleur.Noir));
                if(videAdj.Contains(idSommets)) videAdj.Remove(idSommets);

                foreach (Arretes arrete in graph.sommets[idSommets].arretes)
                {
                    if(graph.sommets[arrete.a].couleur == Couleur.Vide && !videAdj.Contains(arrete.a)){
                        videAdj.Add(arrete.a);
                    }
                }

                if(couleur == Couleur.Noir){
                    coupPossibleBlanc = new List<int>();
                }
                else{
                    coupPossibleNoir = new List<int>();
                }

                foreach(int s in videAdj){
                    if(couleur == Couleur.Noir && CoupEstValide(s, Couleur.Blanc, new List<List<int>>())){
                        coupPossibleBlanc.Add(s);
                    }
                    else if(couleur == Couleur.Blanc && CoupEstValide(s, Couleur.Noir, new List<List<int>>())){
                        coupPossibleNoir.Add(s);
                    }
                }
                return true;
            }
            return false;
        }

        public void SetPawn(int idSommets, Couleur couleur)
        {
                // on initialise un pion
                graph.sommets[idSommets].couleur = couleur;
                if(videAdj.Contains(idSommets)) videAdj.Remove(idSommets);
                foreach (Arretes arrete in graph.sommets[idSommets].arretes)
                {
                    if(graph.sommets[arrete.a].couleur == Couleur.Vide && !videAdj.Contains(arrete.a)){
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
        public void InitGraph()
        {
            videAdj = new List<int>();
            coupPossibleNoir = new List<int>(); 
            coupPossibleBlanc = new List<int>();

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
                        graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[3];
                    }   
                    else if(SommetsEstUnBord(u,v, settings.BoardWidth, settings.BoardHeight)){
                        graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[5];
                    }   
                    else{
                        graph.sommets[v * settings.BoardWidth + u].arretes = new Arretes[8];
                    }
                    //le contenu du sommet
                    graph.sommets[v * settings.BoardWidth + u].couleur = Couleur.Vide;

                    // test des cas particuliers.
                    int idSommet = v * settings.BoardWidth + u;

                    // 0 1 2    C B C
                    // 7 8 3 => B _ B
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
                    
                    int counter = 0;
                    if (!(u==0) || !(v==0))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = (v - 1) * settings.BoardWidth + u - 1,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(v==0))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = (v-1) * settings.BoardWidth + u,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(u==settings.BoardWidth-1) || !(v==0))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = (v - 1) * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(u==settings.BoardWidth-1))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = v * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(u==settings.BoardWidth-1) || !(v==settings.BoardHeight-1))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = (v + 1) * settings.BoardWidth + u + 1,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(v==settings.BoardHeight-1))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = (v + 1) * settings.BoardWidth + u,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(u==0) || !(v==settings.BoardHeight-1))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
                            a = (v + 1) * settings.BoardWidth + u - 1,
                            d = idSommet
                        };
                        counter++;
                    }
                    if (!(u==0))
                    {
                        graph.sommets[idSommet].arretes[counter] = new Arretes { 
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
                    arretes.Add(GetArreteDansMemoDirection(graph.sommets[arrete.a], arrete.a - arrete.d));
                    directions.Add(arrete.a - arrete.d);
                    pionsARetournes.Add(new List<int>(){arrete.a});
                }
            }
            if(arretes.Count == 0){
                return false;
            }
            int c = arretes.Count;
            while(c > 0){
                for(int i = 0; i < directions.Count; i++){
                    if (directions[i] == 0) continue;
                    if(graph.sommets[arretes[i].a].couleur == inverse){
                        Arretes narrete = GetArreteDansMemoDirection(graph.sommets[arretes[i].a], directions[i]);
                        if(narrete == null){
                            directions[i] = 0;
                            pionsARetournes[i] = new();
                            c--;
                        }
                        else {
                            arretes[i] = narrete;
                            pionsARetournes[i].Add(narrete.d);
                        }
                    }
                    else if(graph.sommets[arretes[i].a].couleur == couleur){
                        CoupValide = true;
                        arretes[i] = null;
                        directions[i] = 0;
                        c--;
                    }
                    else{
                        arretes[i] = null;
                        directions[i] = 0;
                        pionsARetournes[i] = new();
                        c--;
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
            int u = Mathf.FloorToInt(settings.BoardWidth / 2f) - 1;
            int v = Mathf.FloorToInt(settings.BoardHeight / 2f) - 1;
            SetPawn(u + v * settings.BoardWidth, Couleur.Noir);
            SetPawn(u + 1 + v * settings.BoardWidth, Couleur.Blanc);
            SetPawn(u + (v + 1) * settings.BoardWidth, Couleur.Blanc);
            SetPawn(u + 1 + (v + 1) * settings.BoardWidth, Couleur.Noir);
        }

        public List<int> GetScore()
        {
            int blanc = 0;
            int noir = 0;
            foreach (Sommets sommet in graph.sommets)
            {
                if(sommet.couleur == Couleur.Blanc){
                    blanc++;
                }
                else if(sommet.couleur == Couleur.Noir){
                    noir++;
                }
            }
            return new List<int>(){blanc, noir};
            
        }

        public bool NoPlacementAvailable(Couleur couleur)
        {
          return couleur == Couleur.Blanc ? coupPossibleBlanc.Count == 0 : coupPossibleNoir.Count == 0;
        }
    }
}