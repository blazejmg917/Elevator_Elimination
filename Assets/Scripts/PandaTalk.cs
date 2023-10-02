using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaTalk : MonoBehaviour
{
    [SerializeField]DialogNode LevelWinDialog;
    [SerializeField]DialogNode LevelLoseDialog;
    [SerializeField]DialogNode LastLevelWinDialog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayWinDialog(){
        DialogManager.Instance.StartDialog(LevelWinDialog);
        MusicScript.Instance.PandaStaticSFX();
    }
    public void DisplayFinalWinDialog(){
        DialogManager.Instance.StartDialog(LastLevelWinDialog);
        MusicScript.Instance.PandaStaticSFX();
    }
    public void DisplayLoseDialog(){
        DialogManager.Instance.StartDialog(LevelLoseDialog);
        MusicScript.Instance.PandaStaticSFX();
    }
}
