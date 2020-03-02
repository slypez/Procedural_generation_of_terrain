using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                if(values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if(values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }
        return new HeightMap(values, minValue, maxValue);
    }


    //private MapData GenerateMapData(Vector2 center)
    //{
    //    float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode, NoiseAlgorithm);

    //    if (terrainData.useFalloffMap)
    //    {

    //        if (falloffMap == null)
    //        {
    //            falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
    //        }
    //        for (int y = 0; y < mapChunkSize + 2; y++)
    //        {
    //            for (int x = 0; x < mapChunkSize + 2; x++)
    //            {
    //                if (terrainData.useFalloffMap)
    //                {
    //                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
    //                }

    //            }
    //        }
    //    }

    //    return new MapData(noiseMap);
    //}
}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
