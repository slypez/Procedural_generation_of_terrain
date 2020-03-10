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
}

public class MapPreview : MonoBehaviour
{
    public bool autoUpdate;
    [SerializeField] private enum DrawMode { Mesh, NoiseMap, FalloffMap };
    [Header("General")]
    [SerializeField] private DrawMode drawMode;
    public Material terrainMaterial;
    [SerializeField] private Renderer textureRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [Header("Preview-images")]
    [SerializeField] private UnityEngine.UI.RawImage noiseTexturePreview;
    [SerializeField] private UnityEngine.UI.RawImage colorTexturePreview;
    [SerializeField] private UnityEngine.UI.RawImage falloffTexturePreview;
    [Header("Map")]
    [SerializeField] private int mapIndexSelector;
    [Range(0, MeshSettings.numSupportedLODs - 1)] [SerializeField] private int editorPreviewLOD; // LOD: 1, 2, 4, 8 . . .
    [SerializeField] private List<Map> maps = new List<Map>();

    public void DrawMapInEditor()
    {
        maps[mapIndexSelector].textureData.ApplyToMaterial(terrainMaterial);
        maps[mapIndexSelector].textureData.UpdateMeshHeights(terrainMaterial, maps[mapIndexSelector].heightMapSettings.minHeight, maps[mapIndexSelector].heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(maps[mapIndexSelector].meshSettings.numVertsPerRow, maps[mapIndexSelector].meshSettings.numVertsPerRow, maps[mapIndexSelector].heightMapSettings, Vector2.zero);


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
                falloffMap = TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(maps[mapIndexSelector].meshSettings.numVertsPerRow), 0, 1));
            }

            noiseMap = TextureGenerator.TextureFromHeightMap(heightMap);
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, maps[mapIndexSelector].meshSettings, editorPreviewLOD));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            falloffMap = TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(maps[mapIndexSelector].meshSettings.numVertsPerRow), 0, 1));
            DrawTexture(falloffMap);
        }

        UpdatePreviewTextures(noiseMap, null, falloffMap);
    }

    private void UpdatePreviewTextures(Texture2D noiseMap = null, Texture2D colorMap = null, Texture2D falloffMap = null)
    {
        if (noiseMap != null)
        {
            noiseTexturePreview.texture = noiseMap;
            noiseTexturePreview.gameObject.SetActive(true);
        }
        else
        {
            noiseTexturePreview.gameObject.SetActive(false);
        }

        if (colorMap != null)
        {
            colorTexturePreview.texture = colorMap;
            colorTexturePreview.gameObject.SetActive(true);
        }
        else
        {
            colorTexturePreview.gameObject.SetActive(false);
        }

        if (falloffMap != null)
        {
            falloffTexturePreview.texture = falloffMap;
            falloffTexturePreview.gameObject.SetActive(true);
        }
        else
        {
            falloffTexturePreview.gameObject.SetActive(false);
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        if(meshFilter != null)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
        }

        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
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
