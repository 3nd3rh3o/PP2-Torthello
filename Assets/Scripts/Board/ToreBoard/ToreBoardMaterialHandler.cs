using UnityEngine;

namespace Tortello
{
    public class ToreBoardMaterialHandler : IMaterialHandler
    {

        private ToreBoardSettings settings;
        private int previousWidth;

        private int previousHeight;
        private int hoveredTile;
        private bool hoverChanged = false;
        private bool failedPlacementAnim = false;
        private int failedPlacementTileID = -1;
        private float animTime = 0f;

        private Material[] mats;
        public ToreBoardMaterialHandler(ToreBoardSettings settings)
        {
            this.settings = settings;
        }

        public void Destroy(MeshRenderer renderer)
        {
            mats = null;
            renderer.sharedMaterials = new Material[0];
        }

        public void FailedPlacement()
        {
            failedPlacementTileID = hoveredTile;
            animTime = 0;
            failedPlacementAnim = true;
        }

        public void InitMeshRenderer(MeshRenderer renderer)
        {
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;

            mats = new Material[settings.BoardHeight * settings.BoardWidth];

            for (int i = 0; i < settings.BoardHeight * settings.BoardWidth; i++)
            {
                mats[i] = settings.Tilematerial;
            }
            renderer.sharedMaterials = mats;
        }

        public void SetHoveredTile(int id)
        {
            if (hoveredTile != id) hoverChanged = true;
            hoveredTile = id;
        }

        public void UpdateMeshRenderer(MeshRenderer renderer)
        {
            if (!failedPlacementAnim && previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth && !hoverChanged) return;

            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;

            mats = new Material[settings.BoardHeight * settings.BoardWidth];

            for (int i = 0; i < settings.BoardHeight * settings.BoardWidth; i++)
            {
                mats[i] = settings.Tilematerial;
            }
            renderer.sharedMaterials = mats;
            Color hoveredColor = Color.white;
            
            for (int i = 0; i < settings.BoardHeight * settings.BoardWidth; i++)
            {
                MaterialPropertyBlock mpb = new();
                
                mpb.SetColor("_BaseColor", failedPlacementAnim && i == failedPlacementTileID ? RedBlinkColor(i == hoveredTile ? hoveredColor : settings.Tilematerial.color) : i == hoveredTile ? hoveredColor : settings.Tilematerial.color);
                renderer.SetPropertyBlock(mpb, i);
            }
            if (failedPlacementAnim) animTime += Time.deltaTime;
        }

        public Color RedBlinkColor(Color drawnColor)
        {
            if (animTime > 1f)
            {
                animTime = 0;
                failedPlacementAnim = false;
                return drawnColor;
            }
            else 
            {
                return animTime < 0.5f ? Color.Lerp(drawnColor, Color.red, animTime*2f) : Color.Lerp(Color.red, drawnColor, (animTime - 0.5f) * 2f);
            }
        }
    }
}