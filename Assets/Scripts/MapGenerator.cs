using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map parameters")]
    public Noise.noiseAlgorithm NoiseAlgorithm;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    public bool autoUpdate;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, NoiseAlgorithm);

        MapDrawer mapDrawer = FindObjectOfType<MapDrawer>();
        mapDrawer.DrawNoiseMap(noiseMap);
    }
}
