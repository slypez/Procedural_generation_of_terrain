using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiamondSquare
{
	public static float[,] GenerateDiamondNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, 
		System.Random rng)
	{
		Debug.Log(mapWidth);
		float[,] noiseMap = new float[mapWidth, mapHeight];

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapHeight; x++)
			{
				noiseMap[x, y] = 0;
			}
		}

		//Random.InitState(settings.seed);

		int h = settings.diamondSquareSettings.roughness;
		noiseMap[0, 0] = rng.Next(-h, h);
		noiseMap[mapHeight - 1, 0] = rng.Next(-h, h);
		noiseMap[0, mapHeight - 1] = rng.Next(-h, h);
		noiseMap[mapHeight - 1, mapHeight - 1] = rng.Next(-h, h);

		float valMin = float.MaxValue;
		float valMax = float.MinValue;

		for (int sideLength = mapHeight - 1; sideLength >= 2; sideLength /= 2)
		{
			int halfSide = sideLength / 2;

			//Diamond
			for (int x = 0; x < mapHeight - 1; x += sideLength)
			{
				for (int y = 0; y < mapHeight - 1; y += sideLength)
				{
					float cornerSum =
						noiseMap[x, y] +
						noiseMap[x + sideLength, y] +
						noiseMap[x, y + sideLength] +
						noiseMap[x + sideLength, y + sideLength];

					float avgValue = cornerSum / 4f;
					avgValue += (rng.Next(-h, h) * 2 * h) - h;

					noiseMap[x + halfSide, y + halfSide] = avgValue;

					valMax = Mathf.Max(valMax, noiseMap[x + halfSide, y + halfSide]);
					valMin = Mathf.Min(valMin, noiseMap[x + halfSide, y + halfSide]);
				}
			}

			//Square
			for (int x = 0; x < mapHeight - 1; x += halfSide)
			{
				for (int y = (x + halfSide) % sideLength; y < mapHeight - 1; y += sideLength)
				{
					float avgValue =
						noiseMap[(x - halfSide + mapHeight - 1) % (mapHeight - 1), y] +
						noiseMap[(x + halfSide) % (mapHeight - 1), y] +
						noiseMap[x, (y + halfSide) % (mapHeight - 1)] +
						noiseMap[x, (y - halfSide + mapHeight - 1) % (mapHeight - 1)];

					avgValue /= 4f;
					avgValue += (rng.Next(-h, h) * 2 * h) - h;

					noiseMap[x, y] = avgValue;

					valMax = Mathf.Max(valMax, avgValue);
					valMin = Mathf.Min(valMin, avgValue);

					if (x == 0)
					{
						noiseMap[mapHeight - 1, y] = avgValue;
					}
					if (y == 0)
					{
						noiseMap[x, mapHeight - 1] = avgValue;
					}
				}
			}

			h /= 2;
		}

		for (int i = 0; i < mapHeight; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				noiseMap[i, j] = (noiseMap[i, j] - valMin) / (valMax - valMin);
			}
		}

		return noiseMap;
	}
}
