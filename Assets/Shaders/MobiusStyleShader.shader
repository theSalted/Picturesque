Shader "Custom/MoebiusGradientShader"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _Brightness("Brightness", Range(0, 2)) = 1.0
        _Contrast("Contrast", Range(0, 2)) = 1.0
        _NumColors("Number of Colors", Range(2, 10)) = 4
        _GradientWidth("Gradient Width", Range(0.0, 1.0)) = 0.2

        _EdgeColor("Edge Color", Color) = (0, 0, 0, 1)
        _EdgeSensitivity("Edge Sensitivity", Range(0, 10)) = 1.0

        _DotSize("Dot Size", Range(0.01, 0.5)) = 0.05
        _DotFrequency("Dot Frequency", Range(1, 500)) = 10.0
        _DotColor("Dot Color", Color) = (0, 0, 0, 1)
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Ensure shadow-related keywords are included
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            // Include necessary shader libraries
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
                float3 worldPosWS : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4; // For shadow mapping
            };

            // Texture and sampler declarations
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float _Brightness;
            float _Contrast;
            float _NumColors;
            float _GradientWidth;

            float4 _EdgeColor;
            float _EdgeSensitivity;

            float _DotSize;
            float _DotFrequency;
            float4 _DotColor;
            float _ShadowThreshold;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetCameraPositionWS() - worldPos;
                OUT.worldPosWS = worldPos;

                // Calculate shadow coordinates
                #if defined(_MAIN_LIGHT_SHADOWS)
                    OUT.shadowCoord = GetMainLightShadowCoord(worldPos);
                #endif

                return OUT;
            }

            float4 ApplyBrightnessContrast(float4 color)
            {
                color.rgb = (color.rgb - 0.5) * _Contrast + 0.5;
                color.rgb *= _Brightness;
                return color;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Sample the base texture
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // Apply brightness and contrast adjustments
                baseColor = ApplyBrightnessContrast(baseColor);

                // Normalize vectors
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);

                // Calculate main light
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(-mainLight.direction);
                float NdotL = saturate(dot(normalWS, lightDir));

                // Compute shadow attenuation
                float shadowAttenuation = 1.0;

                #if defined(_MAIN_LIGHT_SHADOWS)
                    shadowAttenuation = MainLightRealtimeShadow(IN.shadowCoord);
                #endif

                // Adjust NdotL by shadow attenuation
                float shadowedNdotL = NdotL * shadowAttenuation;

                // Edge detection
                float edge = dot(normalWS, viewDirWS);
                edge = pow(edge, _EdgeSensitivity);
                float edgeFactor = step(0.1, edge);

                // Quantize colors to create limited color palette effect with gradients
                float numColors = _NumColors;
                float gradientWidth = _GradientWidth;

                float3 colorSteps = floor(baseColor.rgb * numColors);
                float3 nextColorSteps = min(colorSteps + 1.0, numColors);

                float3 quantizedColor = colorSteps / numColors;
                float3 nextQuantizedColor = nextColorSteps / numColors;

                // Calculate the fractional part for smooth interpolation
                float3 fracColor = frac(baseColor.rgb * numColors);

                // Apply gradient within each cel
                float3 smoothColor = lerp(quantizedColor, nextQuantizedColor, smoothstep(0.5 - gradientWidth, 0.5 + gradientWidth, fracColor));

                // Dot pattern in shadowed areas
                float shadowFactor = 1.0 - shadowedNdotL;
                float dotPattern = 0.0;
                if (shadowFactor > _ShadowThreshold)
                {
                    // Adjust dot pattern to use UV coordinates
                    float2 dotUV = IN.uv * _DotFrequency;
                    float2 pattern = frac(dotUV) - 0.5;
                    float circle = step(length(pattern), _DotSize);
                    dotPattern = circle;
                }

                // Combine base color with dot pattern
                float4 dotColor = lerp(float4(smoothColor, 1.0), _DotColor, dotPattern);

                // Apply edge color
                float4 finalColor = lerp(dotColor, _EdgeColor, 1.0 - edgeFactor);

                finalColor.a = 1.0; // Ensure opacity
                return finalColor;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Lit"

}