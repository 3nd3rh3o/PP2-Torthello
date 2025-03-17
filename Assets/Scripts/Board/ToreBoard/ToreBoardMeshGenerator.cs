using UnityEngine;
using System.Linq;
using UnityEngine.CrashReportHandler;

namespace Torthello
{
    public class ToreBoardMeshGenerator : FlatBoardMeshGenerator
    {
        public ToreBoardMeshGenerator(Settings settings) : base(settings)
        {
        }

        public override void InitMesh(MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new();
            }
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();

            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;
            PreviousLength = settings.sideLength;

            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];

            CreateToreMesh();

            mesh.CombineMeshes(combines, false, false, false);


        }

        public override void UpdateMesh(MeshFilter meshFilter)
        {
            //on teste si les parametres in changes
            if (PreviousHeight == settings.BoardHeight && PreviousWidth == settings.BoardWidth && PreviousLength == settings.sideLength && !(settings.rotAnimD || settings.rotAnimU)) return;
            
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();

            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;
            PreviousLength = settings.sideLength;

            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));

            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];

            CreateToreMesh();

            mesh.CombineMeshes(combines, false, false, false);
        }





        private void CreateToreMesh()
        {
            float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            Vector3 offsetPosition = new(offsetX, 0f, offsetZ);

            for (int i = 0; i < settings.BoardHeight; i++)
            {
                for (int j = 0; j < settings.BoardWidth; j++)
                {
                    Vector3 c = offsetPosition + new Vector3(j * settings.sideLength, 0f, i * settings.sideLength);
                    Mesh mesh = new();
                    float subradius = 1.5f * settings.BoardWidth / (2f * Mathf.PI);
                    float radius = (1.5f * settings.BoardHeight / (2f * Mathf.PI)) + subradius;
                    // Appliquer l'offset de rotation
                    GenMeshOfTileByIndex(mesh, i, j, radius, subradius, settings.BoardHeight, settings.BoardWidth, settings);
                    combines[i * settings.BoardWidth + j].mesh = mesh;
                }
            }
        }



        public static Mesh GenMeshOfTileByIndex(Mesh highLightMesh, int i, int j, float radius, float sectionRadius, int numberOfSection, int pointsPerSection, Settings settings)
        {
            Vector3[] points = new Vector3[4];
            int[] triangles = new int[6];

            // Position du centre de la section sur le grand cercle
            Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;

            // Vecteur représentant le petit cercle
            Vector3 subSectionVector = new Vector3(0, 0, 1) * sectionRadius;

            // Calcul de la position de la section sur le grand cercle
            Vector3 section = Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * sectionCenter;

            // Appliquer l'offset pour faire rouler les cases sur le petit cercle
            float rotStep = 10f;
            Quaternion minorCircleRotation = Quaternion.Euler(new(settings.rotAnimD ? Mathf.Lerp(settings.rotationOffset + rotStep, settings.rotationOffset, settings.rotAnimT) : settings.rotAnimU? Mathf.Lerp(settings.rotationOffset - rotStep, settings.rotationOffset, settings.rotAnimT) : settings.rotationOffset, 0, 0));

            Vector3 p0 = section + Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * minorCircleRotation * Quaternion.Euler(new((360f / pointsPerSection) * j, 0, 0)) * subSectionVector;
            Vector3 p1 = section + Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * minorCircleRotation * Quaternion.Euler(new((360f / pointsPerSection) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p2 = nextSection + Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new((360f / pointsPerSection) * (j + 1), 0, 0)) * subSectionVector;
            Vector3 p3 = nextSection + Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * minorCircleRotation * Quaternion.Euler(new((360f / pointsPerSection) * j, 0, 0)) * subSectionVector;

            // Définir les sommets et les triangles
            points[0] = p0;
            points[1] = p1;
            points[2] = p2;
            points[3] = p3;

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 3;
            triangles[3] = 1;
            triangles[4] = 2;
            triangles[5] = 3;

            // Appliquer les données au mesh
            highLightMesh.vertices = points;
            highLightMesh.triangles = triangles;
            highLightMesh.uv = new Vector2[] { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };

            highLightMesh.RecalculateNormals();
            highLightMesh.RecalculateTangents();
            highLightMesh.RecalculateBounds();
            return highLightMesh;
        }
    }
}