Shader "ERB/Particles/Add_CenterGlow" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_Noise ("Noise", 2D) = "white" {}
		_Flow ("Flow", 2D) = "white" {}
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_SpeedMainTexUVNoiseZW ("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_DistortionSpeedXYPowerZ ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
		_Emission ("Emission", Float) = 2
		_Color ("Color", Vector) = (0.5,0.5,0.5,1)
		[Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode ("Culling", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTestMode ("ZTestMode", Float) = 4
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[Header(Transform)] [Toggle] _EnableCurvedTransform ("Enable Curved Transform", Float) = 0
		[Space(20)] _Cutoff ("Cutoff", Range(0, 1)) = 0
		_CutoffSoftness ("Cutoff softness", Range(0, 1)) = 0
		[Space(20)] [Toggle] _UseNoise ("Use Simple Noise", Float) = 0
		_NoiseFrequency ("Noise Frequency (XY)", Vector) = (1,1,0,0)
		_NoiseSpeed ("Noise Speed (XY)", Vector) = (1,1,0,0)
		_Amplitude ("Amplitude (XY)", Vector) = (1,1,0,0)
		[Space(20)] [Header(Cull Distance)] [Space(10)] [Toggle] _UseCullDistance ("Use Cull Distance", Float) = 0
		_CullDistance ("Cull Distance", Float) = 350
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
}