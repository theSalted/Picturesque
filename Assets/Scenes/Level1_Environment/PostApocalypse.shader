Shader "Custom/ApocalypticSkybox"
{
    Properties
    {
        _TopColor ("Sky Top Color", Color) = (0.4, 0.4, 0.45, 1)
        _BottomColor ("Sky Bottom Color", Color) = (0.3, 0.3, 0.35, 1)
        _CloudTex ("Cloud Texture", 2D) = "white" {}
        _CloudSpeed ("Cloud Speed", Range(0, 1)) = 0.1
        _CloudDensity ("Cloud Density", Range(0, 1)) = 0.5
        _CloudDarkness ("Cloud Darkness", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        
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
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };
            
            fixed4 _TopColor;
            fixed4 _BottomColor;
            sampler2D _CloudTex;
            float _CloudSpeed;
            float _CloudDensity;
            float _CloudDarkness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = normalize(mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 基础天空渐变
                float skyGradient = i.worldPos.y * 0.5 + 0.5;
                fixed4 skyColor = lerp(_BottomColor, _TopColor, skyGradient);
                
                // 云层效果
                float2 cloudUV = i.uv + _Time.x * _CloudSpeed;
                fixed4 cloudTex = tex2D(_CloudTex, cloudUV);
                cloudTex += tex2D(_CloudTex, cloudUV * 1.5 + float2(0.1, 0.2));
                cloudTex *= _CloudDensity;
                
                // 暗化云层
                cloudTex = lerp(cloudTex, cloudTex * _CloudDarkness, cloudTex.r);
                
                // 混合天空和云
                return lerp(skyColor, cloudTex, cloudTex.a);
            }
            ENDCG
        }
    }
}