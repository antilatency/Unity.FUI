Shader "FUI/Image" {
    Properties {
        Texture("Texture", 2D) = "gray" {}
        Multiplier("Multiplier", Vector) = (1,1,1,1)
        Increment("Increment", Vector) = (0,0,0,0)

        [Toggle(TO_SRGB)] TO_SRGB("TO_SRGB", Float) = 0

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
            
        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

        
    }
        
    SubShader {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vertexShader
            #pragma fragment fragmentShader
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #pragma multi_compile_local _ TO_SRGB

            struct VertexInput {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct FragmentInput {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

                
            float4 _ClipRect;

            FragmentInput vertexShader(VertexInput v){
                FragmentInput output;

                output.worldPosition = v.vertex;
                output.vertex = UnityObjectToClipPos(output.worldPosition);
                output.texcoord = v.texcoord;// TRANSFORM_TEX(, _MainTex);
                output.color = v.color;
                return output;
            }

            //#include "PS_Image_ToSRGB_IgnoreAlpha.cginc"

            float4 Multiplier;
            float4 Increment;
            sampler2D Texture;

#ifdef TO_SRGB
            float3 ToSRGB(float3 x) {
                return lerp(12.92 * x, 1.055 * pow(x, 1.0 / 2.4) - 0.055, x > 0.0031308);
            }
#endif


            float4 fragmentShader(FragmentInput input) : SV_Target {

                float4 color = tex2D(Texture, input.texcoord) * input.color;

                color = Multiplier * color + Increment;

#ifdef TO_SRGB
                color.rgb = ToSRGB(color.rgb);
#endif

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(input.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}