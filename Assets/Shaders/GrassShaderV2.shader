Shader "Custom/GrassShaderV2"
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

		_scale("Scale", Range(0,2)) = 1
		_BendDelta("Bend Variation", Range(0, 1)) = 0.2

		_TessellationGrassDistance("Tessellation Grass Distance", Range(0.01, 2)) = 0.1

		_GrassMap("Grass Visibility Map", 2D) = "white" {}
		_GrassThreshold("Grass Visibility Threshold", Range(-0.1, 1)) = 0.5
		_GrassFalloff("Grass Visibility Fade-In Falloff", Range(0, 0.5)) = 0.05

		_WindMap("Wind Offset Map", 2D) = "bump" {}
		_WindVelocity("Wind Velocity", Vector) = (1, 0, 0, 0)
		_WindFrequency("Wind Pulse Frequency", Range(0, 1)) = 0.01
	}

	SubShader
	{

	        // Tags are used to define the render type and the render pipeline
        // LOD is the level of detail of the shader
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"RenderPipeline" = "UniversalPipeline"
		}
		LOD 100
		Cull Off

		        // HLSLINCLUDE is used to include the HLSL code from the Universal Render Pipeline so we dont need
        // a seperate file essentially like <script> in HTML. 
            // include the core and lighting libraries from the Universal Render Pipeline
            // core is used for basic functions and lighting you can guess what that is used for. 
		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT

			#define UNITY_PI 3.14159265359f
			#define UNITY_TWO_PI 6.28318530718f
			#define BLADE_SEGMENTS 4
			
			 //Cbuffer is declared to store the data that is passed from the CPU to the GPU
			CBUFFER_START(UnityPerMaterial)
				float4 _BaseColor;
				float4 _TipColor;
				sampler2D _BladeTexture;

				float _BladeWidthMin;
				float _BladeWidthMax;
				float _BladeHeightMin;
				float _BladeHeightMax;

				float _BladeBendDistance;
				float _BladeBendCurve;

				float _BendDelta;
				float _scale;

				float _TessellationGrassDistance;
				
				sampler2D _GrassMap;
				float4 _GrassMap_ST;
				float  _GrassThreshold;
				float  _GrassFalloff;

				sampler2D _WindMap;
				float4 _WindMap_ST;
				float4 _WindVelocity;
				float  _WindFrequency;

				float4 _ShadowColor;
			CBUFFER_END

			// VertexInput is used to pass data from the CPU to the GPU
			struct VertexInput
			{
				float4 vertex  : POSITION;
				float3 normal  : NORMAL;
				float4 tangent : TANGENT;
				float2 uv      : TEXCOORD0;
			};

            // VertexOutput is used to pass data from the vertex shader to the fragment shader
			struct VertexOutput
			{
				float4 vertex  : SV_POSITION;
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
				float4 pos : SV_POSITION;
				float2 uv  : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			// Simple noise function, from http://answers.unity.com/answers/624136/view.html
            // We should probably change this to one made by Stegu eller iaf keijiro
            //should be between 0 and 1
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
			}

			// Construct a rotation matrix that rotates around the provided axis, sourced from:
			// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
			// "intuitive to use but difficult to implement" - KJ 2024
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
					t * x * x + c, t * x * y - s * z, t * x * z + s * y,
					t * x * y + s * z, t * y * y + c, t * y * z - s * x,
					t * x * z - s * y, t * y * z + s * x, t * z * z + c
				);
			}

			// Regular vertex shader used by typical shaders but transforms the object to clip space
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.normal = v.normal;
				o.tangent = v.tangent;
				o.uv = TRANSFORM_TEX(v.uv, _GrassMap);
				return o;
			}


			// Vertex shader which translates from object to world space.
			VertexOutput geomVert (VertexInput v)
            {
				VertexOutput o; 
				o.vertex = v.vertex;
				o.normal = TransformObjectToWorldNormal(v.normal);
				o.tangent = v.tangent;
				o.uv = TRANSFORM_TEX(v.uv, _GrassMap);
                return o;
            }

			// Geometry functions derived from Roystan's tutorial:
			// https://roystan.net/articles/grass-shader.html

			// This function applies a transformation (during the geometry shader),
			// converting to clip space in the process.
			GeomData TransformGeomToClip(float3 pos, float3 offset, float3x3 transformationMatrix, float2 uv)
			{
				GeomData o;

				// transforming the vertex from object space to clip space here
				o.pos = TransformObjectToHClip(pos + mul(transformationMatrix, offset));
				o.uv = uv;
				o.worldPos = TransformObjectToWorld(pos + mul(transformationMatrix, offset));

				return o;
			}

			 // Geometry shader that creates the grass blades
            // BLADE_SEGMENTS is the number of vertices that make up the grass blade.
            // Tessellation gives us more vertices per blade.
			[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
			void geom(point VertexOutput input[1], inout TriangleStream<GeomData> triStream)
			{
				float grassVisibility = tex2Dlod(_GrassMap, float4(input[0].uv, 0, 0)).r;

				if (grassVisibility < _GrassThreshold) return;
				
				float3 pos = input[0].vertex.xyz;


				float3 normal = input[0].normal;
				float4 tangent = input[0].tangent;
				float3 bitangent = cross(normal, tangent.xyz) * tangent.w;

				float3x3 tangentToLocal = float3x3
				(
					tangent.x, bitangent.x, normal.x,
					tangent.y, bitangent.y, normal.y,
					tangent.z, bitangent.z, normal.z
				);

				tangentToLocal *= _scale; 

				// Rotate around the y-axis a random amount.
				float3x3 randRotMatrix = angleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1.0f));

				// Rotate around the bottom of the blade a random amount.
				float3x3 randBendMatrix = angleAxis3x3(rand(pos.zzx) * _BendDelta * UNITY_PI * 0.5f, float3(-1.0f, 0, 0));

				//scroll the wind map
				float2 windUV = pos.xz * _WindMap_ST.xy + _WindMap_ST.zw + normalize(_WindVelocity.xzy) * _WindFrequency * _Time.y;
				float2 windSample = (tex2Dlod(_WindMap, float4(windUV, 0, 0)).xy * 2 - 1) * length(_WindVelocity);

				float3 windAxis = normalize(float3(windSample.x, windSample.y, 0));
				float3x3 windMatrix = angleAxis3x3(UNITY_PI * windSample, windAxis);

				// Transform the grass blades to the correct tangent space.
				float3x3 baseTransformationMatrix = mul(tangentToLocal, randRotMatrix);
				float3x3 tipTransformationMatrix = _scale * mul(mul(mul(tangentToLocal, windMatrix), randBendMatrix), randRotMatrix);

				float falloff = smoothstep(_GrassThreshold, _GrassThreshold + _GrassFalloff, grassVisibility);

				float width  = lerp(_BladeWidthMin, _BladeWidthMax, rand(pos.xzy) * falloff);
				float height = lerp(_BladeHeightMin, _BladeHeightMax, rand(pos.zyx) * falloff);
				float forward = rand(pos.yyy) * _BladeBendDistance;

				// Create blade segments by adding two vertices at once.
				for (int i = 0; i < BLADE_SEGMENTS; ++i)
				{
					float t = i / (float)BLADE_SEGMENTS;
					float3 offset = float3(width * (1 - t), pow(t, _BladeBendCurve) * forward, height * t);

					float3x3 transformationMatrix = (i == 0) ? baseTransformationMatrix : tipTransformationMatrix;

					triStream.Append(TransformGeomToClip(pos, float3( offset.x, offset.y, offset.z), transformationMatrix, float2(0, t)));
					triStream.Append(TransformGeomToClip(pos, float3(-offset.x, offset.y, offset.z), transformationMatrix, float2(1, t)));
				}

					// Add the final vertex at the tip of the grass blade.
				triStream.Append(TransformGeomToClip(pos, float3(0, forward, height), tipTransformationMatrix, float2(0.5, 1)));

				triStream.RestartStrip();
				
			}
		ENDHLSL

		// This pass draws the grass blades generated by the geometry shader.
        Pass
        {
			Name "GrassPass"
			Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
			#pragma require geometry

			//#pragma vertex vert
			#pragma vertex geomVert
			#pragma geometry geom
            #pragma fragment frag

			// The lighting sections of the frag shader taken from this helpful post by Ben Golus:
			// https://forum.unity.com/threads/water-shader-graph-transparency-and-shadows-universal-render-pipeline-order.748142/#post-5518747
            float4 frag (GeomData i) : SV_Target
            {
				float4 color = tex2D(_BladeTexture, i.uv);

			#ifdef _MAIN_LIGHT_SHADOWS
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = i.worldPos;

				float4 shadowCoord = GetShadowCoord(vertexInput);
				half shadowAttenuation = saturate(MainLightRealtimeShadow(shadowCoord) + 0.25f);
				float4 shadowColor = lerp(0.0f, 1.0f, shadowAttenuation);
				color *= shadowColor;
			#endif

				//apply the base and tip color to the grass blade
                return color * lerp(_BaseColor, _TipColor, i.uv.y);
			}

			ENDHLSL
		}
    }
}