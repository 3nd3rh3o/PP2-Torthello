using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tortello
{
    public class FlatBoardPawnProccessor : IPawnProccessor
    {
        private FlatBoardSettings settings;
        private Pawn[] pawns;
        private Transform parent;

        public FlatBoardPawnProccessor(Transform parent, FlatBoardSettings settings)
        {
            this.settings = settings;
            this.parent = parent;
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            pawns.ToList().ForEach(p => {if (p != null ) MonoBehaviour.DestroyImmediate(p.gameObject);});
#else
            pawns.ToList().ForEach(p => MonoBehaviour.Destroy(p.gameObject));
#endif
            while (parent.childCount > 0)
            {
                MonoBehaviour.DestroyImmediate(parent.GetChild(0).gameObject);
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

        public void Init()
        {
            pawns = new Pawn[settings.BoardHeight * settings.BoardWidth];
        }

        public void RemoveAllPawns()
        {

        }

        public void SpawnPawn(int TileID, Couleur couleur)
        {
            switch (settings.PawnModel)
            {
                case PawnModel.Default:
                    DefaultPawn pawn = parent.GetComponent<StaticPawnImporter>().SpawnDefaultPawn();
                    pawn.pos = TileIDToWP(TileID);
                    pawn.couleur = couleur;
                    pawn.StartSpawnAnim();
                    pawn.transform.parent = parent;
                    break;
            }
        }

        public void StartGame()
        {
            int u = Mathf.FloorToInt(settings.BoardWidth / 2f);
            int v = Mathf.FloorToInt(settings.BoardHeight / 2f);
            SpawnPawn(u + v * settings.BoardWidth, Couleur.Noir);
            SpawnPawn(u + 1 + v * settings.BoardWidth, Couleur.Blanc);
            SpawnPawn(u + (v + 1) * settings.BoardWidth, Couleur.Blanc);
            SpawnPawn(u + 1 + (v + 1) * settings.BoardWidth, Couleur.Noir);
        }

        public void Update()
        {

        }

        private Vector3 TileIDToWP(int TileID)
        {
            float offsetX = (-settings.sideLength * settings.BoardWidth + settings.sideLength) * 0.5f;
            float offsetZ = (-settings.sideLength * settings.BoardHeight + settings.sideLength) * 0.5f;
            Vector3 offset = new(offsetX, 0f, offsetZ);
            int v = Mathf.FloorToInt(TileID / settings.BoardWidth);
            int u = TileID - (v * settings.BoardWidth);
            return offset + new Vector3(u * settings.sideLength, 0f, v * settings.sideLength);
        }
    }
}