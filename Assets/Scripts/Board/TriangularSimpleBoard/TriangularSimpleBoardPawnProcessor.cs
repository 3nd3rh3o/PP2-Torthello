using UnityEngine;
using UnityEngine.AI;

namespace Torthello
{
    public class TriangularSimpleBoardPawnProcessor : TriangularBoardPawnProcessor
    {
        public TriangularSimpleBoardPawnProcessor(Transform parent, Settings settings) : base(parent, settings)
        {
        }

        public override void Init()
        {
            pawns = new Pawn[settings.BoardHeight*(settings.BoardHeight+1)/2];
        }

        protected override Vector3 TileIDToWP(int TileID)
        {
            int v = Rank(TileID, settings.BoardHeight);
            int u = TileID - (v * (v+1) / 2);

            Vector3 U = new(2f * Mathf.Sqrt(Mathf.Pow(settings.sideLength * 0.5f, 2) - Mathf.Pow(settings.sideLength * 0.25f, 2)), 0f, 0f);
            Vector3 V = Quaternion.Euler(0f, 120f, 0f) * U;
            Vector3 vO = (V * (settings.BoardHeight - 1));
            Vector3 uO = vO + (U * (settings.BoardHeight - 1));
            Vector3 o = (Vector3.zero  + vO + uO) / 3f;

            return (- o + (u * U) + (v * V));
        }

        public override void StartGame()
        {
            int r0 = settings.BoardHeight / 2;
            int r1 = settings.BoardHeight / 2 - 1;
            int c0 = r0 * (r0 + 1) / 2 + (r0 / 2);
            int c1 = c0 + 1;
            int c2 = r1 * (r1 + 1) / 2 + (r1 / 2);
            int c3 = c2 + 1;
            SpawnPawn(c0, Couleur.Blanc);
            SpawnPawn(c1, Couleur.Noir);
            SpawnPawn(c2, Couleur.Noir);
            SpawnPawn(c3, Couleur.Blanc);
        }

        private int Rank(int v, int V)
        {
            for (int i = 0; i < V; i++)
            {
                if (v - i < 0)
                {
                    return i - 1;
                }
              v-=i;
            }
            return V-1;
        }

    }
}
