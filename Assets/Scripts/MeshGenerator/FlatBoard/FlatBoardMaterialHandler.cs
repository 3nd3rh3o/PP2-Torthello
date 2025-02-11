using System.ComponentModel;
using UnityEngine;

namespace Tortello
{
    public class FlatBoardMaterialHandler : IMaterialHandler
    {

        private FlatBoardSettings settings;
        private int PreviousWidth;

        private int PreviousHeight;

        private Material[] mats;
        public FlatBoardMaterialHandler(FlatBoardSettings settings){
            this.settings = settings;
        }

        public void Destroy(MeshRenderer renderer)
        {
            mats=null;
            renderer.sharedMaterials=new Material[0];
        }

        public void InitMeshRenderer(MeshRenderer renderer)
        {
            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;

            mats = new Material[settings.BoardHeight*settings.BoardWidth];

            for (int i = 0 ; i < settings.BoardHeight*settings.BoardWidth ; i++)
            {
                mats[i]=settings.Tilematerial;
            }
            renderer.sharedMaterials=mats;
        }

        public void UpdateMeshRenderer(MeshRenderer renderer)
        {
            if(PreviousHeight == settings.BoardHeight && PreviousWidth == settings.BoardWidth) return;

            PreviousHeight = settings.BoardHeight;
            PreviousWidth = settings.BoardWidth;

            mats = new Material[settings.BoardHeight*settings.BoardWidth];

            for (int i = 0 ; i < settings.BoardHeight*settings.BoardWidth ; i++)
            {
                mats[i]=settings.Tilematerial;
            }
            renderer.sharedMaterials=mats;
        }
    }
}