using UnityEngine;

public class NoiseMapGeneration : MonoBehaviour
{
      public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ){ 
        // mapDepth and mapWidth are variables that alter the size of the map
        // offSetX and offsetZ are diffrent LevelTile positions, using these values will make sure theres continuity between tiles
        // Source: https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int iZ = 0; iZ < mapDepth; iZ++) // Z-axis
        {
            for (int iX = 0; iX < mapWidth; iX++) // X-axis
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (iX+offsetX) / scale;
                float sampleZ = (iZ+offsetZ) / scale;
                // generate noise value using PerlinNoise
                float noise = Mathf.PerlinNoise(sampleX, sampleZ); // Possible to rewrite this if we want to use our own noise generation
                noiseMap[iZ, iX] = noise;
            }
        }
        return noiseMap;
    }
}
