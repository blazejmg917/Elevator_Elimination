using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LevelStructure : MonoBehaviour
{
    [SerializeField, Tooltip("the name of this level")]
    private string levelName = "";
    [SerializeField, Tooltip("the creator of this level")]
    private string creatorName = "";
    [SerializeField, Tooltip("the level structure of tiles"), ReadOnly(true)] private Tile[,] tiles = new Tile[,] { };
    [SerializeField, Tooltip("the level structure of tiles"), ReadOnly(true)] private List<TileManager.ListWrapper<Tile>> tileList = new List<TileManager.ListWrapper<Tile>>();
    [SerializeField, Tooltip("the number of floors for this level")] private int floors = 20;
    //[SerializeField, Tooltip("How many targets there are in this level")]private int numTargets = 1;
    // Start is called before the first frame update
    public Tile[,] GetTiles()
    {
        return tiles;
    }
    public List<TileManager.ListWrapper<Tile>> GetTileList(){
        return tileList;
    }
    public void SetTiles(Tile[,] tileStructure)
    {
        Debug.Log("setting tiles: " + tileStructure + ", " + tileStructure.GetLength(0));
        tiles = tileStructure;
    }
    public void SetTiles(List<TileManager.ListWrapper<Tile>> tileStructure)
    {
        //Debug.Log("setting tiles: " + tileStructure + ", " + tileStructure.GetLength(0));
        tileList = tileStructure;
    }

    public bool HasTiles()
    {
        return tiles != null && tiles.Length != 0;
    }

    public int GetFloors(){
        return floors;
    }

    public string GetLevelName()
    {
        return levelName;
    }
    public string GetCreator()
    {
        return levelName;
    }

    public void SetLevelName(string newName)
    {
        levelName = newName;
    }

    public void SetCreatorName(string newName)
    {
        creatorName = newName;
    }


    // public int GetNumTargets(){
    //     return numTargets;
    // }
}
