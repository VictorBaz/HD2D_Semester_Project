Shader "Custom/HLSL_Grass"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _TipColor("Tip Color", Color) = (1, 1, 1, 1)
        _BladeTexture("Blade Texture", 2D) = "white" {}

        _BladeWidthMin("Blade Width (Min)", Range(0, 0.1)) = 0.02
        _BladeWidthMax("Blade Width (Max)", Range(0, 0.1)) = 0.05
        _BladeHeightMin("Blade Height (Min)", Range(0, 2)) = 0.1
        _BladeHeightMax("Blade Height (Max)", Range(0, 2)) = 0.2

        _BladeSegments("Blade Segments", Range(1, 10)) = 3
        _BladeBendDistance("Blade Forward Amount", Float) = 0.38
        _BladeBendCurve("Blade Curvature Amount", Range(1, 4)) = 2
        _BendDelta("Bend Variation", Range(0, 1)) = 0.2

        _TessellationGrassDistance("Tessellation Grass Distance", Range(0.01, 2)) = 0.1

        _GrassMap("Grass Visibility Map", 2D) = "white" {}
        _GrassThreshold("Grass Visibility Threshold", Range(-0.1, 1)) = 0.5
        _GrassFalloff("Grass Visibility Fade-In Falloff", Range(0, 0.5)) = 0.05

        _WindMap("Wind Offset Map", 2D) = "bump" {}
        _WindVelocity("Wind Velocity", Vector) = (1, 0, 0, 0)
        _WindFrequency("Wind Pulse Frequency", Range(0, 1)) = 0.01

        _PositionCount("Interactor Count", Range(0, 100)) = 0
        _Radius("Interactor Radius", Range(0, 5)) = 1
        _MaxWidth("Max Displacement Width", Range(0, 2)) = 0.1

    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        Cull Off

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            #define UNITY_PI 3.14159265359f
            #define UNITY_TWO_PI 6.28318530718f
            #define BLADE_SEGMENTS 4
            #define MAX_POSITIONS 100

            TEXTURE2D(_BladeTexture);  SAMPLER(sampler_BladeTexture);
            TEXTURE2D(_GrassMap);      SAMPLER(sampler_GrassMap);
            TEXTURE2D(_WindMap);       SAMPLER(sampler_WindMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _TipColor;

                float _BladeWidthMin;
                float _BladeWidthMax;
                float _BladeHeightMin;
                float _BladeHeightMax;

                float _BladeBendDistance;
                float _BladeBendCurve;

                float _BendDelta;

                float _TessellationGrassDistance;

                float4 _GrassMap_ST;
                float  _GrassThreshold;
                float  _GrassFalloff;

                float4 _WindMap_ST;
                float4 _WindVelocity;
                float  _WindFrequency;

                float _PositionCount;
                float _Radius;
                float _MaxWidth;
            CBUFFER_END

            
            float4 _Positions[MAX_POSITIONS];

            struct VertexInput
            {
                float4 vertex  : POSITION;
                float3 normal  : NORMAL;
                float4 tangent : TANGENT;
                float2 uv      : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 vertex  : POSITION;
                float3 normal  : NORMAL;
                float4 tangent : TANGENT;
                float2 uv      : TEXCOORD0;
            };

            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside  : SV_InsideTessFactor;
            };

            struct GeomData
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float  circle   : TEXCOORD2; 
            };

            float rand(float3 co)
            {
                return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
            }

            float3x3 angleAxis3x3(float angle, float3 axis)
            {
                float c, s;
                sincos(angle, s, c);

                float t = 1 - c;
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;

                return float3x3
                (
                    t * x * x + c,     t * x * y - s * z, t * x * z + s * y,
                    t * x * y + s * z, t * y * y + c,     t * y * z - s * x,
                    t * x * z - s * y, t * y * z + s * x, t * z * z + c
                );
            }

            VertexOutput tessVert(VertexInput v)
            {
                VertexOutput o;
                o.vertex  = v.vertex;   
                o.normal  = v.normal;
                o.tangent = v.tangent;
                o.uv      = v.uv;
                return o;
            }

            float tessellationEdgeFactor(VertexInput vert0, VertexInput vert1)
            {
                float3 v0 = vert0.vertex.xyz;
                float3 v1 = vert1.vertex.xyz;
                float edgeLength = distance(v0, v1);
                return edgeLength / _TessellationGrassDistance;
            }

            TessellationFactors patchConstantFunc(InputPatch<VertexInput, 3> patch)
            {
                TessellationFactors f;
                f.edge[0] = tessellationEdgeFactor(patch[1], patch[2]);
                f.edge[1] = tessellationEdgeFactor(patch[2], patch[0]);
                f.edge[2] = tessellationEdgeFactor(patch[0], patch[1]);
                f.inside  = (f.edge[0] + f.edge[1] + f.edge[2]) / 3.0f;
                return f;
            }

            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("integer")]
            [patchconstantfunc("patchConstantFunc")]
            VertexInput hull(InputPatch<VertexInput, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            [domain("tri")]
            VertexOutput domain(TessellationFactors factors, OutputPatch<VertexInput, 3> patch, float3 bc : SV_DomainLocation)
            {
                VertexInput i;

                #define INTERPOLATE(fieldname) i.fieldname = \
                    patch[0].fieldname * bc.x + \
                    patch[1].fieldname * bc.y + \
                    patch[2].fieldname * bc.z;

                INTERPOLATE(vertex)
                INTERPOLATE(normal)
                INTERPOLATE(tangent)
                INTERPOLATE(uv)

                return tessVert(i);
            }

            GeomData TransformGeomToClip(float3 posOS, float3 offsetOS, float3x3 m, float2 uv, float circleVal)
            {
                GeomData o;

                float3 pOS = posOS + mul(m, offsetOS);
                float3 pWS = TransformObjectToWorld(pOS);

                o.pos      = TransformWorldToHClip(pWS);
                o.worldPos = pWS;
                o.uv       = uv;
                o.circle   = circleVal;

                return o;
            }

            [maxvertexcount((BLADE_SEGMENTS * 2 + 1) * 3)]
            void geom(triangle VertexOutput input[3], inout TriangleStream<GeomData> triStream)
            {
                [unroll]
                for (int v = 0; v < 3; v++)
                {
                    float2 grassUV = TRANSFORM_TEX(input[v].uv, _GrassMap);
                    float grassVisibility = SAMPLE_TEXTURE2D_LOD(_GrassMap, sampler_GrassMap, grassUV, 0).r;

                    if (grassVisibility < _GrassThreshold)
                        continue;

                    float3 posOS    = input[v].vertex.xyz;
                    float3 normalOS = input[v].normal;
                    float4 tangentOS = input[v].tangent;
                    float3 bitangentOS = cross(normalOS, tangentOS.xyz) * tangentOS.w;

                    float3x3 tangentToLocal = float3x3
                    (
                        tangentOS.x, bitangentOS.x, normalOS.x,
                        tangentOS.y, bitangentOS.y, normalOS.y,
                        tangentOS.z, bitangentOS.z, normalOS.z
                    );

                    float3x3 randRotMatrix  = angleAxis3x3(rand(posOS) * UNITY_TWO_PI, float3(0, 0, 1.0f));
                    float3x3 randBendMatrix = angleAxis3x3(rand(posOS.zzx) * _BendDelta * UNITY_PI * 0.5f, float3(-1.0f, 0, 0));

                    float2 windUV = posOS.xz * _WindMap_ST.xy + _WindMap_ST.zw
                                  + normalize(_WindVelocity.xzy) * _WindFrequency * _Time.y;

                    float2 windSample2 = (SAMPLE_TEXTURE2D_LOD(_WindMap, sampler_WindMap, windUV, 0).xy * 2 - 1)
                                       * length(_WindVelocity);

                    float3 posWS = TransformObjectToWorld(posOS);

                    float2 totalDisp = float2(0, 0);
                    float maxCircle = 0.0;

                    int count = (int)min(_PositionCount, (float)MAX_POSITIONS);

                    [loop]
                    for (int i = 0; i < count; i++)
                    {
                        float3 p = _Positions[i].xyz;

                        float dist = distance(p, posWS);
                        float circle = 1.0 - saturate(dist / max(_Radius, 1e-4));
                        maxCircle = max(maxCircle, circle);

                        float3 sphereDisp = (posWS - p) * circle;                
                        float2 dispXZ = clamp(sphereDisp.xz, -_MaxWidth.xx, _MaxWidth.xx);
                        totalDisp += dispXZ;
                    }

                    float2 bend2 = windSample2 + totalDisp;
                    float bendAmt = length(bend2);

                    float3 bendAxis = (bendAmt > 1e-5) ? normalize(float3(bend2.x, bend2.y, 0)) : float3(0, 1, 0);
                    float3x3 bendMatrix = angleAxis3x3(UNITY_PI * bendAmt, bendAxis);

                    float3x3 baseM = mul(tangentToLocal, randRotMatrix);
                    float3x3 tipM  = mul(mul(mul(tangentToLocal, bendMatrix), randBendMatrix), randRotMatrix);

                    float falloff = smoothstep(_GrassThreshold, _GrassThreshold + _GrassFalloff, grassVisibility);

                    float width   = lerp(_BladeWidthMin,  _BladeWidthMax,  rand(posOS.xzy) * falloff);
                    float height  = lerp(_BladeHeightMin, _BladeHeightMax, rand(posOS.zyx) * falloff);
                    float forward = rand(posOS.yyz) * _BladeBendDistance;

                    for (int s = 0; s < BLADE_SEGMENTS; ++s)
                    {
                        float t = s / (float)BLADE_SEGMENTS;
                        float3 offset = float3(width * (1 - t), pow(t, _BladeBendCurve) * forward, height * t);

                        float3x3 m = (s == 0) ? baseM : tipM;

                        triStream.Append(TransformGeomToClip(posOS, float3( offset.x, offset.y, offset.z), m, float2(0, t), maxCircle));
                        triStream.Append(TransformGeomToClip(posOS, float3(-offset.x, offset.y, offset.z), m, float2(1, t), maxCircle));
                    }

                    triStream.Append(TransformGeomToClip(posOS, float3(0, forward, height), tipM, float2(0.5, 1), maxCircle));
                    triStream.RestartStrip();
                }
            }
        ENDHLSL

        Pass
        {
            Name "GrassPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma require geometry
            #pragma require tessellation tessHW

            #pragma vertex tessVert
            #pragma hull hull
            #pragma domain domain
            #pragma geometry geom
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT

            float4 frag(GeomData i) : SV_Target
            {
                float4 albedo = SAMPLE_TEXTURE2D(_BladeTexture, sampler_BladeTexture, i.uv);
                // here make a lerp from _baseTexture to _tipcolor instead of base color
				float4 tint   = lerp(_BaseColor, _TipColor, i.uv.y);
                float4 color  = albedo * tint;

                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)
                    float4 shadowCoord = TransformWorldToShadowCoord(i.worldPos);
                    Light mainLight = GetMainLight(shadowCoord);
                    half shadowAtten = saturate(mainLight.shadowAttenuation + 0.25h);
                    color.rgb *= shadowAtten;
                #endif

                return color;
            }
            ENDHLSL
        }
    }
}