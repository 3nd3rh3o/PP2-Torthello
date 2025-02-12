using System.Collections.Generic;

namespace Tortello
{
    public class FlatBoardPawnProccessor : IPawnProccessor
    {
        public FlatBoardSettings settings;
        public Pawn[] pawns;

        public FlatBoardPawnProccessor(FlatBoardSettings settings)
        {
            this.settings = settings;
        }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }

        public void FlipAnimSeq(List<List<int>> pawnFlipped)
        {

        }

        public void Init()
        {
            pawns = new Pawn[settings.BoardHeight * settings.BoardWidth];
        }

        public void RemoveAllPawns()
        {
            throw new System.NotImplementedException();
        }

        public void SpawnPawn(int TileID, Couleur couleur)
        {
            switch (settings.PawnModel)
            {
                case PawnModel.Default:

                    break;
            }
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}