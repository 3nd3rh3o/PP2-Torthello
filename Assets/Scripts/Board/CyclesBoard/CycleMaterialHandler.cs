using System;
using Unity.Mathematics;
using UnityEngine;

namespace Torthello
{

    public class CycleMaterialHandler : IMaterialHandler
    {
        protected Settings settings;
        protected CyclesBoardGraph graph;
        protected ComputeBuffer nodesBuffer ;
        protected ComputeBuffer edgesBuffer ;
        protected ComputeBuffer edgesColorsBuffer ;
        protected int[] edges;
        protected float3[] nodes;
        protected float3[] edgesColors;
        protected int nodesRadius = 2;
        protected int edgesRadius = 2;
        protected Couleur nodesColor;
        public CycleMaterialHandler(Settings settings)
        {
            this.settings = settings;
        }

        public void InitMeshRenderer(MeshRenderer renderer)
        {
            //TODO: Finir l'initialisation du renderer
            nodesColor = Couleur.Vide;
            nodesBuffer = new ComputeBuffer(settings.BoardHeight, sizeof(float) * 3);
            edgesBuffer = new ComputeBuffer(settings.BoardHeight * 2, sizeof(int));
            edgesColorsBuffer = new ComputeBuffer(settings.BoardHeight * 2, sizeof(float) * 3);

            graph = new CyclesBoardGraph(settings);
            edges = graph.GetEdges();
            edgesColors = graph.GetEdgesColors();
            nodes = new float3[settings.BoardHeight];
            

        }

        //TODO: Finir ce qu'il y a en dessous

        public void UpdateMeshRenderer(MeshRenderer renderer)
        {
            throw new NotImplementedException();
            //TODO: faire tous les setData ici
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

            //faire les release a la fin de destroy
        }
    }
}
