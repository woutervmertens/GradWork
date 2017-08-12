using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(GeneratePaths))]
public class GeneratePathsEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GeneratePaths pathGenerator = (GeneratePaths) target;
        if (GUILayout.Button("Generate"))
        {
            pathGenerator.Generate();
        }
    }
}
