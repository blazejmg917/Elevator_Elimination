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
        DisplayDialog(LevelWinDialog);
    }
    public void DisplayFinalWinDialog(){
        DisplayDialog(LastLevelWinDialog);
    }
    public void DisplayLoseDialog(){
        DisplayDialog(LevelLoseDialog);
    }

    public void DisplayDialog(DialogNode node){
        if(!gameObject.activeSelf){
            Debug.Log("displaying object cancelled by retry");
            return;
        }
        DialogManager.Instance.StartDialog(node);
        MusicScript.Instance.PandaStaticSFX();
    }
}
