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
            mapPreview.floodFillValues.Clear();
            int saveSeed = mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed;

            for (int i = 0; i < mapPreview.floodFillSampleRate; i++)
            {
                mapPreview.floodFillValues.Add(FloodFill.GenerateFloodFillData(mapPreview.floodFillHeightThresholdValue, mapPreview.previewMeshFilter, mapPreview.debugMaterialWalking, mapPreview.debugMaterialJumping, mapPreview.debugMaterialNotReachable, mapPreview.includeJumpTraversability, mapPreview.floodFillJumpHeightThresholdValue, mapPreview.debugTraversability, mapPreview.debugNonTraversability));
                mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = Random.Range(0, int.MaxValue);
                mapPreview.DrawMapInEditor();
            }
            mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = saveSeed;
            mapPreview.DrawMapInEditor();
            DataAnalyzer.AnalyzeAndShowData(mapPreview.floodFillValues, DataAnalyzer.valueRepresentation.Traversability, DataAnalyzer.numberForm.Percent, mapPreview.floatingPointPrecisionPreview);
        }
    }
}
