Shader "Custom/HologramPlacement"
{
    Properties
    {
        _Color ("Hologram Color", Color) = (0.2, 1.0, 0.5, 0.4)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
        _ScanlineSpeed ("Scanline Speed", Float) = 2.5
        _ScanlineFrequency ("Scanline Frequency", Float) = 15.0
        _ScanlineIntensity ("Scanline Intensity", Range(0.0, 1.0)) = 0.6
        _Glow ("Glow Intensity", Range(1.0, 5.0)) = 2.0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
        }
        LOD 100

        Pass
        {
            Name "HologramPass"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 viewDirWS    : TEXCOORD2;
            };

            float4 _Color;
            float _RimPower;
            float _ScanlineSpeed;
            float _ScanlineFrequency;
            float _ScanlineIntensity;
            float _Glow;

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Position transforms
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;

                // Normal transforms
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normInputs.normalWS;

                // Calculate view direction in world space
                output.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(input.viewDirWS);

                // 1. Fresnel / Rim lighting effect
                float rim = 1.0 - saturate(dot(normal, viewDir));
                float rimGlow = pow(rim, _RimPower);

                // 2. Moving horizontal scanlines based on world Y-position and time
                float scanline = sin(input.positionWS.y * _ScanlineFrequency + _Time.y * _ScanlineSpeed) * 0.5 + 0.5;
                scanline = lerp(1.0, scanline, _ScanlineIntensity);

                // 3. Combine effects
                float3 finalColor = _Color.rgb * _Glow * (rimGlow + (1.0 - rimGlow) * scanline * 0.3);
                
                // Adjust opacity: transparent inside, glowing and scanline-dense at borders
                float finalAlpha = saturate(rimGlow * 1.5 + scanline * 0.25) * _Color.a;

                return float4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}
