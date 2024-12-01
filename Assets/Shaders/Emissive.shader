Shader "Custom/EdgeGlow"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _GlowColor("Glow Color", Color) = (1,0,0,1)
        _GlowPower("Glow Power", Range(0.5, 8.0)) = 2.0
        _RimWidth("Rim Width", Range(0.1, 8.0)) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 viewDir : TEXCOORD1;
                    float3 normalDir : TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float4 _GlowColor;
                float _GlowPower;
                float _RimWidth;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    // 计算视角方向和法线方向
                    o.normalDir = UnityObjectToWorldNormal(v.normal);
                    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
                    o.viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // 计算边缘发光
                    float3 normalDirection = normalize(i.normalDir);
                    float3 viewDirection = normalize(i.viewDir);

                    float rimDot = 1 - saturate(dot(viewDirection, normalDirection));
                    float rimIntensity = pow(rimDot, _RimWidth);
                    float glow = smoothstep(0, 1, rimIntensity) * _GlowPower;

                    // 采样主纹理
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                    // 混合发光效果
                    fixed4 finalColor = col + _GlowColor * glow;

                    return finalColor;
                }
                ENDCG
            }
        }
}