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
        
        // 알파 블렌딩 설정 유지
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
                fixed4 color : COLOR;
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
                // 1. 텍스처 샘플링
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // 2. [추가] 텍스처의 밝기를 알파로 활용 (Luminance)
                // 검정색 부분(RGB가 0에 가까운 곳)을 투명하게 만듭니다.
                float luma = dot(tex.rgb, float3(0.299, 0.587, 0.114));
                
                // 3. [추가] 원형 마스크 (사각형 테두리 절대 방지)
                float2 centerUV = i.uv * 2.0 - 1.0;
                float mask = saturate(1.0 - dot(centerUV, centerUV));
                mask = smoothstep(0.0, 0.2, mask); // 외곽을 아주 살짝만 깎음
                
                // 4. 최종 색상 계산
                fixed4 col = tex * i.color * _Color;
                col.rgb *= _Emission;
                
                // 5. 최종 알파 결정
                // (기존 알파 * 밝기 기반 알파 * 원형 마스크 * 투명도 속성)
                col.a = tex.a * i.color.a * _Color.a * _Opacity * luma * mask;
                
                return col;
            }
            ENDCG
        }
    }
}