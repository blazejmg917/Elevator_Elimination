using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelCreator lc = (LevelCreator)target;
       
        base.OnInspectorGUI();
        if (GUILayout.Button("Setup Level"))
        {
            if (lc.IsSceneBound())
            {
                lc.SetupLevel();
            }
        }

        // if (GUILayout.Button("Display Paths"))
        // {
        //     if (zpg.IsSceneBound())
        //     {
        //         zpg.DebugLines();
        //     }
        // }
    }
}
