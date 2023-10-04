using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHolder : MonoBehaviour
{
    [System.Serializable]public struct TutorialDialog{
        [Tooltip("the index of the level this tutorial text is attached to")]public int level;
        [Tooltip("the dialog holder for the tutorial text")]public DialogHolder dialog;
    }
    [SerializeField, Tooltip("all of the tutorial dialogs for levels that need them")]private List<TutorialDialog> tutorialDialogs = new List<TutorialDialog>();
    private Dictionary<int, DialogHolder> tutorialDict = new Dictionary<int, DialogHolder>();
    //used to store if a tutorial has already been played for a level so it doesn't repeat when you fail a level (cleared when you go back to the main menu)
    private Dictionary<int, bool> tutorialsPlayed = new Dictionary<int, bool>();
    // Start is called before the first frame update
    void Start()
    {
        foreach(TutorialDialog t in tutorialDialogs){
            tutorialDict.Add(t.level,t.dialog);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/// <summary>
/// checks if there is a tutorial node for the given level and if it needs to be played
/// </summary>
/// <param name="level">the id of the level to search for. one-indexed</param>
/// <param name="tNode">the dialog node for the tutorial if it should be played</param>
/// <returns>true if there is a tutorial and it has not been played yet. false otherwise</returns>
    public bool getTutorialNode(int level, out DialogNode tNode){
        tNode = null;
        DialogHolder tHolder;
        bool playedBefore;
        if(tutorialsPlayed.TryGetValue(level, out playedBefore)){
            return false;
        }
        tutorialsPlayed.Add(level,true);
        if(tutorialDict.TryGetValue(level, out tHolder)){
            tNode = tHolder.GetStarterNode();
            return true;
        }
        return false;
    }

}
