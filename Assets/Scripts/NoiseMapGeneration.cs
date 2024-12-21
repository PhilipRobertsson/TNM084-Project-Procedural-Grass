using UnityEngine;

public class NoiseMapGeneration : MonoBehaviour
{
      public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale){ // mapDepth and mapWidth are variables that alter the size of the map
        // Source: https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++) // Z-axis
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++) // X-axis
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = xIndex / scale;
                float sampleZ = zIndex / scale;
                // generate noise value using PerlinNoise
                float noise = Mathf.PerlinNoise(sampleX, sampleZ); // Possible to rewrite this if we want to use our own noise generation
                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
    }
}
