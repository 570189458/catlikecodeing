﻿Shader "Graph/Point Surface GPU"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        CGPROGRAM 

        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma target 4.5

        float _Smoothness;
        float _Step;
        float2 _Scale;
        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<float3> _Position;
        #endif
        struct Input
        {
            float3 worldPos;
        };
        
        void ConfigureProcedural()
        {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float3 position = _Position[unity_InstanceID];
                unity_ObjectToWorld = 0.0;
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
                unity_ObjectToWorld._m00_m11_m22 = 1.0 / _Scale.x;
                
                unity_ObjectToWorld = 0.0;
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(-position, 1.0);
                unity_ObjectToWorld._m00_m11_m22 = 1.0 / _Scale.y;
            #endif
        }

        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface)
        {
            surface.Albedo.rg = saturate(input.worldPos.xy * 0.5 + 0.5);
            surface.Smoothness = _Smoothness;
        }

        ENDCG
    }

    Fallback "Diffuse"
}
