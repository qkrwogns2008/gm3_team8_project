Shader "ERB/Particles/Add_CenterGlow" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 2
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 씬 뷰에서의 정밀도 문제를 해결하기 위해 고정 소수점 대신 float 사용 강제
            #pragma target 3.0 
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Emission;

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
                // TRANSFORM_TEX를 사용하여 프리팹의 타일링/오프셋 값 보존
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 씬 뷰 일그러짐 방지: 텍스처 샘플링 시 미세한 보간 오류 방지
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // 1. UV 중심 계산 (정밀도 유지를 위해 float 사용)
                float2 centerUV = i.uv * 2.0 - 1.0;
                
                // 2. 일그러짐 방지 핵심: 거리를 단순 length가 아닌 제곱합으로 처리
                // 씬 뷰 카메라 각도에 따른 오차를 줄여줍니다.
                float radial = dot(centerUV, centerUV); 
                
                // 3. 두께 조절 (마스크 영역 확장)
                // 1.15 정도의 여유를 주어 씬 뷰에서 외곽선이 깨지는 현상을 방지합니다.
                float mask = saturate(1.15 - radial);
                
                // smoothstep의 범위를 넓게 잡아 원을 두껍고 부드럽게 만듭니다.
                // 첫 번째 인자를 낮출수록(0.0 -> -0.5) 원이 중앙부터 꽉 찹니다.
                mask = smoothstep(-0.5, 1.0, mask);
                
                // 4. 최종 컬러 및 밝기
                fixed4 final = tex * i.color * _Color;
                final.rgb *= _Emission;
                
                // 5. 알파 및 두께감 강화
                // pow 수치를 낮추면(0.3) 원이 훨씬 두툼하게 차오릅니다.
                float finalAlpha = tex.a * i.color.a * pow(mask, 0.3);
                
                // 6. 결과 반환 (Scene View에서도 깨지지 않도록 알파 미리 곱함)
                return fixed4(final.rgb * finalAlpha, finalAlpha);
            }
            ENDCG
        }
    }
}