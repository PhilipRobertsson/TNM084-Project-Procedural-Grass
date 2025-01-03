using System;
using UnityEngine;
using UnityEngine.Rendering;

using static ProceduralSurface;

public class SurfaceLevelGen : MonoBehaviour
{
    [SerializeField, Range(1, 40)]
    int worldSize = 1;

    [SerializeField, Range(1, 3)]
    int chunkDimensions = 1;

    [SerializeField, Range(1, 50)]
    public int chunkResolution = 1;

    [SerializeField, Range(-1f, 1f)]
    public float chunkDisplacement = 0.5f;

    [SerializeField]
    Material grass;

    public enum MaterialMode { Displacement, Ground, GroundVariant, CubeMap }

    [SerializeField]
    ProceduralSurface.MaterialMode material;

    [SerializeField]
    Material[] materials;

    GameObject[] chunkArray;

    void Start(){
        chunkArray = new GameObject[worldSize * worldSize];
        GenerateMap();
    }
    void Update(){
        
    }

    void GenerateMap(){
        for (int iXT = 0; iXT < worldSize; iXT++) {
            for (int iZT = 0; iZT < worldSize; iZT++) {
                // Create Game object
                GameObject chunk = new GameObject { name = "surfaceChunk" };

                // Calculate chunk Position
                Vector3 chunkPosition = new Vector3(chunk.transform.position.x + iXT,
                  chunk.transform.position.y,
                  chunk.transform.position.z + iZT
                 );

                // Assign components
                chunk.AddComponent<MeshFilter>();
                chunk.AddComponent<MeshRenderer>();
                chunk.GetComponent<MeshRenderer>().materials = new Material[] { null, grass };
                chunk.AddComponent<ProceduralSurface>();

                // Set start values for procedural surface component

                chunk.GetComponent<ProceduralSurface>().materials = materials;
                chunk.GetComponent<ProceduralSurface>().material = material;
                chunk.GetComponent<ProceduralSurface>().dimensions = chunkDimensions;
                chunk.GetComponent<ProceduralSurface>().resolution = chunkResolution;
                chunk.GetComponent<ProceduralSurface>().displacement = chunkDisplacement;
                chunk.GetComponent<ProceduralSurface>().domain.translation.x = iXT;
                chunk.GetComponent<ProceduralSurface>().domain.translation.z = iZT;
                chunk.GetComponent<Transform>().position = chunkPosition;

                chunkArray[iZT * worldSize + iXT] = chunk;
            }
        }
    }
}
