Shader "Custom/OculusHand"
{
    Properties
    {
        [Header(General)]
        // Ho cambiato i default in Ciano/Blu per darti la prova visiva che funziona
        _ColorTop("Color Top", Color) = (0.0, 0.8, 1.0, 1)
        _ColorBottom("Color Bottom", Color) = (0.0, 0.3, 0.8, 1)
        _Opacity("Opacity", Range(0 , 1)) = 0.85

        [Header(Fresnel)]
        // Alzato per rendere i bordi curvi 3D molto più visibili
        _FresnelPower("FresnelPower", Range(0 , 5)) = 1.5 

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0 , 0.01)) = 0.002
        _OutlineOpacity("Outline Opacity", Range(0 , 1)) = 0.8
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
        // PASS 1: OUTLINE
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vertOutline(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 extrudedPos = input.positionOS.xyz + (input.normalOS * _OutlineWidth);
                output.positionHCS = TransformObjectToHClip(extrudedPos);
                
                return output;
            }

            half4 fragOutline(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                return half4(_OutlineColor.rgb, _OutlineColor.a * _OutlineOpacity);
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // PASS 2: FILL (Colore interno + Fresnel)
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 normalOS    : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
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
                
                // Normali e Direzione per calcolare l'effetto di luce sui bordi curvi
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(posWS);
                
                return output;
            }

            half4 fragFill(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Mix tra il colore sopra e sotto
                half gradientFactor = saturate(input.normalOS.y * 0.5 + 0.5);
                half3 baseColor = lerp(_ColorBottom.rgb, _ColorTop.rgb, gradientFactor);

                // Calcolo luce sui bordi (Fresnel)
                float3 nWS = normalize(input.normalWS);
                float3 vWS = normalize(input.viewDirWS);
                half fresnel = pow(1.0 - saturate(dot(nWS, vWS)), _FresnelPower);
                
                // Aggiunge la luce di bordo al colore base
                baseColor += fresnel * 0.6; 

                return half4(baseColor, _Opacity);
            }
            ENDHLSL
        }
    }
}