
using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    
    public class CyclesBoardGraph : FlatBoardGraph
    {
        public CyclesBoardGraph(Settings settings) : base(settings)
        {
            this.settings = settings;
        }

        //initialisation du Graph  
        public override void InitGraph(){
             
        }
    }

}