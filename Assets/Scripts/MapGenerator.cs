using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode {NoiseMap, ColorMap, Mesh};
    [Header("General")]
    [SerializeField] private DrawMode drawMode;
    [Header("Map parameters")]
    public Noise.noiseAlgorithm NoiseAlgorithm;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;

    [SerializeField] private int octaves;
    [Range(0f, 1f)] [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    public bool autoUpdate;
    [SerializeField] private TerrainType[] terrainRegions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset, NoiseAlgorithm);
        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainRegions.Length; i++)
                {
                    if(currentHeight <= terrainRegions[i].height)
                    {
                        colorMap[y * mapWidth + x] = terrainRegions[i].color;
                        break;
                    }
                }
            }
        }
        MapDrawer mapDrawer = FindObjectOfType<MapDrawer>();
        if(drawMode == DrawMode.NoiseMap)
        {
            mapDrawer.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            mapDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            mapDrawer.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    private void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    [SerializeField] private string name;
    public float height;
    public Color color;
}
