namespace Tortello
{

    public class FlatBoardGraph : IGraph
    {

        private Graph graph;

        private FlatBoardSettings settings;

        public FlatBoardGraph(FlatBoardSettings settings){
            this.settings = settings;
        }
        public void AddPawn(int idsommets, Couleur couleur)
        {
            graph.sommets
        }

        public void DestroyGraph()
        {
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
            throw new System.NotImplementedException();
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

    }
}