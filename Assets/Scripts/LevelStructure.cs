using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LevelStructure : MonoBehaviour
{
    [SerializeField, Tooltip("the level structure of tiles"), ReadOnly(true)] private Tile[,] tiles = new Tile[,] { };
    [SerializeField, Tooltip("the number of floors for this level")] private int floors = 20;
    // Start is called before the first frame update
    public Tile[,] GetTiles()
    {
        return tiles;
    }
    public void SetTiles(Tile[,] tileStructure)
    {
        Debug.Log("setting tiles: " + tileStructure + ", " + tileStructure.Length);
        tiles = tileStructure;
    }

    public bool HasTiles()
    {
        return tiles != null && tiles.Length != 0;
    }
}
