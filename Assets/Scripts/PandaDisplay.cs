using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PandaDisplay : MonoBehaviour
{
    [System.Serializable]
    public class PandaFinishEvent : UnityEvent{};

    public enum TalkType{
        WIN,
        LOSE,
        WINFINAL
    }

    [SerializeField, Tooltip("the delay between the panda screen spawning and the panda text displaying")]private float pandaTalkStartDelay = 1.3f;
    [SerializeField, Tooltip("the delay between the panda screen spawning and the panda text displaying")]private float pandaTalkEndDelay = 1.5f;
    private float pandaTimer = 0;
    private bool pandaWaitingToTalk;
    private bool pandaWaitingToLeave;
    [SerializeField, Tooltip("the panda screen object")]private GameObject pandaScreen;
    [SerializeField, Tooltip("the panda screen animator")]private Animator pandaScreenAnim;
    [SerializeField, Tooltip("the panda talk script")]private PandaTalk pandaTalk;
    [SerializeField, Tooltip("event for when panda is done displaying")]private PandaFinishEvent pandaComplete = new PandaFinishEvent();
    [SerializeField, Tooltip("which type of dialog to start for the pands")]private TalkType talkType;
    // Start is called before the first frame update
    void Start()
    {
        CancelPandaInstant();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(pandaTimer > 0){
            pandaTimer -= Time.fixedDeltaTime;
        }
        else{
            if(pandaWaitingToLeave){
                pandaWaitingToLeave=false;
                if(talkType != TalkType.LOSE){
                    pandaComplete.Invoke();
                }
                pandaScreen.SetActive(false);
                gameObject.SetActive(false);
            }
            if(pandaWaitingToTalk){
                pandaWaitingToTalk = false;
                switch(talkType){
                    case TalkType.WIN:
                        pandaTalk.DisplayWinDialog();
                        break;
                    case TalkType.WINFINAL:
                        pandaTalk.DisplayFinalWinDialog();
                        break;
                    case TalkType.LOSE:
                        pandaTalk.DisplayLoseDialog();
                        break;
                }
            }
        }
    }


    public void ShowPandaWin(bool finalWin){
        if(pandaWaitingToLeave){
            return;
        }
        gameObject.SetActive(true);
        pandaScreen.SetActive(true);
        //start panda animation
        if(finalWin){
            talkType = TalkType.WINFINAL;
        }
        else{
            talkType = TalkType.WIN;
        }
        pandaWaitingToTalk = true;
        pandaTimer = pandaTalkStartDelay;
    }

    public void ShowPandaLoss(){
        if(pandaWaitingToLeave){
            return;
        }
        gameObject.SetActive(true);
        pandaScreen.SetActive(true);
        talkType = TalkType.LOSE;
        pandaWaitingToTalk = true;
        pandaTimer = pandaTalkStartDelay;
    }

    public void CancelPanda(bool callEndDialog = true){
        if(pandaWaitingToLeave){
            return;
        }
        if(callEndDialog){
            DialogManager.Instance.EndDialog();
        }
        pandaScreenAnim.SetTrigger("CloseWindow");
        //start screen turn off visual
        pandaTimer = pandaTalkEndDelay;
        pandaWaitingToLeave = true;
        
    }
    public void CancelPandaInstant(){
        Debug.Log("kill the fucking panda");
        DialogManager.Instance.EndDialog(false);
        //start screen turn off visual
        pandaWaitingToLeave=false;
                //pandaComplete.Invoke();
        pandaScreen.SetActive(false);
        gameObject.SetActive(false);
        
    }
}
