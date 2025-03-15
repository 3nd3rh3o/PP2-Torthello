using System.Collections.Generic;

namespace Torthello
{
    public class TriangularDoubleBoardGraph : FlatBoardGraph
    {
        public TriangularDoubleBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }

        //initialisation du Graph
        public override void InitGraph()
        {
        }
    }
}