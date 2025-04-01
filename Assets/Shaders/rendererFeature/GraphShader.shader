Shader "Tortello/RendererFeature/GraphRender"
{
    Properties
    {
        // TODO insert material properties here (ASK ME)
    }

    SubShader
    {

        Tags { "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Atmosphere drawing"
            
            // Render State
            Cull Off
            Blend SrcColor One, OneMinusSrcAlpha One
            ZTest Off
            ZWrite Off
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "fullscreenPassSetup.hlsl"

            // TODO declare properties here. (ASK ME)
            uniform StructuredBuffer<float3> _nodes;
            uniform int _numNodes;
            uniform StructuredBuffer<int> _edges;
            uniform int _numEdges;
            uniform StructuredBuffer<float3> _edgesColors;
            uniform float _nodesRadius;
            uniform float _edgesRadius;
            uniform float3 _nodesColor;

            // return dist between p and v.
            float pointDist(float2 p, float2 v)
            {
                return length(v-p);
            }

            float projDist(float2 v, float2 w, float2 p)
            {
                float l2 = pointDist(v, w);
                if(l2 == 0.) return pointDist(p,v);

                float t = max(0., min(1., dot(p-v, w-v)/l2));
                float2 proj = v + t * (w - v);
                return pointDist(p, proj);

            }
            

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                

                // [(0. , 0.) -> (1. , 1.)]
                float2 fragPos = IN.ScreenPosition.xy;

                for(int i = 0; i < _numNodes; i++ ){
                    if(pointDist(fragPos,_nodes[i].xy) <= _nodesRadius ){
                        surface.BaseColor = _nodes[i].z == 0? _nodesColor : _nodes[i].z == 1? float3(0. ,0. ,0.) : float3(1., 1., 1.);
                        surface.Alpha = 1.;
                        return surface;
                    }
                }
                for(int j = 0; j < _numEdges; j++){
                    if( pointDist(fragPos, _nodes[_edges[2*j]].xy) <= _edgesRadius || pointDist(fragPos, _nodes[_edges[2*j+1]].xy) <= _edgesRadius || 
                    ( projDist(_nodes[_edges[2*j]].xy, _nodes[_edges[2*j+1]].xy, fragPos) <= _edgesRadius && projDist(_nodes[_edges[2*j+1]].xy, _nodes[_edges[2*j]].xy, fragPos) <= _edgesRadius ) ){
                        surface.BaseColor = _edgesColors[j];
                        surface.Alpha = 1.;
                        return surface;
                    }
                }

                // for each node, test if in range => draw node.
                // else if in range of edges (close to end points or (proj in both ways.)) => draw edges.
                // else color = 0, 0, 0; alpha = 1.


                


                surface.BaseColor = float3(0., 0., 0.);
                surface.Alpha = 0.;
                return surface;
            }
            
            #include "fullscreenPassRender.hlsl"
            
            ENDHLSL
        }
    }
}