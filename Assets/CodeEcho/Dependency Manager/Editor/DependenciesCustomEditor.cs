using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dependencies))]
public class DependenciesCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (Dependencies)target;

        if (GUILayout.Button("Run Update", GUILayout.Height(40)))
        {
            script.Update();
        }

        if (GUILayout.Button("Tidy Up", GUILayout.Height(40)))
        {
            script.TidyUp();
        }

    }
}
