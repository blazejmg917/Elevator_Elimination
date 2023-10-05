using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogStartEvent : UnityEvent{};
    [System.Serializable]
    public class DialogEndEvent : UnityEvent<bool>{};
    [Header("Other important script components")]
    public Typewriter typewriter;
    public DialogInputHandler inputHandler;
    [Header("dialog UI Objects")]
    [Tooltip("the overarching game object for dialog windows. used to turn on/off display")]
    public GameObject dialogObject;
    [Tooltip("Text box for dialog")]
    public TMP_Text dialogText;
    [Tooltip("Text box to say to press button to continue")]
    public TMP_Text continueText;
    [Space]
    [Header("dialog parameters")]
    [SerializeField, Tooltip("the current dialog node being processed. Here for editor reference only")]
    private DialogNode currentDialog;
    [SerializeField, Tooltip("if dialog is currently being displayed")]
    private bool displayingDialog = false;
    [SerializeField, Tooltip("if the current dialog is being used still")]
    private bool currentDialogRunning = false;
    [SerializeField, Tooltip("if the dialog manager is currently handling input and should not try to deal with any further input for the moment")]
    private bool handlingInput = false;
    [SerializeField, Tooltip("if the dialog manager can recieve any input currently")]
    private bool canTakeInput = false;

    private DialogNode starterDialog = null;

    [SerializeField, Tooltip("The event to play when starting dialog")]
    private DialogStartEvent dialogStart = new DialogStartEvent();
    [SerializeField, Tooltip("The event to play when starting dialog")]
    private DialogEndEvent dialogEnd = new DialogEndEvent();


    
    

    

    //for singleton
    private static DialogManager instance;

    public static DialogManager Instance
    {
        get{
            if(instance == null){
                instance = FindObjectOfType<DialogManager>();
                if(instance == null){
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<DialogManager>();
                }
            }
            return instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if(!typewriter){
            typewriter = GetComponent<Typewriter>();
            if(!typewriter){
                typewriter = gameObject.AddComponent<Typewriter>();
                Debug.LogWarning("NO TYPEWRITER COMPONENT FOUND IN DIALOG MANAGER. CREATING NEW TYPEWRITER COMPONENT, BUT SOME VALUES MAY BE INCORRECT. \nPLEASE ADD A TYPEWRITER COMPONENT TO THE DIALOG MANAGER");
            }
        }
        if(!typewriter.TextBoxSet()){
            typewriter.setDefaultTextbox(dialogText);
        }
        continueText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    

    public bool StartDialog(DialogNode thisDialog){
        dialogStart.Invoke();
        if(!thisDialog){
            return false;
        }
        Debug.Log("dialog manager has received dialog");
        displayingDialog = true;
        dialogObject.SetActive(true);
        inputHandler.AcceptInput(true);
        HandleDialog(thisDialog);
        return true;
    }

    private void HandleDialog(DialogNode thisDialog){
        continueText.gameObject.SetActive(false);
        currentDialog = thisDialog;
        if(thisDialog == null){
            Debug.Log("end of dialog reached");
            EndDialog();
            return;
        }
        
        if(!gameObject.activeSelf){
            EndDialog();
            Debug.Log("stopped breaker error");
            return;
        }
        StartCoroutine("DisplayDialog");

    }

    private void GoToNextDialog(){
        currentDialogRunning = true;
        HandleDialog(currentDialog.getNextNode());

    }

    IEnumerator DisplayDialog(){
        currentDialogRunning = true;
        if(currentDialog == null){
            yield break;
        }
        
        canTakeInput = true;
        yield return StartCoroutine(typewriter.Typewrite(currentDialog.GetSpecialTextBox(), currentDialog.getDialog(), currentDialog.GetSpecialTextSpeed()));

        
        currentDialogRunning = false;
        if(currentDialog && currentDialog.AutoContinue()){
            GoToNextDialog();
        }
        else if (!currentDialog){
            EndDialog();
        }
        else{
            canTakeInput = true;
            if(currentDialog && currentDialog.DisplayContinueText()){
                continueText.gameObject.SetActive(true);
            }
        }
    }

    //ends the current dialog as it reaches its natural end. safe
    public void EndDialog(bool invokeEvents = true){
        currentDialog = null;
        displayingDialog = false;
        handlingInput = false;
        currentDialogRunning = false;
        dialogObject.SetActive(false);
        inputHandler.AcceptInput(false);
        if(invokeEvents){
            dialogEnd.Invoke(false);
        }
    }

    //cancels the current dialog midway through and closes the dialog window. requires a few extra steps, but ensures safety
    public void CancelDialog(){
        typewriter.StopText();
        StopAllCoroutines();
        EndDialog();
    }


    public void HandleInput(){
        if(handlingInput){
            return;
        }
        handlingInput = true;
        if(displayingDialog && canTakeInput){
            if(currentDialogRunning){
                typewriter.SkipText();
                canTakeInput = false;
            }
            else{
                if(currentDialog.ContinueOnInput()){
                    canTakeInput = false;
                    GoToNextDialog();
                }
            }
        }
        handlingInput = false;
    }

}
