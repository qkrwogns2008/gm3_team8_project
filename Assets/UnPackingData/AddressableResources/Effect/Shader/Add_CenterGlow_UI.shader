Shader "ERB/Particles/Add_CenterGlow_UI" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 2

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Blend SrcAlpha One
        ColorMask [_ColorMask]
        ZWrite Off
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0 
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Emission;
            float4 _ClipRect; 

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float2 worldPos : TEXCOORD1; // 위치 데이터 채널
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                // UI 마스킹을 위한 좌표 전달
                o.worldPos = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // [절단] 마스크 영역 계산 (가장 엄격하게 체크)
                float clipping = UnityGet2DClipping(i.worldPos, _ClipRect);
                // 마스크 밖이면 연산 중단
                if (clipping <= 0.01) discard;

                fixed4 tex = tex2D(_MainTex, i.uv);
                float2 centerUV = i.uv * 2.0 - 1.0;
                float radial = dot(centerUV, centerUV); 
                float mask = saturate(1.15 - radial);
                mask = smoothstep(-0.5, 1.0, mask);
                
                fixed4 col = tex * i.color * _Color;
                float alpha = tex.a * i.color.a * pow(mask, 0.3);

                // 최종 알파가 낮아도 찌꺼기가 남으므로 버림
                clip(alpha - 0.05);

                float3 finalRGB = col.rgb * _Emission * alpha;
                return fixed4(finalRGB, alpha);
            }
            ENDCG
        }
    }
}