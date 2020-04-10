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

        switch (settings.noiseAlgorithm)
        {
            case noiseAlgorithm.PERLIN:
                DataAnalyzer.stopwatch.Restart();
                noiseMap = Perlin.GeneratePerlinNoiseMap(mapWidth, mapHeight, settings, randomNr, sampleCenter);
                DataAnalyzer.stopwatch.Stop();
                break;
            case noiseAlgorithm.DIAMOND:
                DataAnalyzer.stopwatch.Restart();
                noiseMap = DiamondSquare.GenerateDiamondNoiseMap(mapWidth, mapHeight, settings, randomNr);
                DataAnalyzer.stopwatch.Stop();
                break;
        }

        return noiseMap;
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
        public float roughness;
    }

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        perlinSettings.octaves = Mathf.Max(perlinSettings.octaves, 1);
        perlinSettings.lacunarity = Mathf.Max(perlinSettings.lacunarity, 1);
        perlinSettings.persistance = Mathf.Clamp01(perlinSettings.persistance);
    }
}