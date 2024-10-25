Shader "FUI/ScreenSpaceOffset" {
    Properties{
        Thickness("Thickness", Float) = 1

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0


    }

        SubShader{
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

                

            struct VertexInput {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 normal : TEXCOORD0;
            };

            struct FragmentInput {
                float4 vertex   : SV_POSITION;
                float4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
            };


            float4 _ClipRect;
            float Thickness;

            FragmentInput vertexShader(VertexInput v) {
                FragmentInput output;

                output.worldPosition = v.vertex;
                output.vertex = UnityObjectToClipPos(v.vertex);

                float2 screenSize = _ScreenParams.xy;
                float2 invScreenSize = _ScreenParams.zw -1;

                float2 pixelPosition = output.vertex * screenSize;
                float2 pixelPositionOffseted = UnityObjectToClipPos(v.vertex + float4(v.normal,0,0)) * screenSize;
                float2 screenSpaceNormal = pixelPositionOffseted - pixelPosition;
                float d = length(screenSpaceNormal);

                output.vertex.xy += normalize(screenSpaceNormal) * Thickness * invScreenSize;

                output.color = v.color;
                return output;
            }



            float4 fragmentShader(FragmentInput input) : SV_Target {

                float4 color = input.color;

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