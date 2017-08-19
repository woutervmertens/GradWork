using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(GeneralManager))]
public class LoadVehicleModelsEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GeneralManager generalManager = (GeneralManager) target;
        if (GUILayout.Button("Load Vehicle Models from Resources"))
        {
            generalManager.LoadInVehicleDictionary();
        }
    }
}
