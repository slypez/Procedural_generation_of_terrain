using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;

        if (DrawDefaultInspector())
        {
            if (mapPreview.autoUpdate == true)
            {
                mapPreview.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate terrain"))
        {
            mapPreview.DrawMapInEditor();
        }

        if (GUILayout.Button("Generate flood-fill stats"))
        {
            float floodFillAverageTraversal = 0f;
            for (int i = 0; i < mapPreview.floodFillSampleRate; i++)
            {
                floodFillAverageTraversal += FloodFill.GenerateFloodFillData(mapPreview.floodFillHeightThresholdValue, mapPreview.previewMeshFilter);

                mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = Random.Range(0, int.MaxValue);
                mapPreview.DrawMapInEditor();
            }

            floodFillAverageTraversal /= mapPreview.floodFillSampleRate;
            Debug.Log("Traversal of terrain is: " + (floodFillAverageTraversal * 100f) + "%" + " with a samplerate of " + mapPreview.floodFillSampleRate);
        }
    }
}
