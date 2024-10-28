Shader "Custom/Lit"
{
    Properties
    {
        _MyObjectId ("My Object ID", Int) = 0
        _Color ("Color", Color) = (1, 1, 1, 1)
        _LightIntensity ("Light Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normal : NORMAL;
            };

            float4 _Color;
            float _LightIntensity;
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.normal = TransformObjectToWorldNormal(IN.normal);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float4 diffuse = max(0.1, dot(normalize(IN.normal), _MainLightPosition.xyz))
                    * _Color * _MainLightColor.rgba * _LightIntensity;
                return diffuse;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "StatObjectRenderPass" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Encode.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _Color;
            int _MyObjectId;
            int _MyObjectCount;
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                return OUT;
            }

            float4 frag (Varyings IN, uint triangle_id : SV_PrimitiveID) : SV_Target
            {
                return float4(_MyObjectId * 1.0 / _MyObjectCount, EncodeUintToFloat3(triangle_id).xyz);
            }
            ENDHLSL
        }
    }
}
