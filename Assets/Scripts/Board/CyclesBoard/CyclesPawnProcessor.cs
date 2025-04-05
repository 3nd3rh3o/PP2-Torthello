using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    public class CyclesPawnProcessor : IPawnProccessor
    {
        private int[] boardState;
        protected Settings settings;
        private int prevWidth;
        private int prevHeight;

        public CyclesPawnProcessor(Transform transform, Settings settings)
        {
            this.settings = settings;
            boardState = new int[settings.BoardHeight];
        }

        public void Destroy()
        {

        }

        public void FlipAnimSeq(List<List<int>> pawnFlipped)
        {

        }

        public void Init()
        {
            prevHeight = settings.BoardHeight;
            prevWidth = settings.BoardWidth;
            boardState = new int[settings.BoardHeight];
            for (int i = 0; i < settings.BoardHeight; i++)
            {
                boardState[i] = 0;
            }
        }

        public void RemoveAllPawns()
        {
            for (int i = 0; i < settings.BoardHeight; i++)
            {
                boardState[i] = 0;
            }
        }

        public void SpawnPawn(int TileID, Couleur couleur)
        {
            boardState[TileID] = couleur == Couleur.Noir ? 1 : 2;
        }

        public void StartGame()
        {
            boardState[0] = 1;
            boardState[1] = 2;
            boardState[2] = 1;
            boardState[3] = 2;
            
        }

        public void Update()
        {
            if (settings.BoardHeight != prevHeight || settings.BoardWidth != prevWidth)
            {
                prevHeight = settings.BoardHeight;
                prevWidth = settings.BoardWidth;
                boardState = new int[settings.BoardHeight];
                for (int i = 0; i < settings.BoardHeight; i++)
                {
                    boardState[i] = 0;
                }
            }
            settings.nodes.SetNodes(boardState);

        }
    }
} 