using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public bool autoUpdate;
    [SerializeField] private enum DrawMode { Mesh, NoiseMap, FalloffMap };
    [Header("General")]
    [SerializeField] private DrawMode drawMode;
    public Material terrainMaterial;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;
    [SerializeField] private Renderer textureRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Map")]
    [Range(0, MeshSettings.numSupportedLODs - 1)] [SerializeField] private int editorPreviewLOD; // LOD: 1, 2, 4, 8 . . .
    [Header("GUI-images")]
    [SerializeField] private UnityEngine.UI.RawImage noiseTexturePreview;
    [SerializeField] private UnityEngine.UI.RawImage colorTexturePreview;
    [SerializeField] private UnityEngine.UI.RawImage falloffTexturePreview;

    public void DrawMapInEditor()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerRow, meshSettings.numVertsPerRow, heightMapSettings, Vector2.zero);


        Texture2D noiseMap = null;
        Texture2D falloffMap = null;

        if (drawMode == DrawMode.NoiseMap)
        {
            noiseMap = TextureGenerator.TextureFromHeightMap(heightMap);
            DrawTexture(noiseMap);
        }
        else if (drawMode == DrawMode.Mesh)
        {
            if (heightMapSettings.useFalloffMap)
            {
                falloffMap = TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerRow), 0, 1));
            }

            noiseMap = TextureGenerator.TextureFromHeightMap(heightMap);
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            falloffMap = TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerRow), 0, 1));
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
        textureData.ApplyToMaterial(terrainMaterial);
    }

    private void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}
