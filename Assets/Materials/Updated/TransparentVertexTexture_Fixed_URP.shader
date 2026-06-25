Shader "Unlit/TransparentVertexTexture_Fixed_URP"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", COLOR) = (1,1,1,1)
        _FadeLimit("Fade Limit", VECTOR) = (0,0,1,1)
        _FadeSign("Fade Sign", Range(-1,1)) = 1
        _Fade("Fade", Range(0,1)) = 1
        _Highlight("Highlight Strength", Range(0,1)) = 0
        _HighlightColor("Highlight Color", COLOR) = (1,1,1,1)

        _OffsetFactor("Offset Factor", float) = 0
        _OffsetUnits("Offset Units", float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" // Definisce che è uno shader URP
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Unlit"
            
            // Il comando Offset funziona ancora perfettamente in ShaderLab
            Offset[_OffsetFactor],[_OffsetUnits]
            
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            
            // Inclusione della libreria core di URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : TEXCOORD1;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // In URP le texture e i sampler si dichiarano con macro specifiche
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // CBUFFER per l'ottimizzazione SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST; // Dichiarata di default per buona pratica, anche se non usi tiling/offset
                half4 _Color;
                half4 _FadeLimit;
                half _FadeSign;
                half _Fade;
                half _Highlight;
                half4 _HighlightColor;
                
                // Manteniamo questi float nel CBUFFER per evitare warning di Unity,
                // anche se vengono usati solo nel blocco Offset di ShaderLab
                float _OffsetFactor;
                float _OffsetUnits;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Nuova funzione URP per lo spazio clip
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = lerp(input.color * _Color, _HighlightColor, _Highlight);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Lettura della texture tramite macro URP
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 finalColor = texColor * input.color;
                
                half lowLimit = smoothstep(_FadeLimit.x, _FadeLimit.y, input.uv.y);
                half highLimit = smoothstep(_FadeLimit.z, _FadeLimit.w, input.uv.y);
                
                finalColor.a *= saturate(lowLimit - _FadeSign * highLimit) * _Fade;

                return finalColor;
            }
            ENDHLSL
        }
    }
}