using System.ComponentModel;
using UnityEngine;

namespace Torthello
{
    public class ToreBoardPawnProcessor : FlatBoardPawnProccessor
    {
        public ToreBoardPawnProcessor(Transform parent, Settings settings) : base(parent, settings)
        {
        }

        public static Vector3 IndexToPos(int i, int j, int maxI, int maxJ, float radius, float sectionRadius)
        {
            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;
            Vector3 section = Quaternion.Euler(new(0, 360f / maxI * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * sectionCenter;
            Vector3 p0 = section + Quaternion.Euler(new(0, 360f / maxI * i, 0)) * Quaternion.Euler(new(360f / maxJ * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, 360f / maxI * i, 0)) * Quaternion.Euler(new(360f / maxJ * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * Quaternion.Euler(new(360f / maxJ * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * Quaternion.Euler(new(360f / maxJ * j, 0, 0)) * subSectionVector;
            return (p0 + p1 + p2 + p3) * 0.25f;
        }

        protected override Vector3 TileIDToWP(int TileID){
            int i = Mathf.FloorToInt(TileID/settings.BoardWidth);
            int j = TileID%settings.BoardWidth;

            float subradius = 1.5f * settings.BoardWidth / (2f * Mathf.PI);
            float radius = (1.5f * settings.BoardHeight / (2f * Mathf.PI)) + subradius;

            return IndexToPos(i,j,settings.BoardHeight,settings.BoardWidth,radius,subradius);
        }

        protected override Quaternion TileIDToNormal(int TileID)
        {
            float subradius = 1.5f * settings.BoardHeight / (2f * Mathf.PI);
            float radius = (1.5f * settings.BoardWidth / (2f * Mathf.PI)) + subradius;

            int i = Mathf.FloorToInt(TileID/settings.BoardWidth);

            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * subradius;

            Vector3 section = Quaternion.Euler(new(0, 360f / settings.BoardHeight * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, 360f / settings.BoardHeight * (i + 1), 0)) * sectionCenter;

            Vector3 sectionMed = (section + nextSection)*0.5f;
            Vector3 PawnUpDirection = (TileIDToWP(TileID)-sectionMed).normalized;
            return Quaternion.FromToRotation(new (0, 1, 0), PawnUpDirection);
        }
    }
}