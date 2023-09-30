using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField, Tooltip("the array of all current tiles")] private Tile[,] tiles;
    //[SerializeField, Tooltip("the lists of all current tiles")]private List<List<Tile>> tilesList = new List<List<Tile>>();
    [SerializeField, Tooltip("the tile prefab")] private GameObject tilePrefab;
    [SerializeField, Tooltip("tile holder object")] private GameObject tileHolder;
    [SerializeField, Tooltip("the size of each tile object")] private Vector3 tileSize;
    [SerializeField, Tooltip("the starting position of the first tile")] private Vector3 tileStart;
    [SerializeField, Tooltip("the tile structure file to load in")] private LevelStructure baseLevel;
    [SerializeField, Tooltip("the starting tile for the player to enter and exit from")] private Tile startTile;

    private static TileManager _instance;
    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TileManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<TileManager>();
                    Debug.Log("Generating new game manager");
                }
            }
            return _instance;
        }
    }

    public void LoadLevel(LevelStructure levelStructure, bool setStructureAsTileHolder = false)
    {
        baseLevel = levelStructure;
        if (setStructureAsTileHolder)
        {
            tileHolder = levelStructure.gameObject;
        }
        LoadLevel(baseLevel.GetTiles());
       
    }
    public void LoadLevel(List<List<Tile>> tileList)
    {
        if(tileList == null || tileList.Count == 0)
        {
            return;
        }
        int iMax = tileList.Count;
        int jMax = tileList[0].Count;
        foreach(List<Tile> list in tileList)
        {
            if(list.Count > jMax)
            {
                jMax = list.Count;
            }
        }
        tiles = new Tile[iMax,jMax];
        int i = 0;
        foreach(List<Tile> list in tileList)
        {
            int j = 0;
            foreach(Tile tile in list)
            {
                tiles[i, j] = tile;
                j++;
            }
            i++;
        }
        LinkTiles();
    }
    public void LoadLevel(Tile[,] levelArray)
    {
        Debug.Log("loading level from array: " + levelArray);
        tiles = levelArray;
        LinkTiles();
    }

    public void LinkTiles()
    {
        Debug.Log("linking tiles: " + tiles.GetLength(0) + ", " + tiles.GetLength(1));
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if(i > 0)
                {
                    tiles[i, j].SetTop(tiles[i - 1, j]);
                }
                if (i < tiles.GetLength(0) - 1) 
                {
                    tiles[i, j].SetBottom(tiles[i + 1, j]);
                }

                if (j > 0)
                {
                    tiles[i, j].SetLeft(tiles[i, j-1]);
                }
                if (j < tiles.GetLength(1) - 1)
                {
                    tiles[i, j].SetRight(tiles[i, j+1]);

                }

            }
        }
        startTile = tiles[tiles.GetLength(0) / 2, tiles.GetLength(1) / 2];
    }

    private Tile GetTile(int x, int y)
    {
        return tiles[x, y];
    }
    private Tile GetTile(int[] coords)
    {
        return GetTile(coords[0], coords[1]);
    }

    public void SetupElevator(int xSize, int ySize, bool overridePrevVals)
    {
        if (tiles != null)
        {
            if (!overridePrevVals)
            {
                Debug.LogWarning("Trying to override previous values through inspector");
                return;
            }
            else
            {
                ClearLevel();
            }
        }

        if (!tileHolder)
        {
            tileHolder = new GameObject("Tile Holder");
            tileHolder.transform.parent = transform;
        }

        tiles = new Tile[xSize, ySize];
        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                GameObject newTileObj = Instantiate(tilePrefab, new Vector3(tileStart.x + tileSize.x * i, tileStart.y, tileStart.z + tileSize.z * j), tilePrefab.transform.rotation, tileHolder.transform);
                Tile newTile = newTileObj.GetComponent<Tile>();
                if (!newTile)
                {
                    newTile = newTileObj.AddComponent<Tile>();
                }
                tiles[i, j] = newTile;
            }
        }
        SaveLevelToStructure(overridePrevVals);
        LinkTiles();
    }

    private void SaveLevelToStructure(bool overridePrevVals)
    {
        if (!tileHolder)
        {
            tileHolder = new GameObject();
        }
        if (!baseLevel)
        {
            baseLevel = tileHolder.GetComponent<LevelStructure>();
            if (!baseLevel)
            {
                baseLevel = tileHolder.AddComponent<LevelStructure>();
            }
        }
        if (baseLevel.HasTiles() && !overridePrevVals)
        {
            
            Debug.LogWarning("Trying to override previous values through inspector");
            return;
            
        }
        else
        {
            baseLevel.SetTiles(tiles);
        }

    }

    private void ClearLevel()
    {
        if (tiles != null && tiles.Length > 0)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i, j])
                    {
                        DestroyImmediate(tiles[i, j].gameObject);
                    }
                }
            }
        }

        tiles = null;
        if (tileHolder)
        {
            foreach (Transform t in tileHolder.transform.GetComponentInChildren<Transform>())
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }

    public bool UpdateLevel()
    {
        bool gameStillRunning = true;
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                Person tilePerson = tiles[i, j].GetPerson();
                if (tilePerson)
                {
                    if (!tilePerson.OnFloorChange())
                    {
                        gameStillRunning = false;
                    }
                    
                }

            }
        }
        return gameStillRunning;
    }

    public Tile GetStartTile()
    {
        return startTile;
    }
}
