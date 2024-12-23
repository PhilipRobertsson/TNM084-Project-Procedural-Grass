Shader "Custom/GrassShader"
{
    Properties
    {
        // Here we define the properties of the shader example texture or color
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}

    }
    SubShader
    {
        // Tags are used to define the render type and the render pipeline
        // LOD is the level of detail of the shader
       
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversialPipeline"}
        LOD 100

        // Here we define the pass of the shader.
        // The pass is the part of the shader that will be executed in the GPU

        Pass
        {


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #pragma target 4.5 //whatever this version is 😋

            //unsure what we need to include here because we are using the Universal Render Pipeline.
            // We want to include files that help us compute the lighting and the shadows or noise ex simplex.
            // I dont know if we need all of them or just some of them but in da video there is custom ones so we could make our own :)
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
           #include "UnityCG.cginc"
            


            
            ENDCG
        }
    }
}
