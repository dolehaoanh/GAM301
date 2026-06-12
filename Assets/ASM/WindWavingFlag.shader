Shader "Custom/WindWavingFlag"
{
    Properties
    {
        _BaseMap ("Flag Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _WaveSpeed ("Wave Speed", Float) = 6.0
        _WaveFrequency ("Wave Frequency", Float) = 4.0
        _WaveStrength ("Wave Strength", Float) = 0.2
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline" 
        }
        LOD 100

        Pass
        {
            Name "FlagPass"
            Cull Off // Double-sided so the flag is visible from both sides

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
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float _WaveSpeed;
            float _WaveFrequency;
            float _WaveStrength;

            Varyings vert(Attributes input)
            {
                Varyings output;

                // Vertex displacement to simulate wind
                // Anchor factor: waves more on the free end (UV x close to 1) and remains static at the pole (UV x close to 0)
                float anchorFactor = input.uv.x; 

                // Create wave animation using sine over time and local position
                float wave = sin(input.positionOS.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveStrength * anchorFactor;
                
                // Offset vertices along the Y (upward/downward waving) and Z (flapping back/forth) axes
                input.positionOS.y += wave * 0.7;
                input.positionOS.z += wave * 0.5;

                // Transform to Clip Space
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.uv = input.uv;

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Sample texture
                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                return texColor;
            }
            ENDHLSL
        }
    }
}
