#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    StructuredBuffer<float3> _Position;
#endif
float2 _Scale;
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