using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum noiseAlgorithm { DIAMOND, SIMPLEX };
    public enum NormalizeMode { LOCAL, GLOBAL };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode, noiseAlgorithm chosenAlgorithm)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random randomNr = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = randomNr.Next(-100000, 100000) - offset.x;
            float offsetY = randomNr.Next(-100000, 100000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }


        if(scale <= 0)
        {
            scale = 0.00001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float yCoord = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

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
                if(noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(normalizeMode == NormalizeMode.LOCAL)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]); // Normalizing of height
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

                return noiseMap;
    }
}
