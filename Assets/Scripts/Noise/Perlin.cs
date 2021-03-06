﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Perlin
{

    public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, System.Random randomNr, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        Vector2[] octaveOffsets = new Vector2[settings.perlinSettings.octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int i = 0; i < settings.perlinSettings.octaves; i++)
        {
            float offsetX = randomNr.Next(-100000, 100000) + settings.perlinSettings.offset.x + sampleCenter.x;
            float offsetY = randomNr.Next(-100000, 100000) - settings.perlinSettings.offset.y - sampleCenter.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.perlinSettings.persistance;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < settings.perlinSettings.octaves; i++)
                {
                    float xCoord = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float yCoord = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    noiseHeight += noiseValue * amplitude;
                    amplitude *= settings.perlinSettings.persistance;
                    frequency *= settings.perlinSettings.lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

                if (settings.perlinSettings.normalizeMode == Noise.NormalizeMode.GLOBAL)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.perlinSettings.normalizeMode == Noise.NormalizeMode.LOCAL)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]); // Normalizing of height
                }
            }
        }
        return noiseMap;
    }

}
