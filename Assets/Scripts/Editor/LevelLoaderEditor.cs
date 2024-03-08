using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelLoader))]
public class LevelLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelLoader lc = (LevelLoader)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Setup Level"))
        {
            if (lc.IsSceneBound())
            {
                lc.LoadLevel();
            }
        }

    }
}
