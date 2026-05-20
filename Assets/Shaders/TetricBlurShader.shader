Shader "Custom/TetricBlurShader"
{
    Properties
    {
        _Intensity ("Blur Intensity", Float) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            Name "TetricBlurPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D_X(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);
            float4 _CameraColorTexture_TexelSize;
            float _Intensity;

            half4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;
                float2 offset = _CameraColorTexture_TexelSize.xy * _Intensity * 4.0;

                // Box blur con 5 samples
                float4 color = SAMPLE_TEXTURE2D_X(_CameraColorTexture, sampler_CameraColorTexture, uv) * 0.2;
                color += SAMPLE_TEXTURE2D_X(_CameraColorTexture, sampler_CameraColorTexture, uv + float2(offset.x, 0.0)) * 0.2;
                color += SAMPLE_TEXTURE2D_X(_CameraColorTexture, sampler_CameraColorTexture, uv - float2(offset.x, 0.0)) * 0.2;
                color += SAMPLE_TEXTURE2D_X(_CameraColorTexture, sampler_CameraColorTexture, uv + float2(0.0, offset.y)) * 0.2;
                color += SAMPLE_TEXTURE2D_X(_CameraColorTexture, sampler_CameraColorTexture, uv - float2(0.0, offset.y)) * 0.2;

                return color;
            }
            ENDHLSL
        }
    }
}
