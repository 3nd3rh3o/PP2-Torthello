using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace Torthello
{
    public class TriangularBoardMatHandler : FlatBoardMaterialHandler
    {
        public TriangularBoardMatHandler(Settings settings) : base(settings)
        {
        }

        public override void UpdateMeshRenderer(MeshRenderer renderer)
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

        public override void InitMeshRenderer(MeshRenderer renderer)
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
    }
}