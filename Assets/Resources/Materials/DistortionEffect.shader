Shader "Custom/DistortionEffect"
{
    Properties
    {
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02  // 강도 범위 축소
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1.5                      // 속도 범위 축소
        _WaveFrequency ("Wave Frequency", Range(0, 20)) = 5               // 주파수 범위 축소
        _DistortionFade ("Distortion Fade", Range(0, 1)) = 0.5            // 페이드 효과 추가
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        GrabPass { "_GrabTexture" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
            };

            sampler2D _GrabTexture;
            float _DistortionStrength;
            float _WaveSpeed;
            float _WaveFrequency;
            float _DistortionFade;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;
                
                // 더 부드러운 웨이브 효과
                float wave = sin(i.uv.y * _WaveFrequency + _Time.y * _WaveSpeed);
                wave *= smoothstep(0, _DistortionFade, 1.0 - i.uv.y); // 부드러운 페이드 아웃
                
                // 왜곡 효과 적용
                uv.x += wave * _DistortionStrength;
                
                return tex2D(_GrabTexture, uv);
            }
            ENDCG
        }
    }
}