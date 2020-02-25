using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode {NoiseMap, ColorMap, Mesh};
    [Header("General")]
    [SerializeField] private DrawMode drawMode;
    public Noise.noiseAlgorithm NoiseAlgorithm;
    [SerializeField] private int seed;
    public bool autoUpdate;
    [Header("Noise")]
    [SerializeField] private int octaves;
    [Range(0f, 1f)] [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private float noiseScale;
    [Header("Map")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private Vector2 mapOffset;
    [Header("Mesh")]
    [SerializeField] private float heightMultiplier;
    [Header("GUI-images")]
    [SerializeField] private UnityEngine.UI.RawImage noiseTexture;
    [SerializeField] private UnityEngine.UI.RawImage colorTexture;
    [Header("Regions")]
    [SerializeField] private TerrainType[] terrainRegions;

    //Full hidden variables 
    private float[,] noiseMap;
    private Color[] colorMap;

    public void GenerateMap()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, mapOffset, NoiseAlgorithm);
        colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainRegions.Length; i++)
                {
                    if(currentHeight <= terrainRegions[i].maxHeight)
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
            mapDrawer.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    private void OnValidate()
    {
        //Set raw image as new texture
        if (drawMode == DrawMode.NoiseMap)
        {
            UpdateGUITextures(true, false);
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            UpdateGUITextures(false, true);
        }
        else if (drawMode == DrawMode.Mesh)
        {
            UpdateGUITextures(true, true);
        }

        //Clamp inspector-values
        if (mapWidth < 1)
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
    
    private void UpdateGUITextures(bool showNoiseTexture, bool showColorTexture)
    {
        if(showNoiseTexture)
        {
            noiseTexture.gameObject.SetActive(true);
            noiseTexture.texture = TextureGenerator.TextureFromHeightMap(noiseMap);
        }
        else if(!showNoiseTexture)
        {
            noiseTexture.gameObject.SetActive(false);
        }

        if (showColorTexture)
        {
            colorTexture.gameObject.SetActive(true);
            colorTexture.texture = TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight);
        }
        else if (!showColorTexture)
        {
            colorTexture.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    [SerializeField] private string name;
    public float maxHeight;
    public Color color;
}
