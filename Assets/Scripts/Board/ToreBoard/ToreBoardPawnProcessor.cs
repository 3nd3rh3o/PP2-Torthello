using UnityEngine;
using System.Collections.Generic;

namespace Torthello
{
    public class ToreBoardPawnProcessor : FlatBoardPawnProccessor
    {

        public ToreBoardPawnProcessor(Transform parent, Settings settings) : base(parent, settings)
        {
        }

        public override void Update()
        {
            // TODO si rotationAnim == true => Reposition and increase lerp factor by Time.deltaTime
            if (previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth && !(settings.rotAnimD || settings.rotAnimU)) return;
            if (settings.rotAnimU || settings.rotAnimD)
            {
                RepositionPawns();
                return;
            }
            //Debug.Log("pawnProcessorUpdate");
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;
            Destroy();
            Init();
        }

        //valable pour toutes les fontctions ID_TO:
        //il faudrait trouver un moyen récupérer ces valeurs sans les recalculer avec le meshgenerator pour ne pas avoir à l'adpater pour chaque mesh différent
        public static Vector3 IndexToPos(int i, int j, int maxI, int maxJ, float radius, float sectionRadius, Settings settings)
        {
            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;

            // Calcul de la position de la section sur le grand cercle
            Vector3 section = Quaternion.Euler(new(0, (360f / maxI) * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, (360f / maxI) * (i + 1), 0)) * sectionCenter;

            // Appliquer l'offset pour faire rouler les cases sur le petit cercle
            float rotStep = 10f;
            Quaternion minorCircleRotation = Quaternion.Euler(new(settings.rotAnimD ? Mathf.Lerp(settings.rotationOffset + rotStep, settings.rotationOffset, settings.rotAnimT) : settings.rotAnimU? Mathf.Lerp(settings.rotationOffset - rotStep, settings.rotationOffset, settings.rotAnimT) : settings.rotationOffset, 0, 0));

            Vector3 p0 = section + Quaternion.Euler(new(0, (360f / maxI) * i, 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, (360f / maxI) * i, 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, (360f / maxI) * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, (360f / maxI) * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new((360f / maxJ) * j, 0, 0)) * subSectionVector;

            return (p0 + p1 + p2 + p3) * 0.25f;
        }

        protected override Vector3 TileIDToWP(int TileID)
        {
            int i = Mathf.FloorToInt(TileID / settings.BoardWidth);
            int j = TileID % settings.BoardWidth;

            float subradius = 1.5f * settings.BoardWidth / (2f * Mathf.PI);
            float radius = (1.5f * settings.BoardHeight / (2f * Mathf.PI)) + subradius;

            // Inclure rotationOffset dans le calcul
            return IndexToPos(i, j, settings.BoardHeight, settings.BoardWidth, radius, subradius, settings);
        }


        protected override Quaternion TileIDToNormal(int TileID)
        {
            float subradius = 1.5f * settings.BoardHeight / (2f * Mathf.PI);
            float radius = (1.5f * settings.BoardWidth / (2f * Mathf.PI)) + subradius;

            int i = Mathf.FloorToInt(TileID / settings.BoardWidth);
            int j = TileID - (i * settings.BoardWidth);


            Vector3[] corn = IndexToTileCorners(i, j, settings.BoardHeight, settings.BoardWidth, radius, subradius, settings);

            Vector3 A = Vector3.Cross(corn[1] - corn[0], corn[3] - corn[0]);
            Vector3 B = Vector3.Cross(corn[2] - corn[1], corn[0] - corn[1]);
            Vector3 C = Vector3.Cross(corn[3] - corn[2], corn[1] - corn[2]);
            Vector3 D = Vector3.Cross(corn[0] - corn[3] , corn[2] - corn[3]);
            
            Vector3 PawnUpDirection = ((A + B + C + D) * .25f).normalized;
            return Quaternion.FromToRotation(new(0, 1, 0), PawnUpDirection);
        }

        //test pour comparer avec la destruction des pions
        public void RepositionPawns()
        {
            //Debug.Log("RepositionPawns");
            for (int tileID = 0; tileID < settings.BoardHeight * settings.BoardWidth; tileID++)
            {
                Pawn pawn = pawns[tileID];

                // Vérifier si la case contient un pion valide
                if (pawn == null)
                {
                    continue;
                }

                // Recalculer la position et l'orientation du pion
                Vector3 newPosition = TileIDToWP(tileID);
                Quaternion newRotation = TileIDToNormal(tileID);
                //Debug.Log("newPosition: " + newPosition + " newRotation: " + newRotation);
                // Mettre à jour les propriétés du pion et appliquer au transform
                pawn.pos = newPosition;
                pawn.rot = newRotation;
            }
        }


        public static Vector3[] IndexToTileCorners(int i, int j, int maxI, int maxJ, float radius, float sectionRadius, Settings settings)
        {
            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
            Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;
            Vector3 section = Quaternion.Euler(new(0, 360f / maxI * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * sectionCenter;

            float rotStep = 10f;
            Quaternion minorCircleRotation = Quaternion.Euler(new(settings.rotAnimD ? Mathf.Lerp(settings.rotationOffset + rotStep, settings.rotationOffset, settings.rotAnimT) : settings.rotAnimU? Mathf.Lerp(settings.rotationOffset - rotStep, settings.rotationOffset, settings.rotAnimT) : settings.rotationOffset, 0, 0));


            Vector3 p0 = section + Quaternion.Euler(new(0, 360f / maxI * i, 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, 360f / maxI * i, 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, 360f / maxI * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new(360f / maxJ * j, 0, 0)) * subSectionVector;

            return new Vector3[] { p0, p1, p2, p3 };
        }
    }
}