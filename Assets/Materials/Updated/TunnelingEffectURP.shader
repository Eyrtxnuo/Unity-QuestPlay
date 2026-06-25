Shader "Unlit/TunnelingEffectURP"
{
    Properties
    {
       _ColorInner("Inner Color", COLOR) = (1,0,0,1)
       _ColorOuter("Outer Color", COLOR) = (0,0,0,1)
       _Direction("Direction", VECTOR) = (0,0,1,0)
       _MinRadius("Min Radius", Range(-1,1)) = 0
       _MaxRadius("Max Radius", Range(-1,1)) = 0
       _Alpha("Fade", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
           "Queue"="Transparent"
           "IgnoreProjector"="True"
           "RenderType"="Transparent"
           "RenderPipeline"="UniversalPipeline" // Definisce che è uno shader URP
        }

        Cull Back
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha, OneMinusDstAlpha One
        
        LOD 100
    
        Pass
        {
            Name "Unlit"
            
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            // Inclusione della libreria core di URP al posto di UnityCG.cginc
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION; // object space
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION; // clip space
                float3 positionWS   : TEXCOORD0;   // world space
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Il CBUFFER è fondamentale in URP per supportare lo SRP Batcher (ottimizzazione draw calls)
            CBUFFER_START(UnityPerMaterial)
                half4 _ColorInner;
                half4 _ColorOuter;
                float3 _Direction;
                half _MinRadius;
                half _MaxRadius;
                half _Alpha;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Nuove funzioni URP per le trasformazioni di spazio
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // GetCameraPositionWS() sostituisce _WorldSpaceCameraPos
                float3 viewDir = normalize(input.positionWS - GetCameraPositionWS());
                float fov = dot(_Direction, viewDir);
                
                float dirMask = saturate((fov - _MinRadius) / (_MaxRadius - _MinRadius));

                half4 color = lerp(_ColorInner, _ColorOuter, fov * 0.5 + 0.5);
                dirMask = smoothstep(0, 1, dirMask);
                
                return half4(color.xyz, color.w * dirMask * _Alpha);
            }
            ENDHLSL
        }
    }
}