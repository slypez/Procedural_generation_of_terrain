using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum noiseAlgorithm { PERLIN, DIAMOND };
    public enum NormalizeMode { LOCAL, GLOBAL };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random randomNr = new System.Random(settings.seed);
        //Random.InitState(settings.seed); // Fix so that this is works later
        Vector2[] octaveOffsets = new Vector2[settings.perlinSettings.octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.perlinSettings.octaves; i++)
        {
            float offsetX = randomNr.Next(-100000, 100000) + settings.perlinSettings.offset.x + sampleCenter.x;
            float offsetY = randomNr.Next(-100000, 100000) - settings.perlinSettings.offset.y - sampleCenter.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.perlinSettings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        switch (settings.noiseAlgorithm)
        {
            case noiseAlgorithm.PERLIN:
                DataAnalyzer.stopwatch.Start();
                noiseMap = Perlin.GeneratePerlinNoiseMap(mapWidth, mapHeight, settings, frequency, amplitude, octaveOffsets, minLocalNoiseHeight, maxLocalNoiseHeight, maxPossibleHeight);
                DataAnalyzer.stopwatch.Stop();
                break;
            case noiseAlgorithm.DIAMOND:
                DataAnalyzer.stopwatch.Start();
                noiseMap = DiamondSquare.GenerateDiamondNoiseMap(65, 65, settings, randomNr);
                DataAnalyzer.stopwatch.Stop();
                break;
        }

        return noiseMap;
    }

    private static void PerlinNoise(int mapWidth, int mapHeight, NoiseSettings settings, float frequency, float amplitude, Vector2[] octaveOffsets, float minLocalNoiseHeight, float maxLocalNoiseHeight, float maxPossibleHeight, float[,] noiseMap)
    {
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

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

                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 -1;

                    noiseMap[x, y] = noiseValue;
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

                if (settings.perlinSettings.normalizeMode == NormalizeMode.GLOBAL)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.perlinSettings.normalizeMode == NormalizeMode.LOCAL)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]); // Normalizing of height
                }
            }
        }
    }
}

[System.Serializable]
public class NoiseSettings
{
    [Header("General")]
    public Noise.noiseAlgorithm noiseAlgorithm;
    public int seed;
    public float scale;
    
    public PerlinSettings perlinSettings;
    public DiamondSquareSettings diamondSquareSettings;
    [System.Serializable]
    public class PerlinSettings
    {
    public Noise.NormalizeMode normalizeMode;
    public int octaves;
    public float lacunarity;
    [Range(0f, 1f)] public float persistance;
    public Vector2 offset;
    }
    [System.Serializable]
    public class DiamondSquareSettings
    {
        public int roughness;
    }

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        perlinSettings.octaves = Mathf.Max(perlinSettings.octaves, 1);
        perlinSettings.lacunarity = Mathf.Max(perlinSettings.lacunarity, 1);
        perlinSettings.persistance = Mathf.Clamp01(perlinSettings.persistance);
    }
}