Shader "Custom/OculusHand2"
{
    Properties
    {
        [Header(General)]
        _ColorTop("Color Top", Color) = (0.196, 0.203, 0.211, 1)
        _ColorBottom("Color Bottom", Color) = (0.121, 0.125, 0.129, 1)
        _Opacity("Opacity", Range(0 , 1)) = 0.8

        [Header(Fresnel)]
        _FresnelPower("FresnelPower", Range(0 , 5)) = 0.16

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (0.537, 0.537, 0.537, 1)
        _OutlineWidth("Outline Width", Range(0 , 0.005)) = 0.00134
        _OutlineOpacity("Outline Opacity", Range(0 , 1)) = 0.4
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            half4 _ColorTop;
            half4 _ColorBottom;
            half _Opacity;
            half _FresnelPower;
            half4 _OutlineColor;
            half _OutlineWidth;
            half _OutlineOpacity;
        CBUFFER_END
        ENDHLSL

        // ------------------------------------------------------------------
        // PASS 1: DEPTH PRE-PASS (Blocca l'effetto Raggi-X)
        // ------------------------------------------------------------------
        Pass
        {
            Name "HandDepthPrime"
            Tags { "LightMode" = "SRPDefaultUnlit" } 

            ZWrite On
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex vertDepth
            #pragma fragment fragDepth

            struct Attributes {
                float4 positionOS : POSITION;
                float4 color      : COLOR; // Legge i vertex color di Meta
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float  alpha       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vertDepth(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.alpha = input.color.a; 
                return output;
            }

            half4 fragDepth(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Se l'alpha del polso scende vicino a 0, buca il mantello dell'invisibilità
                clip(input.alpha - 0.1); 
                
                return half4(0,0,0,0);
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // PASS 2: OUTLINE
        // ------------------------------------------------------------------
        Pass
        {
            Name "HandOutlineURP"
            Tags { "LightMode" = "UniversalForwardOnly" }

            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float  alpha       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vertOutline(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Moltiplica lo spessore dell'outline per l'alpha dei vertici
                // Così il cerchio al polso viene letteralmente schiacciato a zero!
                float width = _OutlineWidth * input.color.a;
                float3 extrudedPos = input.positionOS.xyz + (input.normalOS * width);
                
                output.positionHCS = TransformObjectToHClip(extrudedPos);
                output.alpha = input.color.a;
                
                return output;
            }

            half4 fragOutline(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                return half4(_OutlineColor.rgb, _OutlineColor.a * _OutlineOpacity * input.alpha);
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // PASS 3: FILL
        // ------------------------------------------------------------------
        Pass
        {
            Name "HandFillURP"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertFill
            #pragma fragment fragFill

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 normalOS    : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
                float  alpha       : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vertFill(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalOS = input.normalOS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(posWS);
                
                output.alpha = input.color.a; // Trascina l'alpha dei vertici al fragment
                return output;
            }

            half4 fragFill(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half gradientFactor = saturate(input.normalOS.y * 0.5 + 0.5);
                half3 baseColor = lerp(_ColorBottom.rgb, _ColorTop.rgb, gradientFactor);

                float3 nWS = normalize(input.normalWS);
                float3 vWS = normalize(input.viewDirWS);
                half fresnel = pow(1.0 - saturate(dot(nWS, vWS)), _FresnelPower);
                
                baseColor += fresnel * 0.35;

                // Moltiplica l'opacità globale per la sfumatura invisibile del polso
                half finalAlpha = _Opacity * input.alpha;

                return half4(baseColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}