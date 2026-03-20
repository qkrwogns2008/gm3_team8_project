Shader "ERB/Particles/Blend_CenterGlow" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 2
        _Opacity ("Opacity", Range(0, 1)) = 1
        [Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode ("Culling", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTestMode ("ZTestMode", Float) = 4
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        // --- 부드러운 투명(Alpha Blend) 설정 ---
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off
        Cull [_CullMode]
        ZTest [_ZTestMode]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Emission;
            float _Opacity;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // 파티클 Start Color 수신
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 텍스처 샘플링
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // 최종 색상 계산
                // 텍스처 * 파티클 컬러 * 인스펙터 컬러 * 에미션
                fixed4 col = tex * i.color * _Color * _Emission;
                
                // 투명도 조절 (_Opacity 프로퍼티 반영)
                col.a = tex.a * i.color.a * _Color.a * _Opacity;
                
                return col;
            }
            ENDCG
        }
    }
}