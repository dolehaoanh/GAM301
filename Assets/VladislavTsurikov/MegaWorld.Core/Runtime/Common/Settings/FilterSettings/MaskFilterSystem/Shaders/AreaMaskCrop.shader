Shader "Hidden/MegaWorld/AreaMaskCrop"
{
    Properties
    {
        _AreaMaskTex("Area Mask Texture", 2D) = "black" {}
        _AreaMaskCenterU("Area Mask Center U", Float) = 0.5
        _AreaMaskCenterV("Area Mask Center V", Float) = 0.5
        _AreaMaskSizeU("Area Mask Size U", Float) = 1
        _AreaMaskSizeV("Area Mask Size V", Float) = 1
    }

    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "TerrainTool.cginc"

            sampler2D _AreaMaskTex;
            float _AreaMaskCenterU;
            float _AreaMaskCenterV;
            float _AreaMaskSizeU;
            float _AreaMaskSizeV;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 pcUV : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pcUV = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
                if (brushUV.x < 0.0f || brushUV.x > 1.0f || brushUV.y < 0.0f || brushUV.y > 1.0f)
                {
                    return 0.0f;
                }

                float2 maskUV = float2(_AreaMaskCenterU, _AreaMaskCenterV) +
                    (brushUV - 0.5f) * float2(_AreaMaskSizeU, _AreaMaskSizeV);

                if (maskUV.x < 0.0f || maskUV.x > 1.0f || maskUV.y < 0.0f || maskUV.y > 1.0f)
                {
                    return 0.0f;
                }

                return tex2D(_AreaMaskTex, maskUV);
            }
            ENDCG
        }
    }

    Fallback Off
}
