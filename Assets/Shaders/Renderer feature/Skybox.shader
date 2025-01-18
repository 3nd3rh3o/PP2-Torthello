Shader "Torthello/Skybox"
{
    Properties
    {
        _UpColor ("Top color", Color) = (1, 1, 1, 1)
        _BotColor ("Bottom color", Color) = (1, 1, 1, 1)
        _LightColor ("Light color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {

        Tags { "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Atmosphere drawing"
            
            // Render State
            Cull Off
            Blend One Zero, One Zero
            ZTest Off
            ZWrite Off
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "fullscreenPassSetup.hlsl"

            
            float3 _UpColor;
            float3 _BotColor;
            float3 _LightDir;
            float3 _LightColor;

            float InverseLerp(float A, float B, float T)
            {
                return (T - A) / (B - A);
            }

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float dstToSurface = length(IN.WorldSpacePosition - _WorldSpaceCameraPos);
                
                float3 color;
                if (dstToSurface < 100)
                {
                    color = SampleSceneColor(IN.ScreenPosition.xy);
                }
                else
                {
                    if (dot(IN.ray.xyz, _LightDir.xyz) > 0.9999)
                    {
                        float factor = InverseLerp(0.9999, 1., dot(IN.ray.xyz, _LightDir.xyz));
                        color = lerp(lerp(_BotColor, _UpColor, (1. + dot(-IN.ray.xyz, float3(0, 1, 0))) * .5), _LightColor, factor);
                    }
                    else
                    {
                        color = lerp(_BotColor, _UpColor, (1. + dot(-IN.ray.xyz, float3(0, 1, 0))) * .5);
                    }
                }
                

                surface.BaseColor = color;
                surface.Alpha = float(1);
                return surface;
            }
            
            #include "fullscreenPassRender.hlsl"
            
            ENDHLSL
        }
    }
}