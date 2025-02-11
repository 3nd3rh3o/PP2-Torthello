using UnityEngine;

namespace Tortello
{
    public class FlatBoardInputSystem : IBoardInputSystem
    {
        private FlatBoardSettings settings;

        public FlatBoardInputSystem(FlatBoardSettings settings)
        {
            this.settings = settings;
        }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
        

        //TODO 
        public int GetTileHoveredID()
        {
            return -1;
        }

        public void Init()
        {

        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}