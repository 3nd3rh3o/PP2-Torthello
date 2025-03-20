using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace Torthello
{
    public class TriangularSimpleBoardMatHandler : TriangularBoardMatHandler
    {
        public TriangularSimpleBoardMatHandler(Settings settings) : base(settings)
        {
        }

        public override void UpdateMeshRenderer(MeshRenderer renderer)
        {
            if (!failedPlacementAnim && previousHeight == settings.BoardHeight && previousWidth == settings.BoardWidth && !hoverChanged) return;

            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;

            mats = new Material[settings.BoardHeight*(settings.BoardHeight+1)/2];

            for (int i = 0; i < settings.BoardHeight*(settings.BoardHeight+1)/2; i++)
            {
                mats[i] = settings.HexTilematerial;
            }
            renderer.sharedMaterials = mats;
            Color hoveredColor = Color.white;
            
            for (int i = 0; i < settings.BoardHeight*(settings.BoardHeight+1)/2; i++)
            {
                MaterialPropertyBlock mpb = new();
                
                mpb.SetColor("_BaseColor", failedPlacementAnim && i == failedPlacementTileID ? RedBlinkColor(i == hoveredTile ? hoveredColor : settings.HexTilematerial.color) : i == hoveredTile ? hoveredColor : settings.HexTilematerial.color);
                renderer.SetPropertyBlock(mpb, i);
            }
            if (failedPlacementAnim) animTime += Time.deltaTime;
        }

        public override void InitMeshRenderer(MeshRenderer renderer)
        {
            previousHeight = settings.BoardHeight;
            previousWidth = settings.BoardWidth;

            mats = new Material[settings.BoardHeight*(settings.BoardHeight+1)/2];

            for (int i = 0; i < settings.BoardHeight*(settings.BoardHeight+1)/2; i++)
            {
                mats[i] = settings.HexTilematerial;
            }
            renderer.sharedMaterials = mats;
        }
    }
}