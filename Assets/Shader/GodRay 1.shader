Shader "Horus/GodRay 2"
{
    Properties
    {
        [Enum(Transparent, 10, Additive, 1)] _DstBlend ("Rendering Mode", Float) = 1
        _Color ("Color", color) = (1, 1, 1, 0.5)
        _Power ("Highlight Power", range(0.001, 10)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha [_DstBlend]
        ZWrite Off
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "MyLib.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            // sampler2D _MainTex;
            float _Power;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                fixed4 col = _Color;
                col.a *= i.uv.y;
                col.a *= pow(max(remap_clamp(abs(i.uv.x - 0.5), 0, 0.5, 1, 0), 1e-5), _Power);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}