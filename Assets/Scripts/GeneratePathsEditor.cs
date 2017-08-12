using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(PathMapBuilder))]
public class GeneratePathsEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PathMapBuilder pathGenerator = (PathMapBuilder) target;
        if (GUILayout.Button("Generate"))
        {
            pathGenerator.Generate();
        }
    }
}
