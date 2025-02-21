using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Torthello
{
    public class TorBoardPawnProcessor : IPawnProccessor
    {
        private TorBoardSettings settings;
        private Pawn[] pawns;
        private Transform parent;
        private TorBoardMeshGenerator meshGenerator;

        public TorBoardPawnProcessor(Transform parent, TorBoardSettings settings, TorBoardMeshGenerator meshGenerator)
        {
            this.settings = settings;
            this.parent = parent;
            this.meshGenerator = meshGenerator;
        }

        public void Init()
        {
            pawns = new Pawn[settings.BoardSize * settings.BoardSize];
        }

        public void StartGame()
        {
            int u = Mathf.FloorToInt(settings.BoardSize / 2f) - 1;
            int v = Mathf.FloorToInt(settings.BoardSize / 2f) - 1;
            SpawnPawn(u + v * settings.BoardSize, Couleur.Noir);
            SpawnPawn(u + 1 + v * settings.BoardSize, Couleur.Blanc);
            SpawnPawn(u + (v + 1) * settings.BoardSize, Couleur.Blanc);
            SpawnPawn(u + 1 + (v + 1) * settings.BoardSize, Couleur.Noir);
        }

        public void SpawnPawn(int TileID, Couleur couleur)
        {
            if (parent == null)
            {
                Debug.LogError("Parent transform is null.");
                return;
            }

            StaticPawnImporter importer = parent.GetComponent<StaticPawnImporter>();
            if (importer == null)
            {
                Debug.LogError("StaticPawnImporter component is missing on the parent transform.");
                return;
            }

            switch (settings.PawnModel)
            {
                case PawnModel.Default:
                    DefaultPawn pawn = importer.SpawnDefaultPawn();
                    if (pawn == null)
                    {
                        Debug.LogError("Failed to spawn default pawn.");
                        return;
                    }

                    Vector3 position = TileIDToWP(TileID);
                    Quaternion rotation = GetTileRotation(TileID);

                    Debug.Log($"Spawning pawn at position: {position}, with rotation: {rotation}");

                    pawn.pos = position;
                    pawn.couleur = couleur;
                    pawn.rot = rotation;
                    pawn.StartSpawnAnim();
                    pawns[TileID] = pawn;
                    pawn.transform.parent = parent;

                    Debug.Log($"Spawned pawn position :{pawn.transform.position}, rotation: {pawn.transform.rotation}");
                    Debug.Log($"Spawned pawn pos :{pawn.pos}, rotation: {pawn.transform.localRotation}");
                    break;
            }
        }

        public void FlipAnimSeq(List<List<int>> pawnFlipped)
        {
            switch (settings.PawnModel)
            {
                case PawnModel.Default:
                    pawnFlipped.ForEach(l => l.ForEach(p => ((DefaultPawn)pawns[p]).StartFlipAnim()));
                    break;
            }
        }

        public void RemoveAllPawns()
        {
#if UNITY_EDITOR
            pawns.ToList().ForEach(p => { if (p != null) MonoBehaviour.DestroyImmediate(p.gameObject); });
            while (parent.childCount > 0)
            {
                MonoBehaviour.DestroyImmediate(parent.GetChild(0).gameObject);
            }
#else
            pawns.ToList().ForEach(p => { if (p != null) MonoBehaviour.Destroy(p.gameObject); });
#endif
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            pawns.ToList().ForEach(p => { if (p != null) MonoBehaviour.DestroyImmediate(p.gameObject); });
            while (parent.childCount > 0)
            {
                MonoBehaviour.DestroyImmediate(parent.GetChild(0).gameObject);
            }
#else
            pawns.ToList().ForEach(p => { if (p != null) MonoBehaviour.Destroy(p.gameObject); });
#endif
        }

        public void Update()
        {

        }

        private Vector3 TileIDToWP(int TileID)
        {
            int u = TileID % settings.BoardSize;
            int v = TileID / settings.BoardSize;
            return meshGenerator.GetTileCenter(u, v);
        }

        private Quaternion GetTileRotation(int TileID)
        {
            int u = TileID % settings.BoardSize;
            int v = TileID / settings.BoardSize;
            Vector3[] corners = meshGenerator.GetTileCorners(u, v);

            if (corners.Length < 3)
            {
                Debug.LogError("Not enough corners to calculate rotation.");
                return Quaternion.identity;
            }

            // Calculer la normale de la surface
            Vector3 forward = (corners[1] - corners[0]).normalized;
            Vector3 right = (corners[3] - corners[0]).normalized;
            Vector3 up = Vector3.Cross(forward, right).normalized;

            return Quaternion.LookRotation(forward, up);
        }
    }
}