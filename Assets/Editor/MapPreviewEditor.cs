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
            FloodFill.GenerateFloodFillData(mapPreview.previewMeshFilter.sharedMesh.vertices);
        }
    }
}
