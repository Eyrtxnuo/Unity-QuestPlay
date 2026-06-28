/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.
[... Original Meta License Header ...]
************************************************************************************/

Shader "Custom/OculusRayCursor"
{
    Properties
    {
        _Radius("Radius", FLOAT) = 0.5
        _OutlineThickness("Outline Thickness", FLOAT) = 0.05
        _Color("Inner Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags
        { 
            "RenderType" = "Transparent"  
            "Queue" = "Transparent+10" 
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Cull Off
        ZTest LEqual
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Offset -5, -5
        
        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Core.hlsl sostituisce UnityCG.cginc in URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // SRP Batcher: racchiudiamo le variabili per migliorare le performance in VR
            CBUFFER_START(UnityPerMaterial)
                float _Radius;
                float _OutlineThickness;
                half4 _Color;
                half4 _OutlineColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Inizializzazione istanze VR
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                // Conversione da spazio oggetto a clip per la URP
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Setup dell'occhio corrente per la VR (Fondamentale per vederlo in entrambi gli occhi)
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Calcolo della distanza dal centro (per fare il cerchio)
                float dist = distance(input.uv, float2(0.5, 0.5));
                
                // Derivate parziali per calcolare l'anti-aliasing sui bordi del cerchio
                float2 ddDist = float2(ddx(dist), ddy(dist));
                float ddDistLen = rcp(length(ddDist));

                half4 innerColor = _Color;
                half4 borderColor = _OutlineColor;

                float outerRadius = _Radius;
                float innerRadius = _OutlineThickness - _Radius;

                // Calcoli per il bordo esterno
                float outerDist = dist - outerRadius;
                float outerDistOverLen = outerDist * ddDistLen;

                // Calcoli per il bordo interno (outline)
                float innerDist = dist + innerRadius;
                float innerDistOverLen = innerDist * ddDistLen;
                
                // Misto colore interno / colore contorno
                float colorLerpParam = saturate(innerDistOverLen);
                half4 fragColor = lerp(innerColor, borderColor, colorLerpParam);
                
                // Maschera alpha per tagliare l'esterno del cerchio con un bordo morbido
                fragColor.a *= (1.0 - saturate(outerDistOverLen));
                
                return fragColor;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Unlit"
}