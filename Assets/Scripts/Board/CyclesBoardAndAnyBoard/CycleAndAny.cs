using System;
using Unity.Mathematics;
using UnityEngine;

namespace Torthello
{

    public class CycleAndAny : FlatBoardPawnProccessor
    {
        public CycleAndAny(Transform parent, Settings settings) : base(parent, settings)
        {
        }

        private void UpdateMapCycle()
        {
                Shader cs = settings.visualizationShader;

                ComputeBuffer nodesBuffer = new ComputeBuffer(settings.BoardWidth * settings.BoardHeight, sizeof(float) * 4);
                ComputeBuffer edgesBuffer = new ComputeBuffer(settings.BoardWidth * settings.BoardHeight, sizeof(float) * 4);
                ComputeBuffer edgesColorsBuffer = new ComputeBuffer(settings.BoardWidth * settings.BoardHeight, sizeof(float) * 4);
                int numNodes = 0;
                int numEdges = 0;
                float nodesRadius = 0.0f;
                float edgesRadius = 0.0f;
                float nodesColor = 0.0f;
                
                float3[] p = new float3[settings.BoardWidth * settings.BoardHeight];
                int[] p1 = new int[settings.BoardWidth * settings.BoardHeight];
                int[] p2 = new int[settings.BoardWidth * settings.BoardHeight];

                nodesBuffer.SetData(p);

                nodesBuffer.Release();
        }
        private void UpdateMapAny()
        {
        }
    }
}
