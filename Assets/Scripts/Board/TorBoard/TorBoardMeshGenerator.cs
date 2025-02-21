using UnityEngine;
using System.Linq;

namespace Torthello {
    public class TorBoardMeshGenerator : IMeshGenerator
    {
        private Settings settings;
        private int previousWidth;
        private int previousHeight;
        private float previousSideLength;
        private CombineInstance[] combines;
        private Vector3[][] tileCorners;
        private Vector3[] tileCenters;

        public TorBoardMeshGenerator(Settings settings)
        {
            this.settings = settings;
        }

        // Initialise le maillage du plateau
        public void InitMesh(MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new Mesh();
            }
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;
            previousSideLength = settings.sideLength;
            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];
            tileCorners = new Vector3[settings.BoardHeight * settings.BoardWidth][];
            tileCenters = new Vector3[settings.BoardHeight * settings.BoardWidth];
            CreateBoardMesh();
            mesh.CombineMeshes(combines, false, false, false);

            // Appliquer le matériau
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = settings.Tilematerial;
            }
        }

        // Met à jour le maillage du plateau si les paramètres ont changé
        public void UpdateMesh(MeshFilter meshFilter)
        {
            if (previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth && previousSideLength == settings.sideLength) return;
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;
            previousSideLength = settings.sideLength;
            combines = new CombineInstance[settings.BoardHeight * settings.BoardWidth];
            tileCorners = new Vector3[settings.BoardHeight * settings.BoardWidth][];
            tileCenters = new Vector3[settings.BoardHeight * settings.BoardWidth];
            CreateBoardMesh();
            mesh.CombineMeshes(combines, false, false, false);
        }

        // Détruit le maillage du plateau
        public void Destroy(MeshFilter meshFilter)
        {
    #if UNITY_EDITOR
            combines.ToList().ForEach(c => MonoBehaviour.DestroyImmediate(c.mesh));
            MonoBehaviour.DestroyImmediate(meshFilter.sharedMesh);
    #else
            combines.ToList().ForEach(c => MonoBehaviour.Destroy(c.mesh));
            MonoBehaviour.Destroy(meshFilter.sharedMesh);
    #endif
            combines = null;
        }

        // Crée le maillage du plateau
        private void CreateBoardMesh()
        {
            int boardSize = settings.BoardWidth * settings.BoardHeight;

            // Calculer majorRadius et minorRadius en fonction de boardSize
            float majorRadius = boardSize / (Mathf.PI);
            float minorRadius = majorRadius / 2;

            for (int i = 0; i < boardSize; i++)
            {
            for (int j = 0; j < boardSize; j++)
            {
                float u = (float)i / boardSize * 2 * Mathf.PI; // Angle autour du tore
                float v = (float)j / boardSize * 2 * Mathf.PI; // Angle le long du tore

                Vector3 center = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v)) * Mathf.Cos(u), 
                minorRadius * Mathf.Sin(v), 
                (majorRadius + minorRadius * Mathf.Cos(v)) * Mathf.Sin(u)
                );

                tileCenters[i * boardSize + j] = center;

                Mesh mesh = new Mesh();
                CreateTrapezoidMesh(center, u, v, majorRadius, minorRadius, boardSize, mesh);
                combines[i * boardSize + j].mesh = mesh;
                combines[i * boardSize + j].transform = Matrix4x4.identity;

                // Ajouter les coins du trapèze à tileCorners
                tileCorners[i * boardSize + j] = mesh.vertices;
            }
            }
        }

        // Crée un maillage de trapèze pour une tuile
        private static void CreateTrapezoidMesh(Vector3 center, float u, float v, float majorRadius, float minorRadius, int boardSize, Mesh mesh)
        {
            Vector3[] points = new Vector3[4];

            // Définir les angles de rotation pour chaque sommet du trapèze en fonction de la taille du plateau
            float angleStepU = 2 * Mathf.PI / boardSize;
            float angleStepV = 2 * Mathf.PI / boardSize;

            // Calculer les sommets du trapèze en tenant compte de la courbure du tore
            points[0] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Cos(u - angleStepU / 2),
                minorRadius * Mathf.Sin(v - angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Sin(u - angleStepU / 2)
            );
            /*Calcul de la composante X: ^^au dessus^^

                v - angleStepV / 2 :                            Ajuste l'angle v pour le calcul de la position sur le petit cercle (section transversale du tore).
                Mathf.Cos(v - angleStepV / 2) :                 Calcule le cosinus de cet angle ajusté.
                minorRadius * Mathf.Cos(v - angleStepV / 2) :   Multiplie le cosinus par le rayon mineur pour obtenir la position sur le petit cercle.
                majorRadius + ... :                             Ajoute le rayon majeur pour décaler cette position par rapport au centre du tore.
                Mathf.Cos(u - angleStepU / 2) :                 Calcule le cosinus de l'angle u ajusté pour la position sur le grand cercle (le tore lui-même).
                ... * Mathf.Cos(u - angleStepU / 2) :            Multiplie par ce cosinus pour obtenir la composante X finale.
            */
            points[1] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Cos(u - angleStepU / 2),
                minorRadius * Mathf.Sin(v + angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Sin(u - angleStepU / 2)
            );

            points[2] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Cos(u + angleStepU / 2),
                minorRadius * Mathf.Sin(v + angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v + angleStepV / 2)) * Mathf.Sin(u + angleStepU / 2)
            );

            points[3] = new Vector3(
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Cos(u + angleStepU / 2),
                minorRadius * Mathf.Sin(v - angleStepV / 2),
                (majorRadius + minorRadius * Mathf.Cos(v - angleStepV / 2)) * Mathf.Sin(u + angleStepU / 2)
            );

            int[] f = new int[] { 0, 1, 2, 2, 3, 0 }; // Définir les faces du trapèze dans le sens horaire
            Vector2[] uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            mesh.vertices = points;
            mesh.triangles = f;
            mesh.uv = uv; // Définir les coordonnées de texture pour chaque sommet, m'en sers pas pour le moment mais le tuto l'avait et chatgpt dit que c'est bien si jamais on veut texturer plus tard 
            mesh.RecalculateNormals(); // Recalculer les normales pour l'éclairage
            mesh.RecalculateTangents(); // Recalculer les tangentes pour les effets de surface
            mesh.RecalculateBounds(); // Recalculer les limites pour le rendu
            // commentaires généré par chatgpt au dessus, recalculateNormals suffit pour avoit un rendu
        }

        // Récupère les coins d'une tuile
        public Vector3[] GetTileCorners(int u, int v)
        {
            return tileCorners[v * settings.BoardWidth + u];
        }

        // Récupère le centre d'une tuile
        public Vector3 GetTileCenter(int u, int v)
        {
            return tileCenters[v * settings.BoardWidth + u];
        }
    }
}