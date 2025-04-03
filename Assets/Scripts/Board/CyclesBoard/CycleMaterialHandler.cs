using System;
using Unity.Mathematics;
using UnityEngine;

namespace Torthello
{

    public class CycleMaterialHandler : IMaterialHandler
    {
        protected Settings settings;

        protected ComputeBuffer nodesBuffer ;
        protected ComputeBuffer edgesBuffer ;
        protected ComputeBuffer edgesColorsBuffer ;
        protected int[] edges;
        protected float3[] nodes;
        protected float3[] edgesColors;
        protected int nodesRadius = 2;
        protected int edgesRadius = 2;
        protected Couleur nodesColor;
        public CycleMaterialHandler(Transform parent, Settings settings)
        {
            this.settings = settings;
        }

        private void UpdateMap()
        {
            //TODO: update map with the new values of the graph
                Shader s = settings.visualizationShader;

                ComputeBuffer nodesBuffer = new ComputeBuffer(settings.BoardWidth * settings.BoardHeight, sizeof(float) * 4);
                ComputeBuffer edgesBuffer = new ComputeBuffer(settings.BoardWidth * settings.BoardHeight, sizeof(float) * 4);
                ComputeBuffer edgesColorsBuffer = new ComputeBuffer(settings.BoardWidth * settings.BoardHeight, sizeof(float) * 4);
     
                
                float3[] p = new float3[settings.BoardWidth * settings.BoardHeight];
                int[] p1 = new int[settings.BoardWidth * settings.BoardHeight];
                int[] p2 = new int[settings.BoardWidth * settings.BoardHeight];

                nodesBuffer.SetData(p);

                nodesBuffer.Release();
        }

        public void InitMeshRenderer(MeshRenderer renderer)
        {
            //TODO: Finir l'initialisation du renderer
            nodesColor = Couleur.Vide;
            nodesBuffer = new ComputeBuffer(settings.BoardHeight, sizeof(float) * 3);
            edgesBuffer = new ComputeBuffer(settings.BoardHeight * 2, sizeof(int));
            edgesColorsBuffer = new ComputeBuffer(settings.BoardHeight * 2, sizeof(float) * 3);

            for(int i = 0; i < settings.BoardHeight; i++)
            {
                nodes[i] = new float3(0, 0, 0); // a modifier
                edges[i] =  0; // a modifier
                edgesColors[i] =  new float3(0, 0, 0); // a modifier
            }

        }
        //TODO: Finir ce qu'il y a en dessous

        public void UpdateMeshRenderer(MeshRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public void SetHoveredTile(int id)
        {
            throw new NotImplementedException();
        }

        public void FailedPlacement()
        {
            throw new NotImplementedException();
        }

        public void Destroy(MeshRenderer renderer)
        {
            throw new NotImplementedException();
        }
    }
}
