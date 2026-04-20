Shader "ERB/Particles/Blend_CenterGlow_AtlasSafe" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 2
        _Opacity ("Opacity", Range(0, 1)) = 1
        [Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode ("Culling", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTestMode ("ZTestMode", Float) = 4
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
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
                // 텍스처 시트 애니메이션(ST)이 정상 작동하도록 UV 변환 유지
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. 텍스처 샘플링
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // 2. 최종 색상 계산 (에미션은 RGB에만 곱하는 것이 정석입니다)
                fixed4 col = tex * i.color * _Color;
                col.rgb *= _Emission;
                
                // 3. 알파 계산 (마스크를 제거하여 아틀라스/시트 이미지 보호)
                // 텍스처 배경이 검정색인데 사각형 테두리가 남는다면 아래 luma를 곱하세요.
                // 만약 텍스처 알파 채널이 완벽하다면 col.a = tex.a * i.color.a * _Color.a * _Opacity; 만 써도 됩니다.
                float luma = saturate(dot(tex.rgb, float3(0.299, 0.587, 0.114)) * 2.0); 
                
                // 텍스처의 알파와 밝기(Luma)를 조합하여 배경 노이즈 제거
                col.a = tex.a * i.color.a * _Color.a * _Opacity * luma;
                
                return col;
            }
            ENDCG
        }
    }
}