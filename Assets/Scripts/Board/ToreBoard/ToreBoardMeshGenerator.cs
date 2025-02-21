using UnityEngine;
using System.Linq;
using UnityEngine.SocialPlatforms.GameCenter;

namespace Tortello
{
    public class ToreBoardMeshGenerator : FlatBoardMeshGenerator
    {



        public ToreBoardMeshGenerator(FlatBoardSettings settings) : base(settings)
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

        public void UpdateMesh(MeshFilter meshFilter)
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

        public void Destroy(MeshFilter mF)
        {
#if UNITY_EDITOR
            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));
            MonoBehaviour.DestroyImmediate(mF.sharedMesh);
#else
            combines.ToList().ForEach(c => MonoBehaviour.Destroy(c.mesh));
            MonoBehaviour.Destroy(mF.sharedMesh);
#endif
            combines = null;

        }




        private void CreateToreMesh()
        {
            Vector3[] points = new Vector3[settings.BoardHeight*settings.BoardWidth];
            for(int i = 0 ; i < settings.BoardHeight ; i++){
                for(int j = 0 ; j < settings.BoardWidth ; j++){
                    points[i*settings.BoardWidth+j] = new Vector3();
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

        highLightMesh.RecalculateNormals();
        highLightMesh.RecalculateTangents();
        highLightMesh.RecalculateBounds();
        return highLightMesh;
    }

    }
}