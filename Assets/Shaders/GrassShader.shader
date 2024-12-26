Shader "Custom/GrassShader"
{
    Properties
    {
        // Here we define the properties of the shader example texture or color
        _BaseColor ("Base Colour", Color) = (1,1,1,1)
        _TipColor ( "Tip Colour", Color) = (1,1,1,1)
        _BladeTexture("Blade Texture", 2D) = "white" {}

        // Definerar gräsblads egenskaper
        _BladeWidthMin("Blade Width (Min)", Range(0.0, 0.1)) = 0.05
        _BladeWidthMax("Blade Width (Max)", Range(0.0, 0.1)) = 0.1
        _BladeHeightMin("Blade Height (Min)", Range(0.0, 0.1)) = 0.05
        _BladeHeightMax("Blade Height (Max)", Range(0.0, 0.1)) = 0.1

        // Definerar gräsbladets böjning
        _BladeSegments("Blade Segments", Range(1, 10)) = 3
        _BladeBendDistance("Blade Forward Amount", Float) = 0.4
        _BladeBendCurve("Blade Curve Amount", Range(1, 4)) = 2

        // Definerar gräsbladets variation
        _BendDelta("Bend Variation", Range(0, 1)) = 0.2
        
        // Definerar gräsbladets "kvalitet" (tessellation)
        _TessellationGrassDistance("Tessellation Grass Distance", Range(0.01, 2)) = 0.1

        // Definerar gräsets täckning av ytan från kameran
        _GrassMap("Grass Visiblity Map", 2D) = "white" {}
        _GrassThreshold("Grass Visiblity Threshold", Range(-0.1, 1)) = 0.5
        _GrassFalloff("Grass Visiblity Fade-In Falloff", Range(0, 0.5)) = 0.05

        // Definerar gräsets rörelse (vind)
        _WindMap("Wind Offset Map", 2D) = "bump" {}
        _WindVelocity("Wind Speed", Vector) = (1,0,0,0)
        _WindFrequency("Wind Pulse Frequency", Range(0,1)) = 0.01
    }

    SubShader
    {
        // Tags are used to define the render type and the render pipeline
        // LOD is the level of detail of the shader
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversialPipeline" }
        LOD 100
        Cull Off

        // HLSLINCLUDE is used to include the HLSL code from the Universal Render Pipeline so we dont need
        // a seperate file essentially like <script> in HTML. 
        HLSLINCLUDE

            // include the core and lighting libraries from the Universal Render Pipeline
            // core is used for basic functions and lighting you can guess what that is used for. 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"



            CBUFFER_START(UnityPerMaterial)

	            float4 _TipColor;
	            sampler2D _BladeTexture;

	            float _BladeWidthMin;
	            float _BladeWidthMax;
	            float _BladeHeightMin;
	            float _BladeHeightMax;

	            float _BladeBendDistance;
	            float _BladeBendCurve;

	            float _BendDelta;

	            float _TessellationGrassDistance;

	            sampler2D _GrassMap;
	            float4 _GrassMap_ST;
	            float _GrassThreshold;
	            float _GrassFalloff;

	            sampler2D _WindMap;
	            float4 _WindMap_ST;
	            float4 _WindVelocity;
	            float _WindFrequency;

	            float4 _ShadowColor;
            CBUFFER_END

            // VertexInput is used to pass data from the CPU to the GPU
            struct VertexInput{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            // VertexOutput is used to pass data from the vertex shader to the fragment shader
            struct VertexOutput{
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            VertexOutput vert(VertexInput v){

                VertexOutput o;
                // Here we transform the vertex from object space to clip space
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.normal = v.normal;
                o.tangent = v.tangent;
                o.uv = TRANSFORM_TEX(v.uv, _GrassMap);
                return o;
            }
        ENDHLSL

    Pass{
        // Name of the pass
        Name "GrassPass"
        // UniversialForward for URP
        Tags { "LightMode" = "UniversialForward" }

        HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 frag(VertexOutput i) : SV_Target{
                return float4(1.0f, 1.0f, 1.0f, 1.0f);
            }
        ENDHLSL
    }






        
    }
}
