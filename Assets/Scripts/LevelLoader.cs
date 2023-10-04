using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField, Tooltip("the level structure to load")] private LevelStructure loadStructure;
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
        TileManager.Instance.LoadLevelList(loadStructure,overwriteLevel);
        overwriteLevel = false;
    }
}
