Shader "UI/EyelidDualClose_Final"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(-0.1, 0.6)) = 0.0     // ✅ 최대값 0.6으로 확장
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _Cutoff;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distanceFromCenter = abs(i.uv.y - 0.5);
                
                // ✅ 부드럽게 닫히는 마스크, 중앙도 완전 차단되도록
                float mask = smoothstep(_Cutoff, _Cutoff + 0.02, distanceFromCenter);
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // ✅ 감길수록 더 가려지도록 마스크 적용 (반전 X)
                col.a *= mask;

                return col;
            }
            ENDCG
        }
    }
}
