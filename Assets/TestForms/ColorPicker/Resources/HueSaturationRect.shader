Shader "UI/HueSaturationRect"
{
    Properties
    {


        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
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
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };


                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = v.texcoord;
                    OUT.color = v.color;
                    return OUT;
                }

                float3 HSVtoRGB(float3 HSV)
                {
                    float3 RGB = 0;
                    float C = HSV.z * HSV.y;
                    float H = HSV.x * 6;
                    float X = C * (1 - abs(fmod(H, 2) - 1));
                    if (HSV.y != 0)
                    {
                        float I = floor(H);
                        if (I == 0) { RGB = float3(C, X, 0); }
                        else if (I == 1) { RGB = float3(X, C, 0); }
                        else if (I == 2) { RGB = float3(0, C, X); }
                        else if (I == 3) { RGB = float3(0, X, C); }
                        else if (I == 4) { RGB = float3(X, 0, C); }
                        else { RGB = float3(C, 0, X); }
                    }
                    float M = HSV.z - C;
                    return RGB + M;
                }


                float4 frag(v2f IN) : SV_Target{
                    float4 color = float4(HSVtoRGB(float3(IN.texcoord,1)),1);

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #endif

                    return color;
                }
            ENDCG
            }
        }
}