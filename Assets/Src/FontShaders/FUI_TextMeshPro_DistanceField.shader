Shader "FUI/TextMeshPro/Distance Field" {

Properties {

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0
	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5
	_Sharpness			("Sharpness", Range(-1,1)) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)

	

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15

	//unused properties 
	_OutlineWidth		("Outline Width", float) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0
}

SubShader {

	Tags
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	//Blend Off
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		//#pragma target 3.0
		#pragma vertex VertShader
		#pragma fragment PixShader
		/*#pragma shader_feature __ BEVEL_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
		#pragma shader_feature __ GLOW_ON*/

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		//#include "Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc"
		//#include "Assets/TextMesh Pro/Shaders/TMPro.cginc"

		struct vertex_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4	position		: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};


		struct pixel_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4	position		: SV_POSITION;
			float4	localPosition	: LOCAL_POSITION;
			fixed4	color			: COLOR;
			float2	atlas			: TEXCOORD0;		// Atlas
			float4	param			: TEXCOORD1;		// alphaClip, scale, bias, weight
		};

		float _GradientScale;
		float _FaceDilate;
		float _WeightNormal;
		float _WeightBold;
		float _Sharpness;
		float _ScaleRatioA;
		float4 _ClipRect;

		sampler2D _MainTex;
		float _TextureWidth;
		float _TextureHeight;

		pixel_t VertShader(vertex_t input) {
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input,output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float bold = step(input.texcoord1.y, 0);

			output.localPosition = input.position;

			float4 vert = input.position;


			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
			//if (UNITY_MATRIX_P[3][3] == 0)
			//	scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			//output.debug = abs(input.texcoord1.y);

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float bias =(.5 - weight) + (.5 / scale);

			//float alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _OutlineSoftness * _ScaleRatioA);
			//alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;



			// Generate UV for the Masking Texture
			//float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			//float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Support for texture tiling and offset
			//float2 textureUV = UnpackUV(input.texcoord1.x);



			output.position = vPosition;
			output.color = input.color;
			output.atlas =	input.texcoord0;
			output.param =	float4(0, scale, bias, 0);




			return output;
		}

		#ifndef UNITY_COLORSPACE_GAMMA
		float destFactorApproximation(float b, float t) {
			return 1.0 - pow(t, 0.763095763943736 + -0.0940928392987432*b + 2.42378703711849*b*b + -0.889566853428863*t + 1.07550069171562*b*t + 0.17420813332235*t*t);
		}

		float3 sourceFactorApproximation(float3 b, float t) {
			return pow(t, 1.36351317335054 + 1.08300955379772*b + -0.721343609284074*b*b + 0.0263642538421258*t + 0.837532624736851*b*t + -0.499750802593315*t*t);
		}		
		#endif

		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);


			float	scale	= input.param.y;
			float	bias	= input.param.z;
			//float	weight	= input.param.w;


			float c = tex2D(_MainTex, input.atlas).a;

			
			float sd = (bias - c) * scale;
			float alpha = saturate(1-sd) * input.color.a;

			#if UNITY_UI_CLIP_RECT
			alpha *= UnityGet2DClipping(input.localPosition.xy, _ClipRect);
			#endif

			#if UNITY_UI_ALPHACLIP
				clip(alpha - 0.001);
			#endif

			float3 color = input.color.rgb;
			float3 sourceColor = color * alpha;
			float oneMinusDestFactor = alpha;

			#ifndef UNITY_COLORSPACE_GAMMA

				float luma = dot(color, float3(0.2126, 0.7152, 0.0722));
				oneMinusDestFactor = 1 - destFactorApproximation(luma, alpha);
				oneMinusDestFactor = saturate(oneMinusDestFactor);
				sourceColor = color * sourceFactorApproximation(color, alpha);
				sourceColor = saturate(sourceColor);
			#endif

			return float4(sourceColor, oneMinusDestFactor);
		}

		ENDCG
	}
}

Fallback "TextMeshPro/Mobile/Distance Field"
//CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
