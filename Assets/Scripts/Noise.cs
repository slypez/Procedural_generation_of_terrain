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
        Random.InitState(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = randomNr.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = randomNr.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        switch (settings.noiseAlgorithm)
        {
            case noiseAlgorithm.PERLIN:
                PerlinNoise(mapWidth, mapHeight, settings, frequency, amplitude, octaveOffsets, minLocalNoiseHeight, maxLocalNoiseHeight, maxPossibleHeight, noiseMap);
                break;
            case noiseAlgorithm.DIAMOND:
                DiamondSquareNoise(mapWidth, mapHeight, settings, frequency, amplitude, octaveOffsets, minLocalNoiseHeight, maxLocalNoiseHeight, maxPossibleHeight, noiseMap);
                break;
        }

        return noiseMap;
    }
    private static void DiamondSquareNoise(int mapWidth, int mapHeight, NoiseSettings settings, float frequency, float amplitude, Vector2[] octaveOffsets, float minLocalNoiseHeight, float maxLocalNoiseHeight, float maxPossibleHeight, float[,] noiseMap) // Om det inte fungerar, se till att noiseMapen i parametern är en direkt ref keyword till den riktiga som går in.
    {
        int sideLength, halfSide, x, y;
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        float avgValue;

        //Initialize: Give 4 corners values for algorithm
        noiseMap[0, 0] = Random.value;
        noiseMap[mapWidth - 1, 0] = Random.value;
        noiseMap[0, mapHeight - 1] = Random.value;
        noiseMap[mapWidth - 1, mapHeight - 1] = Random.value;

        for (sideLength = mapWidth -1; sideLength > 1; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            //Diamond-step
            for (x = 0; x < mapWidth - 1; x+= halfSide)
            {
                for (y = (x + halfSide) % sideLength; y < mapWidth - 1; y+= sideLength)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;
                    for (int i = 0; i < settings.octaves; i++)
                    {
                        float xCoord = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                        float yCoord = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                        avgValue = noiseMap[(x - halfSide + mapWidth - 1) % (mapWidth - 1), y];
                        avgValue += noiseMap[(x + halfSide) % (mapWidth - 1), y];
                        avgValue += noiseMap[x, (y + halfSide) % (mapWidth - 1)];
                        avgValue += noiseMap[x, (y - halfSide + mapWidth - 1) % (mapWidth -1)];
                        avgValue /= 4f;

                        //float rnd = (Random.value * 2f * settings.h) - settings.h;
                        //avgValue = Mathf.Clamp01(avgValue + rnd);
                        //avgValue += (Random.value * 2 * h) - h;

                        noiseMap[x, y] = avgValue;
                        noiseHeight += avgValue * amplitude;
                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
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

                    if(x == 0)
                    {
                        noiseMap[mapWidth - 1, y] = noiseHeight;
                    }
                    if(y == 0)
                    {
                        noiseMap[x, mapWidth - 1] = noiseHeight;
                    }

                    if (settings.normalizeMode == NormalizeMode.GLOBAL)
                    {
                        float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            //Square-step
            for (x = 0; x < mapHeight - 1 - sideLength; x += sideLength)
            {
                for (y = 0; y < mapWidth - 1 - sideLength; y += sideLength)
                {
                    amplitude = 1;
                    frequency = 1;

                    float noiseHeight = 0;
                    for (int i = 0; i < settings.octaves; i++)
                    {
                        int newX = (int)(x / settings.scale * frequency);
                        int newY = (int)(y / settings.scale * frequency);

                        //int newX = (int)((x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency);
                        //int newY = (int)((y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency);

                        avgValue = noiseMap[x, y];
                        avgValue += noiseMap[x + sideLength, y];
                        avgValue += noiseMap[x, y + sideLength];
                        avgValue += noiseMap[x + sideLength, y + sideLength];
                        avgValue /= 4f;

                        //float rnd = (Random.value * 2f * settings.h) - settings.h;
                        //avgValue = Mathf.Clamp01(avgValue + rnd);
                        //avgValue += (Random.value * 2 * h) - h;

                        noiseMap[x, y] = avgValue;
                        noiseHeight += avgValue * amplitude;
                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
                    }
                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                    noiseMap[x + halfSide, y + halfSide] = noiseHeight;

                    if (settings.normalizeMode == NormalizeMode.GLOBAL)
                    {
                        float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }
            //settings.h -= settings.h * 0.5f * settings.roughness;
        }

        if (settings.normalizeMode == NormalizeMode.LOCAL)
        {
            for (int y2 = 0; y2 < mapHeight; y2++)
            {
                for (int x2 = 0; x2 < mapWidth; x2++)
                {
                    noiseMap[x2, y2] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x2, y2]); // Normalizing of height
                }
            }
        }
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
                for (int i = 0; i < settings.octaves; i++)
                {
                    float xCoord = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float yCoord = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 -1;

                    noiseMap[x, y] = noiseValue;
                    noiseHeight += noiseValue * amplitude;
                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
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

                if (settings.normalizeMode == NormalizeMode.GLOBAL)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.LOCAL)
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
    [Header("Parameters")]
    public Noise.NormalizeMode normalizeMode;
    public Noise.noiseAlgorithm noiseAlgorithm;
    public int seed;
    public int octaves;
    public float lacunarity;
    [Range(0f, 1f)] public float persistance;
    public float scale;
    public Vector2 offset;
    [Header("Diamond-square")]
    public float deleteThisVariableLater;
    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}

public static class DiamondSquare
{


    //public void sampleSquare(int x, int y, int size, double value)
    //{
    //    int hs = size / 2;

    //    // a     b 
    //    //
    //    //    x
    //    //
    //    // c     d

    //    double a = sample(x - hs, y - hs);
    //    double b = sample(x + hs, y - hs);
    //    double c = sample(x - hs, y + hs);
    //    double d = sample(x + hs, y + hs);

    //    setSample(x, y, ((a + b + c + d) / 4.0) + value);

    //}

    //public void sampleDiamond(int x, int y, int size, double value)
    //{
    //    int hs = size / 2;

    //    //   c
    //    //
    //    //a  x  b
    //    //
    //    //   d

    //    double a = sample(x - hs, y);
    //    double b = sample(x + hs, y);
    //    double c = sample(x, y - hs);
    //    double d = sample(x, y + hs);

    //    setSample(x, y, ((a + b + c + d) / 4.0) + value);
    //}

    //private void DiamondSquareAlgorithm(int stepsize, double scale)
    //{

    //    int halfstep = stepsize / 2;

    //    for (int y = halfstep; y <= h + halfstep; y += stepsize)
    //{
    //        for (int x = halfstep; x <= w + halfstep; x += stepsize)
    //    {
    //            sampleSquare(x, y, stepsize, Random.Range(-1f, 1f));
    //        }
    //    }

    //    for (int y = 0; y <= h; y += stepsize)
    //{
    //        for (int x = 0; x <= w; x += stepsize)
    //    {
    //            sampleDiamond(x + halfstep, y, stepsize, Random.Range(-1f, 1f));
    //            sampleDiamond(x, y + halfstep, stepsize, Random.Range(-1f, 1f));
    //        }
    //    }

    //}
}