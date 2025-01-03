using UnityEngine;
using UnityEditor;

public class TileGeneration : MonoBehaviour
{
    [SerializeField]
    NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;

    [SerializeField]
    private float mapScale;

    [SerializeField]
    private float heightMultiplier;

    [SerializeField]
    private bool toggleGrassTexture;

    [SerializeField]
    private Texture2D grassTexture;

    float[,] heightMap;

    private void UpdateMeshVertices(float[,] heightMap){
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * this.heightMultiplier, vertex.z);
                vertexIndex++;
            }
        }
        // update the vertices in the mesh and update its properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }

    void Start()
    {
        // Source: https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
        GenerateTile();
    }

    void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // calculate the offsets based on the tile position
        heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ);

        // generate a heightMap using noise
        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = tileTexture;
        UpdateMeshVertices(heightMap);
    }

    private Texture2D BuildTexture(float[,] heightMap){
        int tileDepth = 0;
        int tileWidth = 0;
        Texture2D tileTexture = null;

        if (!toggleGrassTexture){
            tileDepth = heightMap.GetLength(0);
            tileWidth = heightMap.GetLength(1);
            tileTexture = new Texture2D(tileWidth, tileDepth);
        }
        else{
            tileDepth = grassTexture.height;
            tileWidth = grassTexture.width;
            tileTexture = grassTexture;
        }

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                if (!toggleGrassTexture)
                {
                    float height = heightMap[zIndex, xIndex];
                    // assign as color a shade of grey proportional to the height value
                    colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
                }else{
                    colorMap[colorIndex] = tileTexture.GetPixel(xIndex, zIndex);
                }
            }
        }

        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();
        return tileTexture;
    }
}