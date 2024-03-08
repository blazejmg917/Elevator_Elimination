using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFileLoader : MonoBehaviour
{
    [SerializeField, Tooltip("the filename of the level to load")] private string levelFileName;
    [SerializeField, Tooltip("must mark this true to overwrite an existing level")] private bool overwriteLevel = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLevel()
    {
        TileManager.Instance.UpdatePersonMap();
        TileManager.Instance.LoadLevelFromFile(levelFileName, out _, overwriteLevel);
        overwriteLevel = false;
    }
}
