using UnityEngine;

namespace Torthello
{
    public class FlatBoardMaterialHandler : IMaterialHandler
    {

        protected Settings settings;
        protected int previousWidth;
        protected int previousHeight;
        protected int hoveredTile = -1;
        protected bool hoverChanged = false;
        protected bool failedPlacementAnim = false;
        protected int failedPlacementTileID = -1;
        protected float animTime = 0f;

        protected Material[] mats;
        public FlatBoardMaterialHandler(Settings settings)
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

        public virtual void InitMeshRenderer(MeshRenderer renderer)
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

        public virtual void UpdateMeshRenderer(MeshRenderer renderer)
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