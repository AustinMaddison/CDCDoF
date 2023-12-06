Shader "PostProcessing/CscLensBlur"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Spread("Standard Deviation (Spread)", Float) = 0
		_KernelSize("Kernel Size", Integer) = 1
    }
    SubShader
    {
        Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

		HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#define E 2.71828f

		sampler2D _MainTex;

		CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_TexelSize;
			uint _KernelSize;
			float _Spread;
		CBUFFER_END

		float gaussian(int x)
		{
			float sigmaSqu = _Spread * _Spread;
			return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
		}

		struct appdata
		{
			float4 positionOS : Position;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS : SV_Position;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
			o.uv = v.uv;
			return o;
		}

		// The precomputed CSC Real and Imaginary Kernel's from the paper referenced in the projects proposal: https://dl.acm.org/doi/10.1145/3084363.3085022.
		#define KERNEL_RADIUS 8
		#define KERNEL_COUNT 17

		// 2-Component (For Far Field Blur, More Precise)
		static const float4 Kernel0BracketsRealXY_ImZW_2 = float4(-0.038708,0.943062,-0.025574,0.660892);
		static const float2 Kernel0Weights_RealX_ImY_2 = float2(0.411259,-0.548794);
		static const float4 Kernel0_RealX_ImY_RealZ_ImW_2[] = {
				float4(/*XY: Non Bracketed*/0.014096,-0.022658,/*Bracketed WZ:*/0.055991,0.004413),
				float4(/*XY: Non Bracketed*/-0.020612,-0.025574,/*Bracketed WZ:*/0.019188,0.000000),
				float4(/*XY: Non Bracketed*/-0.038708,0.006957,/*Bracketed WZ:*/0.000000,0.049223),
				float4(/*XY: Non Bracketed*/-0.021449,0.040468,/*Bracketed WZ:*/0.018301,0.099929),
				float4(/*XY: Non Bracketed*/0.013015,0.050223,/*Bracketed WZ:*/0.054845,0.114689),
				float4(/*XY: Non Bracketed*/0.042178,0.038585,/*Bracketed WZ:*/0.085769,0.097080),
				float4(/*XY: Non Bracketed*/0.057972,0.019812,/*Bracketed WZ:*/0.102517,0.068674),
				float4(/*XY: Non Bracketed*/0.063647,0.005252,/*Bracketed WZ:*/0.108535,0.046643),
				float4(/*XY: Non Bracketed*/0.064754,0.000000,/*Bracketed WZ:*/0.109709,0.038697),
				float4(/*XY: Non Bracketed*/0.063647,0.005252,/*Bracketed WZ:*/0.108535,0.046643),
				float4(/*XY: Non Bracketed*/0.057972,0.019812,/*Bracketed WZ:*/0.102517,0.068674),
				float4(/*XY: Non Bracketed*/0.042178,0.038585,/*Bracketed WZ:*/0.085769,0.097080),
				float4(/*XY: Non Bracketed*/0.013015,0.050223,/*Bracketed WZ:*/0.054845,0.114689),
				float4(/*XY: Non Bracketed*/-0.021449,0.040468,/*Bracketed WZ:*/0.018301,0.099929),
				float4(/*XY: Non Bracketed*/-0.038708,0.006957,/*Bracketed WZ:*/0.000000,0.049223),
				float4(/*XY: Non Bracketed*/-0.020612,-0.025574,/*Bracketed WZ:*/0.019188,0.000000),
				float4(/*XY: Non Bracketed*/0.014096,-0.022658,/*Bracketed WZ:*/0.055991,0.004413)
		};
		static const float4 Kernel1BracketsRealXY_ImZW_2 = float4(0.000115,0.559524,0.000000,0.178226);
		static const float2 Kernel1Weights_RealX_ImY_2 = float2(0.513282,4.561110);
		static const float4 Kernel1_RealX_ImY_RealZ_ImW_2[] = {
				float4(/*XY: Non Bracketed*/0.000115,0.009116,/*Bracketed WZ:*/0.000000,0.051147),
				float4(/*XY: Non Bracketed*/0.005324,0.013416,/*Bracketed WZ:*/0.009311,0.075276),
				float4(/*XY: Non Bracketed*/0.013753,0.016519,/*Bracketed WZ:*/0.024376,0.092685),
				float4(/*XY: Non Bracketed*/0.024700,0.017215,/*Bracketed WZ:*/0.043940,0.096591),
				float4(/*XY: Non Bracketed*/0.036693,0.015064,/*Bracketed WZ:*/0.065375,0.084521),
				float4(/*XY: Non Bracketed*/0.047976,0.010684,/*Bracketed WZ:*/0.085539,0.059948),
				float4(/*XY: Non Bracketed*/0.057015,0.005570,/*Bracketed WZ:*/0.101695,0.031254),
				float4(/*XY: Non Bracketed*/0.062782,0.001529,/*Bracketed WZ:*/0.112002,0.008578),
				float4(/*XY: Non Bracketed*/0.064754,0.000000,/*Bracketed WZ:*/0.115526,0.000000),
				float4(/*XY: Non Bracketed*/0.062782,0.001529,/*Bracketed WZ:*/0.112002,0.008578),
				float4(/*XY: Non Bracketed*/0.057015,0.005570,/*Bracketed WZ:*/0.101695,0.031254),
				float4(/*XY: Non Bracketed*/0.047976,0.010684,/*Bracketed WZ:*/0.085539,0.059948),
				float4(/*XY: Non Bracketed*/0.036693,0.015064,/*Bracketed WZ:*/0.065375,0.084521),
				float4(/*XY: Non Bracketed*/0.024700,0.017215,/*Bracketed WZ:*/0.043940,0.096591),
				float4(/*XY: Non Bracketed*/0.013753,0.016519,/*Bracketed WZ:*/0.024376,0.092685),
				float4(/*XY: Non Bracketed*/0.005324,0.013416,/*Bracketed WZ:*/0.009311,0.075276),
				float4(/*XY: Non Bracketed*/0.000115,0.009116,/*Bracketed WZ:*/0.000000,0.051147)
		};

		// 1-Component (Near Field Blur Less Precise)
		static const float4 Kernel0BracketsRealXY_ImZW_1 = float4(-0.001442,0.672786,0.000000,0.311371);
		static const float2 Kernel0Weights_RealX_ImY_1 = float2(0.767583,1.862321);
		static const float4 Kernel0_RealX_ImY_RealZ_ImW_1[] = {
				float4(/*XY: Non Bracketed*/-0.001442,0.026656,/*Bracketed WZ:*/0.000000,0.085609),
				float4(/*XY: Non Bracketed*/0.010488,0.030945,/*Bracketed WZ:*/0.017733,0.099384),
				float4(/*XY: Non Bracketed*/0.023771,0.030830,/*Bracketed WZ:*/0.037475,0.099012),
				float4(/*XY: Non Bracketed*/0.036356,0.026770,/*Bracketed WZ:*/0.056181,0.085976),
				float4(/*XY: Non Bracketed*/0.046822,0.020140,/*Bracketed WZ:*/0.071737,0.064680),
				float4(/*XY: Non Bracketed*/0.054555,0.012687,/*Bracketed WZ:*/0.083231,0.040745),
				float4(/*XY: Non Bracketed*/0.059606,0.006074,/*Bracketed WZ:*/0.090738,0.019507),
				float4(/*XY: Non Bracketed*/0.062366,0.001584,/*Bracketed WZ:*/0.094841,0.005086),
				float4(/*XY: Non Bracketed*/0.063232,0.000000,/*Bracketed WZ:*/0.096128,0.000000),
				float4(/*XY: Non Bracketed*/0.062366,0.001584,/*Bracketed WZ:*/0.094841,0.005086),
				float4(/*XY: Non Bracketed*/0.059606,0.006074,/*Bracketed WZ:*/0.090738,0.019507),
				float4(/*XY: Non Bracketed*/0.054555,0.012687,/*Bracketed WZ:*/0.083231,0.040745),
				float4(/*XY: Non Bracketed*/0.046822,0.020140,/*Bracketed WZ:*/0.071737,0.064680),
				float4(/*XY: Non Bracketed*/0.036356,0.026770,/*Bracketed WZ:*/0.056181,0.085976),
				float4(/*XY: Non Bracketed*/0.023771,0.030830,/*Bracketed WZ:*/0.037475,0.099012),
				float4(/*XY: Non Bracketed*/0.010488,0.030945,/*Bracketed WZ:*/0.017733,0.099384),
				float4(/*XY: Non Bracketed*/-0.001442,0.026656,/*Bracketed WZ:*/0.000000,0.085609)
		};
		ENDHLSL
        Pass
        {
			Name "Horizontal"
           
		   	HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_horizontal

            float4 frag_horizontal (v2f i) : SV_Target
			{
				float4 col = float3(0.0f, 0.0f, 0.0f, 0.0f);
				float kernelSum = 0.0f;
	


				for (int x = 0; x <= 8 * 2; ++x)
				{
					int index = x - 8;
					float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
					
					float2 c0 = Kernel0_RealX_ImY_RealZ_ImW_2[index + 8].xy;
					float2 c1 = Kernel1_RealX_ImY_RealZ_ImW_2[index + 8].xy;

					float coc = _Spread;

					kernelSum += c0;
					float3 texel = tex2D(_MainTex, uv).rgb;

					// Apply kernel weights
					col.r += float4(texel.r * c0, texel.r * c1);
					col.g += float4(texel.g * c0, texel.g * c1);
					col.b += float4(texel.b * c0, texel.b * c1);
				}

				col /= kernelSum;
				return float4(col, 1.0f);

			// 	float3 col = float3(0.0f, 0.0f, 0.0f);
			// 	float kernelSum = 0.0f;

			// 	int upper = ((_KernelSize - 1) / 2);
			// 	int lower = -upper;

			// 	for (int x = lower; x <= upper; ++x)
			// 	{
			// 		float gauss = gaussian(x);
			// 		kernelSum += gauss;
			// 		float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
			// 		col += gauss * tex2D(_MainTex, uv).xyz;
			// 	}

			// 	col /= kernelSum;

			// 	return float4(col, 1.0f);
			}
            ENDHLSL
        }

		Pass
        {
			Name "Vertical"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_vertical

            float4 frag_vertical (v2f i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				// float kernelSum = 0.0f;

				// int upper = ((_KernelSize - 1) / 2);
				// int lower = -upper;

				// for (int y = lower; y <= upper; ++y)
				// {
				// 	float gauss = gaussian(y);
				// 	kernelSum += gauss;
				// 	float2 uv = i.uv + float2(0.0f, _MainTex_TexelSize.y * y);
				// 	col += gauss * tex2D(_MainTex, uv).xyz;
				// }

				col = tex2D(_MainTex, i.uv).xyz;
				return float4(col, 1.0f);
			}
            ENDHLSL
        }
    }
}
