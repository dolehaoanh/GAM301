Shader "Custom/Dissolve"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _DissolveAmount ("Dissolve Amount", Range(0, 1.05)) = 0.0
        _EdgeColor ("Edge Color", Color) = (2, 0.5, 0, 1) // Glowing orange/red
        _EdgeWidth ("Edge Width", Range(0.005, 0.1)) = 0.03
        _NoiseScale ("Noise Scale", Float) = 12.0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="TransparentCutout" 
            "Queue"="AlphaTest" 
            "RenderPipeline"="UniversalPipeline" 
        }
        LOD 100

        Pass
        {
            Name "DissolvePass"
            Cull Off // Render back faces too for a nice hollow effect
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionWS   : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float _DissolveAmount;
            float4 _EdgeColor;
            float _EdgeWidth;
            float _NoiseScale;

            // Simple noise function
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            // Smooth noise function
            float smoothNoise(float2 uv)
            {
                float2 id = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                float a = noise(id);
                float b = noise(id + float2(1.0, 0.0));
                float c = noise(id + float2(0.0, 1.0));
                float d = noise(id + float2(1.0, 1.0));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.uv = input.uv;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Sample original texture
                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // Calculate procedural noise value based on local/world coordinates or UVs
                float noiseVal = smoothNoise(input.uv * _NoiseScale);

                // Clip pixels below dissolve threshold
                clip(noiseVal - _DissolveAmount);

                // Calculate edge glow (pixels that are about to be dissolved)
                float edge = step(noiseVal - _EdgeWidth, _DissolveAmount);

                // Add edge color glow to the final color
                float3 finalColor = texColor.rgb + (edge * _EdgeColor.rgb);

                return float4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
}
