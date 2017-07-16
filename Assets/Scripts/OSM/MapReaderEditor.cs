using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(MapReader))]
public class MapReaderEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapReader mapReader = (MapReader) target;
        if (GUILayout.Button("Generate"))
        {
            mapReader.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            mapReader.Clear();
        }
    }
}
