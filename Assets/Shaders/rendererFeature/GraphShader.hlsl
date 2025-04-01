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



            float projDist(float2 v, float2 w, float2 p)
            {
                l2 = pointDist(v, w); 
                if(l2 == 0. && projDist(p,v)){
                    t = max(0., min(1., dot(p-v, w-v)/l2));
                    proj = v + t * (w - v);
                    return pointDist(p, proj);
                }
                return 0.;
            }
            // return dist between p and v.
            float pointDist(float p, float v)
            {
                return length(v-p);
            }

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                

                // [(0. , 0.) -> (1. , 1.)]
                float2 fragPos = IN.ScreenPosition.xy;

                // for each node, test if in range => draw node.
                // else if in range of edges (close to end points or (proj in both ways.)) => draw edges.
                // else color = 0, 0, 0; alpha = 1.


                


                surface.BaseColor = float3(1., 1., 1.);
                surface.Alpha = 1;
                return surface;
            }
            
            #include "fullscreenPassRender.hlsl"
            
            ENDHLSL
        }
    }
}