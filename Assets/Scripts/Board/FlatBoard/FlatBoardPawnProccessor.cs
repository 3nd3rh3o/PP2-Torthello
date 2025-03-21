using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Torthello
{
    public class FlatBoardPawnProccessor : IPawnProccessor
    {
        protected Settings settings;
        protected Pawn[] pawns;
        protected Transform parent;
        protected int previousWidth;
        protected int previousHeight;

        public FlatBoardPawnProccessor(Transform parent, Settings settings)
        {
            this.settings = settings;
            this.parent = parent;
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;
        }

        public virtual void Destroy()
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

        public void FlipAnimSeq(List<List<int>> pawnFlipped)
        {
            switch (settings.PawnModel)
            {
                case PawnModel.Default:
                    pawnFlipped.ForEach(l => l.ForEach(p => ((DefaultPawn)pawns[p]).StartFlipAnim()));
                    break;
            }
        }

        public virtual void Init()
        {
            pawns = new Pawn[settings.BoardHeight * settings.BoardWidth];
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

        public virtual void SpawnPawn(int TileID, Couleur couleur)
        {
            switch (settings.PawnModel)
            {
                case PawnModel.Default:
                    DefaultPawn pawn = parent.GetComponent<StaticPawnImporter>().SpawnDefaultPawn();
                    pawn.pos = TileIDToWP(TileID);
                    pawn.rot = TileIDToNormal(TileID);
                    pawn.couleur = couleur;
                    pawn.StartSpawnAnim();
                    pawns[TileID] = pawn;
                    pawn.transform.parent = parent;
                    break;
            }
        }

        public virtual void StartGame()
        {
            int u = Mathf.FloorToInt(settings.BoardWidth / 2f) - 1;
            int v = Mathf.FloorToInt(settings.BoardHeight / 2f) - 1;
            SpawnPawn(u + v * settings.BoardWidth, Couleur.Noir);
            SpawnPawn(u + 1 + v * settings.BoardWidth, Couleur.Blanc);
            SpawnPawn(u + (v + 1) * settings.BoardWidth, Couleur.Blanc);
            SpawnPawn(u + 1 + (v + 1) * settings.BoardWidth, Couleur.Noir);
        }

        public virtual void Update()
        {
            if (previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth) return;
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;
            Destroy();
            Init();
        }

        protected virtual Vector3 TileIDToWP(int TileID)
        {
            float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);
            int v = Mathf.FloorToInt(TileID / settings.BoardWidth);
            int u = TileID - (v * settings.BoardWidth);
            return offset + new Vector3(u * settings.sideLength, 0f, v * settings.sideLength);
        }

        protected virtual Quaternion TileIDToNormal(int TileID)
        {
            Vector3 PawnUpDirection = new(0, 1, 0);
            return Quaternion.FromToRotation(new (0, 1, 0), PawnUpDirection);
        }
    }
}