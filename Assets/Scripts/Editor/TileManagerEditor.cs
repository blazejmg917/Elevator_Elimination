using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileManager tm = (TileManager)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Relink Tiles"))
        {
            if (tm.IsSceneBound())
            {
                tm.LinkTiles();
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
