using UnityEngine;

namespace Torthello
{
    public class TriangularBoardPawnProcessor : FlatBoardPawnProccessor
    {
        public TriangularBoardPawnProcessor(Transform parent, Settings settings) : base(parent, settings)
        {
        }
        protected override Vector3 TileIDToWP(int TileID)
        {
            int v = TileID / settings.BoardWidth;
            int u = TileID % settings.BoardWidth;

            Vector3 U = new(2f * Mathf.Sqrt(Mathf.Pow(settings.sideLength * 0.5f, 2) - Mathf.Pow(settings.sideLength * 0.25f, 2)), 0f, 0f);
            Vector3 V = Quaternion.Euler(0f, 120f, 0f) * U;
            Vector3 oU = settings.BoardWidth * -0.5f * U;
            Vector3 oV = settings.BoardHeight * -0.5f * V;

            return oU + oV + (u * U) + (v * V);
        }
    }
}
