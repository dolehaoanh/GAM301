Shader "Custom/MainMenuBackground"
{
    Properties
    {
        _BaseColor1 ("Base Color 1", Color) = (0.05, 0.06, 0.1, 1)
        _BaseColor2 ("Base Color 2", Color) = (0.12, 0.16, 0.22, 1)
        _GridColor ("Grid Color", Color) = (0.95, 0.8, 0.3, 0.12)
        _Speed ("Flow Speed", Float) = 0.05
        _GridSize ("Grid Size", Float) = 22.0
        _GridThickness ("Grid Thickness", Float) = 0.04
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Unlit"
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

            float4 _BaseColor1;
            float4 _BaseColor2;
            float4 _GridColor;
            float _Speed;
            float _GridSize;
            float _GridThickness;

            // Simple pseudo-noise function
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            // Smooth 2D noise
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
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Calculate aspect ratio dynamically using screen size
                float aspect = _ScreenSize.x * _ScreenSize.w;

                // Create a slow flowing wave coordinate
                float2 flowUV = input.uv * 3.0 + float2(_Time.y * _Speed, _Time.y * _Speed * 0.7);
                float n1 = smoothNoise(flowUV);
                float n2 = smoothNoise(flowUV * 2.0 - float2(_Time.y * _Speed * 0.5, 0.0));
                float combinedNoise = (n1 + n2) * 0.5;

                // Color gradient flow
                float4 baseCol = lerp(_BaseColor1, _BaseColor2, combinedNoise);

                // Add scanline pulse (bigger and less dense)
                float scanline = sin(input.uv.y * 6.0 + _Time.y * 1.2) * 0.5 + 0.5;
                float4 finalColor = baseCol + _GridColor * scanline * 0.25;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
