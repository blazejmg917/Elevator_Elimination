using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    private TileManager tileMan;
    [SerializeField, Tooltip("the width of the level to create"), Min(1)] private int width = 4;
    [SerializeField, Tooltip("the height of the level to create"), Min(1)] private int height = 4;
    [SerializeField, Tooltip("must mark this true to overwrite an existing level")] private bool overwriteLevel = false;
    // Start is called before the first frame update
    void Start()
    {
        tileMan = TileManager.Instance;
    }

    public void SetupLevel()
    {
        if (!tileMan)
        {
            tileMan = TileManager.Instance;
        }
        tileMan.SetupElevator(width, height, overwriteLevel);
        overwriteLevel = false;
    }
}
