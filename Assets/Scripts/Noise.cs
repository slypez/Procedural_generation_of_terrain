using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum noiseAlgorithm { DIAMOND, SIMPLEX };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, noiseAlgorithm chosenAlgorithm)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if(scale <= 0)
        {
            scale = 0.00001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = x / scale * frequency;
                    float yCoord = y / scale * frequency;

                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1; // Most important line, here we set value and give algorithm. \/

                    //switch (chosenAlgorithm)
                    //{
                    //    case noiseAlgorithm.DIAMOND:
                    //        noiseValue = 1; // = INSERT DIAMOND-FUNC HERE;
                    //        break;
                    //    case noiseAlgorithm.SIMPLEX:
                    //        noiseValue = 2;// = INSERT SIMPLEX-FUNC HERE;
                    //        break;
                    //}
                    noiseMap[x, y] = noiseValue;
                    noiseHeight += noiseValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); // Normalizing of height
            }
        }

                return noiseMap;
    }
}
