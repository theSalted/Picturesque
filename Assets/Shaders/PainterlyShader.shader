Shader "Custom/PainterlyOilPaintingShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BrushNormalMap ("Brush Normal Map", 2D) = "bump" {}
        _BrushMask ("Brush Mask", 2D) = "white" {}
        _CanvasTex ("Canvas Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _BrushStrength ("Brush Strength", Range(0,1)) = 0.5
        _CanvasStrength ("Canvas Strength", Range(0,1)) = 0.5
        _ColorVariation ("Color Variation", Range(0,1)) = 0.1
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Include URP shader libraries
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Define shader model
            #pragma target 4.5

            // Uniforms
            CBUFFER_START(UnityPerMaterial)
                float4 _SpecularColor;
                half _Smoothness;
                half _Metallic;
                half _BrushStrength;
                half _CanvasStrength;
                half _ColorVariation;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_BrushNormalMap);
            SAMPLER(sampler_BrushNormalMap);

            TEXTURE2D(_BrushMask);
            SAMPLER(sampler_BrushMask);

            TEXTURE2D(_CanvasTex);
            SAMPLER(sampler_CanvasTex);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            // Vertex Input Structure
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                float4 tangentOS  : TANGENT;
            };

            // Vertex Output Structure
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 tangentWS  : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float3 viewDirWS  : TEXCOORD4;
                float3 positionWS : TEXCOORD5;
                #ifdef _MAIN_LIGHT_SHADOWS
                    float4 shadowCoord  : TEXCOORD6;
                #endif
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.uv = IN.uv;
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.tangentWS = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangentOS.w;
                OUT.viewDirWS = normalize(GetCameraPositionWS() - positionWS);
                OUT.positionWS = positionWS;

                // Initialize shadow coordinates
                #ifdef _MAIN_LIGHT_SHADOWS
                    OUT.shadowCoord = GetMainLightShadowCoord(positionWS);
                #endif

                return OUT;
            }

            // Helper function to create TBN matrix
            float3x3 CreateTBN(float3 normalWS, float3 tangentWS, float3 bitangentWS)
            {
                return float3x3(tangentWS, bitangentWS, normalWS);
            }

            // Fragment Shader
            float4 frag(Varyings IN) : SV_Target
            {
                // Sample albedo texture
                float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;

                // Sample noise texture for color variation
                float3 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv * 10).rgb; // Adjust scale as needed
                albedo += (noise - 0.5) * _ColorVariation;

                // Sample canvas texture
                float3 canvasTex = SAMPLE_TEXTURE2D(_CanvasTex, sampler_CanvasTex, IN.uv * 5).rgb; // Adjust scale as needed

                // Sample brush mask
                float brushMask = SAMPLE_TEXTURE2D(_BrushMask, sampler_BrushMask, IN.uv * 5).r; // Adjust scale as needed

                // Calculate normal from brush normal map
                float3 normalMap = SAMPLE_TEXTURE2D(_BrushNormalMap, sampler_BrushNormalMap, IN.uv * 5).rgb * 2 - 1;
                float3x3 TBN = CreateTBN(IN.normalWS, IN.tangentWS, IN.bitangentWS);
                float3 normalWS = normalize(mul(normalMap * _BrushStrength, TBN) + IN.normalWS * (1 - _BrushStrength));

                // Get lighting information
                float3 viewDir = normalize(IN.viewDirWS);

                // Main light
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 lightColor = mainLight.color;

                // Calculate shadow attenuation
                float shadowAtten = 1.0;
                #ifdef _MAIN_LIGHT_SHADOWS
                    shadowAtten = SampleMainLightShadowmap(IN.shadowCoord);
                #endif

                // Lambertian diffuse
                float NdotL = saturate(dot(normalWS, lightDir));
                float3 diffuse = albedo * NdotL * lightColor * shadowAtten;

                // Specular (adjusted for painterly effect)
                float3 halfVector = normalize(lightDir + viewDir);
                float NdotH = saturate(dot(normalWS, halfVector));
                float specular = pow(NdotH, _Smoothness * 128) * _Metallic;

                // Combine diffuse and specular
                float3 color = diffuse + specular * _SpecularColor.rgb * lightColor;

                // Blend in canvas texture where paint is thin
                color = lerp(color, canvasTex, (1 - brushMask) * _CanvasStrength);

                // Apply gamma correction (if needed)
                color = pow(color, 1.0 / 2.2);

                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}