using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum noiseAlgorithm { DIAMOND, SIMPLEX };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, noiseAlgorithm chosenAlgorithm)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random randomNr = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = randomNr.Next(-100000, 100000) + offset.x;
            float offsetY = randomNr.Next(-100000, 100000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if(scale <= 0)
        {
            scale = 0.00001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float yCoord = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

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
