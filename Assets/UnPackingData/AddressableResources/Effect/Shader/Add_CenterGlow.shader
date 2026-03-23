Shader "ERB/Particles/Add_CenterGlow" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 2
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        // 흰 네모 방지 및 빛나는 효과 설정
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass {
            CGPROGRAM // HLSL 대신 더 호환성 좋은 CG 구문 사용
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Emission;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // 파티클 시스템의 Start Color를 직접 받음
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
                o.color = v.color; // 파티클 색상 데이터 전달
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 텍스처 컬러 추출
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // 최종 색상 = 텍스처 * 파티클컬러 * 인스펙터컬러 * 에미션(밝기)
                fixed4 final = tex * i.color * _Color * _Emission;
                
                return final;
            }
            ENDCG
        }
    }
}