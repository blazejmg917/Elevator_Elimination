using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsHolder : MonoBehaviour
{
    [SerializeField, Tooltip("A list of all levels in the game. Put these in order")]private List<LevelStructure> gameLevels = new List<LevelStructure>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LevelStructure getLevelById(int levelId){
        if(gameLevels.Count <= levelId || levelId < 0){
            Debug.LogWarning("game does not have " + levelId + " levels");
            return null;
        }
        return gameLevels[levelId];
    }
}
