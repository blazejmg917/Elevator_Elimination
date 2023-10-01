using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilePersonSpawner))]
public class TileEdtor : Editor
{
    public override void OnInspectorGUI()
    {
        TilePersonSpawner tps = (TilePersonSpawner)target;
       
        base.OnInspectorGUI();
        if (GUILayout.Button("Spawn Object"))
        {
            tps.SpawnObjectOnTile();
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
