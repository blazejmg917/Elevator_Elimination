using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelFileLoader))]
public class LevelFileLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelFileLoader lfc = (LevelFileLoader)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Load Level From File"))
        {
            if (lfc.IsSceneBound())
            {
                lfc.LoadLevel();
            }
        }

    }
}
