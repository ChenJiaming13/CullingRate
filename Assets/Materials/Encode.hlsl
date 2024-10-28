#ifndef INCLUDE_ENCODE_HLSL
#define INCLUDE_ENCODE_HLSL

float3 EncodeUintToFloat3(uint value)
{
    float x = (value & 0xFF) / 255.0;          // 取低8位
    float y = ((value >> 8) & 0xFF) / 255.0;   // 中间8位
    float z = ((value >> 16) & 0xFF) / 255.0;  // 高8位
    return float3(x, y, z);
}

uint DecodeFloat3ToUint(float3 encodedValue)
{
    uint x = (uint)(encodedValue.x * 255.0) & 0xFF;
    uint y = ((uint)(encodedValue.y * 255.0) & 0xFF) << 8;
    uint z = ((uint)(encodedValue.z * 255.0) & 0xFF) << 16;
    return x | y | z;
}

#endif