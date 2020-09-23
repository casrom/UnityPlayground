#ifndef MYRP_UNLIT_INCLUDED
#define MYRP_UNLIT_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

CBUFFER_START(UnityPerFrame)
float4x4 unity_MatrixVP;
CBUFFER_END

CBUFFER_START(UnityPerDraw)
float4x4 unity_ObjectToWorld;
CBUFFER_END

struct Triangle
{
    float3 vertices[3];
};
StructuredBuffer<Triangle> _triangles;
StructuredBuffer<float4> _data;


struct VertexInput {
    float4 pos : POSITION;
};

struct VertexOutput {
    float4 clipPos : SV_Position;
};

struct v2f {
    float4 pos : SV_POSITION;
    uint id : custom0;
    float weight : custom1;
};

float4 ObjectToClipPos(float4 pos)
{
    float4 worldPos = mul(unity_ObjectToWorld, float4(pos.xyz, 1.0));
    return mul(unity_MatrixVP, worldPos);
}
float _isolevel;

v2f vert(float4 vertex:POSITION, uint iid : SV_InstanceID)
{
    v2f o = (v2f) 0;
    if (_data[iid].w < _isolevel)
    {
        vertex.xyz;
        vertex.xyz += _data[iid].xyz;
        o.pos = ObjectToClipPos(vertex);
        o.id = iid;
        o.weight = _data[iid].w;
    }
    return o;
}


float4 frag(v2f input) : SV_TARGET
{
    float k = frac(sin(input.weight) * 43758.5453123);
    return float4(k, 0, 0, 1);

}

#endif // MYRP_UNLIT_INCLUDED