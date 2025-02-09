using System.Collections.Generic;

namespace Tortello
{

    public class FlatBoardGraph : IGraph
    {

        private Graph graph;

        private FlatBoardSettings settings;

        public FlatBoardGraph(FlatBoardSettings settings){
            this.settings = settings;
        }
        public void AddPawn(int idsommets, Couleur couleur, List<List<int>> pionsretournes)
        {
            // on ajoute un pion selement si le coup est valide
            if(!CoupEstValide(idsommets, couleur,pionsretournes)){
                graph.sommets[idsommets].couleur = couleur;
            }
            
        }

        public void SetPawn(int idsommets, Couleur couleur)
        {
                graph.sommets[idsommets].couleur = couleur;
        }

        public void DestroyGraph()
        {
            // on detruit le graph
            graph = null;
        }

        //initialisation du Graph
        public void InitGraph()
        {
            graph = new Graph();
            graph.sommets = new Sommets[settings.BoardWidth * settings.BoardHeight];
            for (int v = 0; v < settings.BoardHeight; v++){
                for (int u = 0; u < settings.BoardWidth; u++){

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
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes();
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].d = v * settings.BoardHeight + u;
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].a =v * settings.BoardHeight + u - 1;
                        counter++;
                    }
                    // top ?
                    if(v > 0){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes();
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].d = v * settings.BoardHeight + u;
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].a = (v - 1) * settings.BoardHeight + u;
                        counter++;
                    }
                    // right ?
                    if(v > 0){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes();
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].d = v * settings.BoardHeight + u;
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].a = v * settings.BoardHeight + u + 1;
                        counter++;
                    }
                    // bottom ?
                    if(v > 0){
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter] = new Arretes();
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].d = v * settings.BoardHeight + u;
                        graph.sommets[v * settings.BoardWidth + u].arretes[counter].a = (v + 1) * settings.BoardHeight + u;
                        counter++;
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
            throw new System.NotImplementedException();
        }

        public static bool SommetsEstUnCoin(int u, int v, int width, int height){

            return(u == 0 && v == 0) || (u == width -1 && v ==0)||(u == 0 && v == height -1)||(u == width-1 && v == height -1);
        }
        public bool SommetsEstUnBord(int u, int v, int boarwidth, int boarheight){
            return u == 0 || v ==0|| u == boarwidth -1|| v == boarheight -1;
        }
        public bool CoupEstValide(int idsommets, Couleur couleur,List<List<int>> pionsretournes){
            bool CoupValide = false;
            Sommets sommetactuel = graph.sommets[idsommets];

            // si le sommet (case) est pas vide on ne peut pas jouer
            if(sommetactuel.couleur != Couleur.Vide){
                return false;
            }

            Couleur inverse;
            if(couleur == Couleur.Blanc){
                inverse = Couleur.Noir;
            }
            else{
                 inverse = Couleur.Blanc;
            }
            // peut aussi l'ecrire couleur inverse en une ligne (cf mickael)  
            // Couleur inverse = couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
            List<Arretes> arretes = new List<Arretes>();
            List<List<int>> directions = new List<List<int>>();
            
            foreach (Arretes arrete in sommetactuel.arretes)
            {
                if(graph.sommets[arrete.a].couleur == inverse){
                    arretes.Add(arrete);
                    directions.Add(new List<int>{arrete.a, arrete.d});
                    pionsretournes.Add(new List<int>{arrete.a});
                }
            }
            if(arretes.Count == 0){
                return false;
            }

            while(arretes.Count == 0){
                for(int i = 0; i < arretes.Count; i++){
                    if(graph.sommets[arretes[i].a].couleur == inverse){
                        Arretes narrete = GetArreteDansMemoDirection(graph.sommets[arretes[i].a], directions[i]);
                        if(narrete == null){
                            arretes.RemoveAt(i);
                            directions.RemoveAt(i);
                            pionsretournes.RemoveAt(i);
                            i--;
                        }
                        else{
                            arretes[i] = narrete;
                            pionsretournes[i].Add(narrete.d);
                        }
                    }
                    // si on trouve une case de la meme couleur que le joueur on peut jouer
                    else if(graph.sommets[arretes[i].a].couleur == couleur){
                        CoupValide = true;
                        arretes.RemoveAt(i);
                        directions.RemoveAt(i);
                    }
                    else{
                        arretes.RemoveAt(i);
                        directions.RemoveAt(i);
                        pionsretournes.RemoveAt(i);
                        i--;
                    }
                }
            }
            return CoupValide;
        }
        // fonction qui retourne l'arrete dans la direction donnée
        public Arretes GetArreteDansMemoDirection(Sommets sommet, List<int> direction){
            foreach (Arretes arrete in sommet.arretes)
            {
                if(arrete.a - arrete.d == direction[1]){
                    return arrete;
                }
            }
            return null;
        }
    }
}