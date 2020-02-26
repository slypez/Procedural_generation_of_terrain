using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode {NoiseMap, ColorMap, Mesh};
    [Header("General")]
    [SerializeField] private DrawMode drawMode;
    public Noise.noiseAlgorithm NoiseAlgorithm;
    [SerializeField] private Noise.NormalizeMode normalizeMode;
    [SerializeField] private int seed;
    public bool autoUpdate;
    [Header("Noise")]
    [SerializeField] private int octaves;
    [Range(0f, 1f)] [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private float noiseScale;
    [Header("Map")]
    [Range(0, 6)] [SerializeField] private int editorPreviewLOD; // LOD: 1, 2, 4, 8 . . .
    [SerializeField] private Vector2 mapOffset;
    public const int mapChunkSize = 241; // Highest factor of 2 + 1 under 255, MAX-vertices is 255x255
    [Header("Mesh")]
    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve meshHeightCurve;
    [Header("GUI-images")]
    [SerializeField] private UnityEngine.UI.RawImage noiseTexture;
    [SerializeField] private UnityEngine.UI.RawImage colorTexture;
    [Header("Regions")]
    [SerializeField] private TerrainType[] terrainRegions;

    //Full hidden variables 
    private float[,] noiseMap;
    private Color[] colorMap;
    private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDrawer mapDrawer = FindObjectOfType<MapDrawer>();
        if (drawMode == DrawMode.NoiseMap)
        {
            mapDrawer.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            mapDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            mapDrawer.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, meshHeightCurve, editorPreviewLOD), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if(meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private MapData GenerateMapData(Vector2 center)
    {
        noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, center + mapOffset, normalizeMode, NoiseAlgorithm);
        colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainRegions.Length; i++)
                {
                    if(currentHeight >= terrainRegions[i].maxHeight)
                    {
                        colorMap[y * mapChunkSize + x] = terrainRegions[i].color;   
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
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
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }

    private struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
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
            colorTexture.texture = TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize);
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

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
