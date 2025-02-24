using UnityEngine;
using System.Linq;
using UnityEngine.SocialPlatforms.GameCenter;

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
            if (PreviousHeight == settings.BoardHeight && PreviousWidth == settings.BoardWidth && PreviousLength == settings.sideLength) return;

            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();

            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;
            PreviousLength = settings.sideLength;

#if UNITY_EDITOR
            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));
#else
            combines.ToList().ForEach(c => MonoBehaviour.Destroy(c.mesh));
#endif

            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];

            CreateToreMesh();

            mesh.CombineMeshes(combines, false, false, false);
        }





        private void CreateToreMesh()
        {
            //Calcul du centre de la premiere case
            float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);

            //generation des mesh en fonction de l'offset
            for (int i = 0; i < settings.BoardHeight; i++)
            {
                for (int j = 0; j < settings.BoardWidth; j++)
                {
                    Vector3 c = offset + new Vector3(j * settings.sideLength, 0f, i * settings.sideLength);
                    Mesh mesh = new();
                    float subradius = 1.5f*settings.BoardWidth/(2f*Mathf.PI);
                    float radius = (1.5f*settings.BoardHeight/(2f*Mathf.PI))+subradius;
                    GenMeshOfTileByIndex(mesh, i,j,radius,subradius,settings.BoardHeight,settings.BoardWidth);
                    combines[i * settings.BoardWidth + j].mesh = mesh;
                }
            }
        }



        public static Mesh GenMeshOfTileByIndex(Mesh highLightMesh, int i, int j, float radius, float sectionRadius, int numberOfSection, int pointsPerSection)
    {
        Vector3[] points = new Vector3[4];
        int[] triangles = new int[6];


        Vector3 sectionCenter = new Vector3(0, 0, 1) * radius;
        Vector3 subSectionVector = new Vector3(0, 0, 1) * (sectionRadius + 0.0001f);

        Vector3 section = Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * sectionCenter;
            Vector3 nextSection = Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * sectionCenter;

        Vector3 p0 = section + Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * Quaternion.Euler(new((360f / pointsPerSection) * j, 0, 0)) * subSectionVector;
        Vector3 p1 = section + Quaternion.Euler(new(0, (360f / numberOfSection) * i, 0)) * Quaternion.Euler(new((360f / pointsPerSection) * (j + 1), 0, 0)) * subSectionVector;
        Vector3 p2 = nextSection + Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new((360f / pointsPerSection) * (j + 1), 0, 0)) * subSectionVector;
        Vector3 p3 = nextSection + Quaternion.Euler(new(0, (360f / numberOfSection) * (i + 1), 0)) * Quaternion.Euler(new((360f / pointsPerSection) * j, 0, 0)) * subSectionVector;

        int p0i = 0;
        int p1i = 1;
        int p2i = 2;
        int p3i = 3;

        points[p0i] = p0;
        points[p1i] = p1;
        points[p2i] = p2;
        points[p3i] = p3;

        triangles[0] = p0i;
        triangles[1] = p1i;
        triangles[2] = p3i;
        triangles[3] = p1i;
        triangles[4] = p2i;
        triangles[5] = p3i;

        highLightMesh.Clear();
        highLightMesh.vertices = points;
        highLightMesh.triangles = triangles;
        highLightMesh.uv = new Vector2[]{new(0,0),new(0,1),new(1,1),new(1,0)};

        highLightMesh.RecalculateNormals();
        highLightMesh.RecalculateTangents();
        highLightMesh.RecalculateBounds();
        return highLightMesh;
    }

    }
}