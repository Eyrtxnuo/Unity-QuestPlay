/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.
[... Original Meta License Header ...]
************************************************************************************/

Shader "Custom/OculusRayVisual"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Feather ("Feather", Float) = 4
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Core.hlsl sostituisce UnityCG.cginc in URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Mettere le variabili in un CBUFFER abilita l'SRP Batcher (fondamentale per le performance in VR)
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Feather;
            CBUFFER_END

            struct Attributes 
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID // Per la VR (Single Pass)
            };

            struct Varyings 
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // Passiamo l'ID al fragment shader
                UNITY_VERTEX_OUTPUT_STEREO     // Per renderizzare in entrambi gli occhi
            };

            Varyings vert(Attributes input) 
            {
                Varyings output;
                
                // Inizializzazione istanze VR
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Nuova funzione URP per convertire lo spazio oggetto in spazio clip
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                
                // Mantiene la logica originale di usare la Z del vertice come UV
                output.uv = input.positionOS.zz + 0.5;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target 
            {
                // Setup dell'occhio corrente per la VR
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 col = _Color;
                
                // Calcola l'effetto di dissolvenza (feather) basato sulla coordinata X calcolata nel vertice
                col.a = saturate(1.0 - _Feather * (input.uv.x - 0.5) * (input.uv.x - 0.5));
                
                return col;
            }
            ENDHLSL
        }
    }
}