using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
    public string name;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;
    public Material skyBox;
}

public class MapPreview : MonoBehaviour
{
    public bool autoUpdate;
    [SerializeField] private enum DrawMode { Mesh, NoiseMap, FalloffMap };
    [Header("General")]
    [SerializeField] private DrawMode drawMode;
    public Material terrainMaterial;
    [Header("Preview")]
    public MeshFilter previewMeshFilter;
    [SerializeField] private Renderer previewTextureRenderer;
    [SerializeField] private MeshRenderer previewMeshRenderer;
    [Header("Testing")]
    public bool WriteValuesToTextFile;
    public DataAnalyzer.LogContent logContent;
    [Tooltip("Amount of tests done in a single execution")] public int TestAmount;
    [Tooltip("Amount of samples taken in a single test")] public int sampleRate;
    public int floatingPointPrecisionPreview;
    [Header("Flood-fill")]
    public bool debugTraversability;
    public bool debugNonTraversability;
    public bool includeJumpTraversability;
    public float floodFillHeightThresholdValue;
    public float floodFillJumpHeightThresholdValue;
    [HideInInspector] public List<float> floodFillValues = new List<float>();
    public Material debugMaterialWalking;
    public Material debugMaterialJumping;
    public Material debugMaterialNotReachable;
    [Header("Execution time")]
    public DataAnalyzer.unitOfTime unitOfTime;
    [HideInInspector] public List<float> executionTimeValues = new List<float>();
    [Header("Map")]
    public int mapIndexSelector;
    [Range(0, MeshSettings.numSupportedLODs - 1)] [SerializeField] private int editorPreviewLOD; // LOD: 1, 2, 4, 8 . . .
    public List<Map> maps = new List<Map>();

    public void DrawMapInEditor()
    {
        maps[mapIndexSelector].textureData.ApplyToMaterial(terrainMaterial);
        maps[mapIndexSelector].textureData.UpdateMeshHeights(terrainMaterial, maps[mapIndexSelector].heightMapSettings.minHeight, maps[mapIndexSelector].heightMapSettings.maxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(maps[mapIndexSelector].meshSettings.numVertsPerRow, maps[mapIndexSelector].meshSettings.numVertsPerRow, maps[mapIndexSelector].heightMapSettings, Vector2.zero);
        RenderSettings.skybox = maps[mapIndexSelector].skyBox;

        Texture2D noiseMap = null;
        Texture2D falloffMap = null;

        if (drawMode == DrawMode.NoiseMap)
        {
            noiseMap = TextureGenerator.TextureFromHeightMap(heightMap);
            DrawTexture(noiseMap);
        }
        else if (drawMode == DrawMode.Mesh)
        {
            if (maps[mapIndexSelector].heightMapSettings.useFalloffMap)
            {
                //falloffMap = TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(maps[mapIndexSelector].meshSettings.numVertsPerRow), 0, 1));
            }

            noiseMap = TextureGenerator.TextureFromHeightMap(heightMap);
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, maps[mapIndexSelector].meshSettings, editorPreviewLOD));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            //falloffMap = TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(maps[mapIndexSelector].meshSettings.numVertsPerRow), 0, 1));
            DrawTexture(falloffMap);
        }
        //Update GUI-images here
    }

    public void DrawTexture(Texture2D texture)
    {
        previewTextureRenderer.sharedMaterial.mainTexture = texture;
        previewTextureRenderer.transform.localScale = new Vector3(texture.width, 1, -texture.height) / 10f;

        previewTextureRenderer.gameObject.SetActive(true);
        previewMeshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        if(previewMeshFilter != null)
        {
            previewMeshFilter.sharedMesh = meshData.CreateMesh();
        }

        if (previewTextureRenderer != null)
        {
            previewTextureRenderer.gameObject.SetActive(false);
        }
        if(previewMeshFilter != null)
        {
            previewMeshFilter.gameObject.SetActive(true);
        }
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    private void OnTextureValuesUpdated()
    {
        maps[mapIndexSelector].textureData.ApplyToMaterial(terrainMaterial);
    }

    private void OnValidate()
    {
        if (maps[mapIndexSelector].meshSettings != null)
        {
            maps[mapIndexSelector].meshSettings.OnValuesUpdated -= OnValuesUpdated;
            maps[mapIndexSelector].meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (maps[mapIndexSelector].heightMapSettings != null)
        {
            maps[mapIndexSelector].heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            maps[mapIndexSelector].heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (maps[mapIndexSelector].textureData != null)
        {
            maps[mapIndexSelector].textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            maps[mapIndexSelector].textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        //Clamp map-index
        if(mapIndexSelector < 0)
        {
            mapIndexSelector = 0;
        }
        if(mapIndexSelector > maps.Count - 1)
        {
            mapIndexSelector = maps.Count - 1;
        }
    }
}
