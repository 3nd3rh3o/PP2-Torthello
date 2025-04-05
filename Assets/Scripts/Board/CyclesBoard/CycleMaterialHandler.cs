using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Torthello
{

    public class CycleMaterialHandler : FlatBoardMaterialHandler
    {
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
        public CycleMaterialHandler(Settings settings) : base(settings)
        {
            this.settings = settings;
        }

        public override void InitMeshRenderer(MeshRenderer renderer)
        {
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            nodesColor = Couleur.Vide;
            nodesBuffer = new ComputeBuffer(settings.nodes.GetNodes().Length, sizeof(float) * 3);
            edgesBuffer = new ComputeBuffer(settings.edges.Length, sizeof(int));
            edgesColorsBuffer = new ComputeBuffer(settings.edgesColors.Length, sizeof(float) * 3);

            nodesBuffer.SetData(settings.nodes.GetNodes());
            edgesBuffer.SetData(settings.edges);
            edgesColorsBuffer.SetData(settings.edgesColors);
            settings.graphMaterial.SetBuffer("_nodes", nodesBuffer);
            settings.graphMaterial.SetInt("_numNodes", nodesBuffer.count);
            settings.graphMaterial.SetBuffer("_edges", edgesBuffer);
            settings.graphMaterial.SetInt("_numEdges", edgesBuffer.count);
            settings.graphMaterial.SetBuffer("_edgesColors", edgesColorsBuffer);
            settings.graphMaterial.SetFloat("_nodesRadius", 0.02f);
            settings.graphMaterial.SetFloat("_edgesRadius", 0.01f);
            settings.graphMaterial.SetColor("_nodesColor", Color.HSVToRGB(settings.hue, 0.7f, 0.7f));
            
            settings.graphRendererFeature.SetActive(true);
        }

        //TODO: Finir ce qu'il y a en dessous

        public override void UpdateMeshRenderer(MeshRenderer renderer)
        {
            if (settings.BoardWidth != previousWidth || settings.BoardHeight != previousHeight)
            {
                nodesBuffer.Release();
                edgesBuffer.Release();
                edgesColorsBuffer.Release();
                nodesBuffer = new ComputeBuffer(settings.nodes.GetNodes().Length, sizeof(float) * 3);
                edgesBuffer = new ComputeBuffer(settings.edges.Length, sizeof(int));
                edgesColorsBuffer = new ComputeBuffer(settings.edgesColors.Length, sizeof(float) * 3);
                previousWidth = settings.BoardWidth;
                previousHeight = settings.BoardHeight;
            }
            nodesBuffer.SetData(settings.nodes.GetNodes());
            edgesBuffer.SetData(settings.edges);
            edgesColorsBuffer.SetData(settings.edgesColors);
            settings.graphMaterial.SetBuffer("_nodes", nodesBuffer);
            settings.graphMaterial.SetInt("_numNodes", nodesBuffer.count);
            settings.graphMaterial.SetBuffer("_edges", edgesBuffer);
            settings.graphMaterial.SetInt("_numEdges", edgesBuffer.count / 2);
            settings.graphMaterial.SetBuffer("_edgesColors", edgesColorsBuffer);
            settings.graphMaterial.SetFloat("_nodesRadius", 0.02f);
            settings.graphMaterial.SetFloat("_edgesRadius", 0.01f);
            settings.graphMaterial.SetColor("_nodesColor", Color.HSVToRGB(settings.hue, 0.7f, 0.7f));
            
            settings.graphMaterial.SetInt("_hoveredTile", settings.hoveredTile);
            settings.graphMaterial.SetVector("_hoverColor", new Vector3(1f, 1f, 1f));
        }

        public override void Destroy(MeshRenderer renderer)
        {
            settings.graphRendererFeature.SetActive(false);
            nodesBuffer.Release();
            edgesBuffer.Release();
            edgesColorsBuffer.Release();
        }
    }
}
