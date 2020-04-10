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
            if ((mapPreview.sampleRate <= 0 || mapPreview.sampleRate > 1) && (mapPreview.debugTraversability || mapPreview.debugNonTraversability))
            {
                Debug.LogError("You can only debug one mesh at a time. Change samples to 1 and it should work. Or turn of any debugging");
            }
            else
            {
                mapPreview.floodFillValues.Clear();
                int saveSeed = mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed;
                Transform debugContainer = mapPreview.previewMeshFilter.transform.GetChild(0);

                for (int i = 0; i < mapPreview.sampleRate; i++)
                {
                    mapPreview.floodFillValues.Add(FloodFill.GenerateFloodFillData(mapPreview.floodFillHeightThresholdValue, mapPreview.previewMeshFilter, debugContainer, mapPreview.debugMaterialWalking, mapPreview.debugMaterialJumping, mapPreview.debugMaterialNotReachable, mapPreview.includeJumpTraversability, mapPreview.floodFillJumpHeightThresholdValue, mapPreview.debugTraversability, mapPreview.debugNonTraversability));
                    mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = Random.Range(0, int.MaxValue);
                    mapPreview.DrawMapInEditor();
                }
                mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = saveSeed;
                mapPreview.DrawMapInEditor();
                DataAnalyzer.AnalyzeAndShowData(mapPreview.floodFillValues, DataAnalyzer.valueRepresentation.Traversability, DataAnalyzer.numberForm.Percent, mapPreview.floatingPointPrecisionPreview);
            }
        }
        if (GUILayout.Button("Clear flood-fill debug"))
        {
            FloodFill.CheckIfDestroyOldDebugArea(mapPreview.previewMeshFilter.transform.GetChild(0));
        }

        if (GUILayout.Button("Generate algorithm exectiontime stats"))
        {
            mapPreview.executionTimeValues.Clear();
            float executionTime = 0f;
            int saveSeed = mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed;

            for (int i = 0; i < mapPreview.sampleRate; i++)
            {
                DataAnalyzer.stopwatch.Reset();
                mapPreview.DrawMapInEditor();
                switch (mapPreview.unitOfTime)
                {
                    case DataAnalyzer.unitOfTime.None:
                        executionTime = DataAnalyzer.stopwatch.Elapsed.Ticks; // Change this if None should be units instead.
                        break;
                    case DataAnalyzer.unitOfTime.Milliseconds:
                        executionTime = (float)DataAnalyzer.stopwatch.Elapsed.TotalMilliseconds;
                        break;
                    case DataAnalyzer.unitOfTime.Seconds:
                        executionTime = (float)DataAnalyzer.stopwatch.Elapsed.TotalSeconds;
                        break;
                    case DataAnalyzer.unitOfTime.Minutes:
                        executionTime = (float)DataAnalyzer.stopwatch.Elapsed.TotalMinutes;
                        break;
                    case DataAnalyzer.unitOfTime.Hours:
                        executionTime = (float)DataAnalyzer.stopwatch.Elapsed.TotalHours;
                        break;
                }
                mapPreview.executionTimeValues.Add(executionTime);
                mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = Random.Range(0, int.MaxValue);
            }
            mapPreview.maps[mapPreview.mapIndexSelector].heightMapSettings.noiseSettings.seed = saveSeed;
            mapPreview.DrawMapInEditor();

            DataAnalyzer.AnalyzeAndShowData(mapPreview.executionTimeValues, DataAnalyzer.valueRepresentation.ExecutionTime, DataAnalyzer.numberForm.Decimal, mapPreview.floatingPointPrecisionPreview, mapPreview.unitOfTime);
        }
    }
}
