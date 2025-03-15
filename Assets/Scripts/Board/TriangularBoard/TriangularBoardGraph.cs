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
        }
}