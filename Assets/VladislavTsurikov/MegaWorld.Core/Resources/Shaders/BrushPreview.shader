Shader "MegaWorld/TerrainEngine/BrushPreview"
{
    Properties
    {
        _BrushTex("Brush Tex", 2D) = "black" {}
        _BlackThreshold("Black Threshold", Range(0,1)) = 0.001
    }

    SubShader
    {
        ZTest Always Cull Back ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGINCLUDE
        #pragma exclude_renderers gles

        #include "UnityCG.cginc"
        #include "TerrainPreview.cginc"

        sampler2D _BrushTex;
        float4 _BrushTex_TexelSize;
        float _BlackThreshold;

        float SampleBrushValue(float2 uv)
        {
            return UnpackHeightmap(tex2D(_BrushTex, uv));
        }

        float SampleBrushMask(float2 uv)
        {
            float value = SampleBrushValue(uv);
            return step(_BlackThreshold, value);
        }
        ENDCG

        Pass
        {
            Name "TerrainPreviewProcedural"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float4 clipPosition : SV_POSITION;
                float3 positionWorld : TEXCOORD0;
                float3 positionWorldOrig : TEXCOORD1;
                float2 pcPixels : TEXCOORD2;
                float2 brushUV : TEXCOORD3;
            };

            v2f vert(uint vid : SV_VertexID)
            {
                float2 pcPixels = BuildProceduralQuadMeshVertex(vid);

                float2 heightmapUV = PaintContextPixelsToHeightmapUV(pcPixels);
                float heightmapSample = UnpackHeightmap(tex2Dlod(_Heightmap, float4(heightmapUV, 0, 0)));

                float2 brushUV = PaintContextPixelsToBrushUV(pcPixels);

                float3 positionObject = PaintContextPixelsToObjectPosition(pcPixels, heightmapSample);
                float3 positionWorld = TerrainObjectToWorldPosition(positionObject);

                v2f o;
                o.pcPixels = pcPixels;
                o.positionWorld = positionWorld;
                o.positionWorldOrig = positionWorld;
                o.clipPosition = UnityWorldToClipPos(positionWorld);
                o.brushUV = brushUV;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;
                if (oob == 0.0f)
                {
                    return 0;
                }

                float brushSample = SampleBrushValue(i.brushUV);
                float2 texel = _BrushTex_TexelSize.xy;

                float centerMask = step(_BlackThreshold, brushSample);

                float leftMask = SampleBrushMask(i.brushUV + float2(-texel.x, 0.0f));
                float rightMask = SampleBrushMask(i.brushUV + float2(texel.x, 0.0f));
                float downMask = SampleBrushMask(i.brushUV + float2(0.0f, -texel.y));
                float upMask = SampleBrushMask(i.brushUV + float2(0.0f, texel.y));

                float downLeftMask = SampleBrushMask(i.brushUV + float2(-texel.x, -texel.y));
                float downRightMask = SampleBrushMask(i.brushUV + float2(texel.x, -texel.y));
                float upLeftMask = SampleBrushMask(i.brushUV + float2(-texel.x, texel.y));
                float upRightMask = SampleBrushMask(i.brushUV + float2(texel.x, texel.y));

                float outline = 0.0f;
                outline = max(outline, centerMask * (1.0f - leftMask));
                outline = max(outline, centerMask * (1.0f - rightMask));
                outline = max(outline, centerMask * (1.0f - downMask));
                outline = max(outline, centerMask * (1.0f - upMask));
                outline = max(outline, centerMask * (1.0f - downLeftMask));
                outline = max(outline, centerMask * (1.0f - downRightMask));
                outline = max(outline, centerMask * (1.0f - upLeftMask));
                outline = max(outline, centerMask * (1.0f - upRightMask));

                float fill = brushSample;
                float fillAlpha = 0.6f * saturate(fill * 5.0f);

                float outlineStrength = outline;
                float outlineAlpha = outlineStrength;

                float3 baseColor = float3(0.5f, 0.5f, 1.0f);
                float3 fillColor = baseColor * saturate(0.5f * fill);
                float3 outlineColor = baseColor;

                float3 color = fillColor + outlineColor * outlineStrength;
                float alpha = max(fillAlpha, outlineAlpha);

                return float4(color, alpha) * oob;
            }
            ENDCG
        }

        Pass
        {
            Name "TerrainPreviewProceduralDeltaNormals"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _HeightmapOrig;

            struct v2f
            {
                float4 clipPosition : SV_POSITION;
                float3 positionWorld : TEXCOORD0;
                float3 positionWorldOrig : TEXCOORD1;
                float2 pcPixels : TEXCOORD2;
                float2 brushUV : TEXCOORD3;
            };

            v2f vert(uint vid : SV_VertexID)
            {
                float2 pcPixels = BuildProceduralQuadMeshVertex(vid);

                float2 heightmapUV = PaintContextPixelsToHeightmapUV(pcPixels);
                float heightmapSample = UnpackHeightmap(tex2Dlod(_Heightmap, float4(heightmapUV, 0, 0)));
                float heightmapSampleOrig = UnpackHeightmap(tex2Dlod(_HeightmapOrig, float4(heightmapUV, 0, 0)));

                float2 brushUV = PaintContextPixelsToBrushUV(pcPixels);

                float3 positionObject = PaintContextPixelsToObjectPosition(pcPixels, heightmapSample);
                float3 positionWorld = TerrainObjectToWorldPosition(positionObject);

                float3 positionObjectOrig = PaintContextPixelsToObjectPosition(pcPixels, heightmapSampleOrig);
                float3 positionWorldOrig = TerrainObjectToWorldPosition(positionObjectOrig);

                v2f o;
                o.pcPixels = pcPixels;
                o.positionWorld = positionWorld;
                o.positionWorldOrig = positionWorldOrig;
                o.clipPosition = UnityWorldToClipPos(positionWorld);
                o.brushUV = brushUV;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float brushSample = UnpackHeightmap(tex2D(_BrushTex, i.brushUV));
                float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;

                float deltaHeight = abs(i.positionWorld.y - i.positionWorldOrig.y);

                float3 dx = ddx(i.positionWorld);
                float3 dy = ddy(i.positionWorld);
                float3 normal = normalize(cross(dy, dx));

                float3 lightDir = UnityWorldSpaceLightDir(i.positionWorld.xyz);

                float4 color;
                color.rgb = saturate(normal.xzy * float3(-0.5f, -0.5f, 0.5f) + 0.5f);
                color.rgb = lerp(color.rgb, float3(1.0f, 1.0f, 1.0f), 0.3f);
                color.rgb = color.rgb * (0.1f + 0.9f * dot(lightDir, normal));
                color.a = 0.9f * saturate(0.2f * deltaHeight);

                return color * oob;
            }
            ENDCG
        }

        Pass
        {
            Name "TerrainPreviewProceduralDeltaStripes"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _HeightmapOrig;

            struct v2f
            {
                float4 clipPosition : SV_POSITION;
                float3 positionWorld : TEXCOORD0;
                float3 positionWorldOrig : TEXCOORD1;
                float2 pcPixels : TEXCOORD2;
                float2 brushUV : TEXCOORD3;
            };

            v2f vert(uint vid : SV_VertexID)
            {
                float2 pcPixels = BuildProceduralQuadMeshVertex(vid);

                float2 heightmapUV = PaintContextPixelsToHeightmapUV(pcPixels);
                float heightmapSample = UnpackHeightmap(tex2Dlod(_Heightmap, float4(heightmapUV, 0, 0)));
                float heightmapSampleOrig = UnpackHeightmap(tex2Dlod(_HeightmapOrig, float4(heightmapUV, 0, 0)));

                float2 brushUV = PaintContextPixelsToBrushUV(pcPixels);

                float3 positionObject = PaintContextPixelsToObjectPosition(pcPixels, heightmapSample);
                float3 positionWorld = TerrainObjectToWorldPosition(positionObject);

                float3 positionObjectOrig = PaintContextPixelsToObjectPosition(pcPixels, heightmapSampleOrig);
                float3 positionWorldOrig = TerrainObjectToWorldPosition(positionObjectOrig);

                v2f o;
                o.pcPixels = pcPixels;
                o.positionWorld = positionWorld;
                o.positionWorldOrig = positionWorldOrig;
                o.clipPosition = UnityWorldToClipPos(positionWorld);
                o.brushUV = brushUV;
                return o;
            }

            float MultiStripes(in float x, in float freq1, in float freq2)
            {
                float2 derivatives = float2(ddx(x), ddy(x));
                float derivLen = length(derivatives);

                float tweak = 0.5;
                float sharpen = tweak / max(derivLen, 0.00001f);

                float triwave1 = abs(frac(x * freq1) - 0.5f) - 0.25f;
                float triwave2 = abs(frac(x * freq2) - 0.5f) - 0.25f;

                float width = 0.95f;

                float result1 = saturate((triwave1 - width * 0.25f) * sharpen / freq1 + 0.75f);
                float result2 = saturate((triwave2 - width * 0.25f) * sharpen / freq2 + 0.75f);

                return max(result1, result2);
            }

            float4 frag(v2f i) : SV_Target
            {
                float brushSample = UnpackHeightmap(tex2D(_BrushTex, i.brushUV));
                float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;

                float heightStripes = MultiStripes(i.positionWorld.y + 0.25f, 0.0625f, 1.0f);
                float xStripes = MultiStripes(i.positionWorld.x, 0.03125f, 0.5f);
                float zStripes = MultiStripes(i.positionWorld.z, 0.03125f, 0.5f);

                float deltaHeight = saturate(abs(i.positionWorld.y - i.positionWorldOrig.y));
                float4 color = lerp(float4(0.5f, 0.5f, 1.0f, 1.0f), float4(0.5f, 1.0f, 0.5f, 1.0f), deltaHeight);
                return color * lerp(deltaHeight * 0.5f, (brushSample > 0.0f ? 1.0f : 0.0f),
                                    0.5f * saturate(heightStripes + xStripes + zStripes)) * oob;
            }
            ENDCG
        }
    }

    Fallback Off
}
