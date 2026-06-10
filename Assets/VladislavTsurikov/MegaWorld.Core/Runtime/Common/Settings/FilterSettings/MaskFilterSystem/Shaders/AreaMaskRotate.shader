Shader "Hidden/MegaWorld/AreaMaskRotate"
{
    Properties
    {
        _AreaMaskTex("Area Mask Texture", 2D) = "black" {}
        _AreaMaskUAxisX("Area Mask U Axis X", Float) = 1
        _AreaMaskUAxisY("Area Mask U Axis Y", Float) = 0
        _AreaMaskVAxisX("Area Mask V Axis X", Float) = 0
        _AreaMaskVAxisY("Area Mask V Axis Y", Float) = 1
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

            sampler2D _AreaMaskTex;
            float _AreaMaskUAxisX;
            float _AreaMaskUAxisY;
            float _AreaMaskVAxisX;
            float _AreaMaskVAxisY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 localUV = i.uv - 0.5f;
                float2 sourceUV = 0.5f +
                    localUV.x * float2(_AreaMaskUAxisX, _AreaMaskUAxisY) +
                    localUV.y * float2(_AreaMaskVAxisX, _AreaMaskVAxisY);

                if (sourceUV.x < 0.0f || sourceUV.x > 1.0f || sourceUV.y < 0.0f || sourceUV.y > 1.0f)
                {
                    return 0.0f;
                }

                return tex2D(_AreaMaskTex, sourceUV);
            }
            ENDCG
        }
    }

    Fallback Off
}
