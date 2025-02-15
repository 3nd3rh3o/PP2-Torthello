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

        public TorBoardPawnProcessor(Transform parent, TorBoardSettings settings)
        {
            this.settings = settings;
            this.parent = parent;
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
                    pawn.pos = TileIDToWP(TileID);
                    pawn.couleur = couleur;
                    pawn.StartSpawnAnim();
                    pawns[TileID] = pawn;
                    pawn.transform.parent = parent;
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
            float offsetX = (-settings.SideLength * settings.BoardSize + settings.SideLength) * 0.5f;
            float offsetZ = (-settings.SideLength * settings.BoardSize + settings.SideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);
            int v = Mathf.FloorToInt(TileID / settings.BoardSize);
            int u = TileID - (v * settings.BoardSize);
            u = (u + settings.BoardSize) % settings.BoardSize;
            v = (v + settings.BoardSize) % settings.BoardSize;
            return offset + new Vector3(u * settings.SideLength, 0f, v * settings.SideLength);
        }
    }
}