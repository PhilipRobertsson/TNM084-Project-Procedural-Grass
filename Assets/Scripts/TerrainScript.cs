using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TerrainScript : MonoBehaviour
{

    private ComputeShader terrainDisplacement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrainDisplacement = Resources.Load<ComputeShader>("TerrainDisplacement");

        //Get the mesh of the terrain, vertices and UVs
        Mesh mesh = GetComponent<MeshFilter>().mesh; 
        Vector3[] verts = mesh.vertices;  
        Vector2[] uvs = mesh.uv;

        //Create the buffers for the vertices and UVs and set the data
        ComputeBuffer vertexBuffer = new ComputeBuffer(verts.Length, 12);
        ComputeBuffer uvBuffer = new ComputeBuffer(uvs.Length, 8);
        vertexBuffer.SetData(verts);
        uvBuffer.SetData(uvs);

        //Get the material of the terrain and set the buffers and textures
        Material terrainMat = GetComponent<MeshRenderer>().sharedMaterial;

        //Set the buffers and textures to the compute shader aka TerrainDispalcement.compute
        terrainDisplacement.SetBuffer(0, "_Vertices", vertexBuffer);
        terrainDisplacement.SetBuffer(0, "_UVs", uvBuffer);
        terrainDisplacement.SetTexture(0, "_HeightMap", terrainMat.GetTexture("_HeightMap"));
        terrainDisplacement.SetFloat("_DisplacementStrength", terrainMat.GetFloat("_DisplacementStrength"));
        terrainDisplacement.Dispatch(0, Mathf.CeilToInt(verts.Length / 128.0f), 1, 1);

        //Dispatch the compute shader. 
        vertexBuffer.GetData(verts);
        vertexBuffer.Release();
        uvBuffer.Release();

        //Set the new vertices to the mesh and recalculate the normals and bounds
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        //
        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = null;
        mc.sharedMesh = mesh;



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
