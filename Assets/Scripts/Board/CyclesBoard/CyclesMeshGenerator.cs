using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Torthello
{
    public class CyclesMeshGenerator : IMeshGenerator
    {
        protected Settings settings;
        protected float2[] nodes;
        protected List<int> edges;
        protected List<float3> edgesColor;
        private int prevWidth;
        private int prevHeight;

        public CyclesMeshGenerator(Settings settings)
        {
            this.settings = settings;
        }

        public void Destroy(MeshFilter mF)
        {

        }

        public void InitMesh(MeshFilter meshFilter)
        {
            prevWidth = settings.BoardWidth;
            prevHeight = settings.BoardHeight;
            int lCycle = settings.BoardHeight;
            int puissance = settings.BoardWidth;
            nodes = new float2[lCycle];
            edges = new List<int>();
            edgesColor = new List<float3>();
            List<int> powEdgesDone = new List<int>();
            const float RADIUS = 0.3f;
            float THETA = 2f * Mathf.PI / lCycle;
            for (int i = 0; i < lCycle; i++)
            {
                edges.AddRange(i == 0 && puissance < lCycle && puissance > 1 ?
                    new int[]
                    {
                        i, (i + 1) % lCycle,
                        i, (i + puissance) % lCycle
                    } :
                    new int[]
                    {
                        i, (i + 1) % lCycle
                    });
                edgesColor.AddRange(i == 0 && puissance < lCycle && puissance > 1 ?
                    new float3[]
                    {
                        new float3(1f, 0f, 0f),
                        new float3(0f, 1f, 0f)
                    } :
                    new float3[]
                    {
                        new float3(1f, 0f, 0f)
                    });
                if (puissance < lCycle && puissance > 1)
                {
                    int nextE = puissance % lCycle;
                    while (nextE != 0)
                    {
                        edges.AddRange(new int[]{
                        nextE, (nextE + puissance) % lCycle
                    });
                        edgesColor.Add(new float3(0f, 1f, 0f));

                        nextE = (nextE + puissance) % lCycle;
                    }
                }
                nodes[i] = new float2(0.5f + RADIUS * Mathf.Cos(i * THETA), 0.5f + RADIUS * Mathf.Sin(i * THETA));

            }
            settings.nodes = new Nodes(lCycle);
            settings.nodes.SetNodes(nodes);
            settings.edges = edges.ToArray();
            settings.edgesColors = edgesColor.ToArray();
        }

        public void UpdateMesh(MeshFilter meshFilter)
        {
            settings.nodes.SetNodes(nodes);
            if (settings.BoardWidth != prevWidth || settings.BoardHeight != prevHeight)
            {
                prevWidth = settings.BoardWidth;
                prevHeight = settings.BoardHeight;
                InitMesh(null);
            }
        }
    }
}