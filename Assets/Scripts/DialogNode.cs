using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogNode : MonoBehaviour
{
    public enum VarChangeTypes{
        ADD,
        SUBTRACT,
        SET_EQUAL
    }
    [Serializable]
    public struct VarChange{
        [Tooltip("The string id of the variable/Quest you want to change")]
        public string varId;
        [Tooltip("what operation you want to apply to this variable/quest status")]
        public VarChangeTypes operationType;
        [Tooltip("the value you want to add/subtract/set equal to")]
        public int changeAmount;
    }

    [Serializable]
    public struct FlagChange{
        [Tooltip("The string id of the flag you want to change")]
        public string flagId;
        [Tooltip("whether you want to set this flag to true or false")]
        public bool flagVal;
    }
    [Serializable]
    public class DialogEvent : UnityEvent{};
    [Header("default dialog node parameters")]
    [SerializeField, Tooltip("The text contained within this dialog node")]
    protected string contents;
    [SerializeField, Tooltip("a special text box this dialog should be displayed in. leave null if not used")]
    protected TMP_Text textBox;
    [SerializeField, Tooltip("a special typing rate (characters/second). leave at 0 or negative if default should be used")]
    protected float charPerSec = -1;
    [SerializeField, Tooltip("whether you can use standard input to move on from this dialog. \nShould always be set to true unless you have another way to continue (like a button)")]
    protected bool continueOnInput = true;
    [SerializeField, Tooltip("mark true if you want this dialog to pass to the next dialog without requiring input once it is finished")]
    protected bool autoContinue = false;
    [SerializeField, Tooltip("mark this value as true if this node is simply a dummy node that should not get its own text box\nCan be used to change variables/flags or to decide which dialog to show for conditionals/random choices\ndo not use for dialog choices, as the choices will not be displayed and text will stall.")]
    protected bool isDummy;
    protected DialogEvent dialogStartEvent = new DialogEvent();
    [SerializeField, Tooltip("events that are run once this dialog is completed. these are changed at the END of dialog ")]
    protected DialogEvent dialogEndEvent = new DialogEvent();
    [SerializeField, Tooltip("the next dialog node after this one")]private DialogNode nextNode;
    
    void Start(){
        
    }

    

    //check if this node is a dummy node
    public bool IsDummy(){
        return isDummy;
    }

    public bool ContinueOnInput(){
        return continueOnInput;
    }

    public bool AutoContinue(){
        return autoContinue;
    }
    
    //what to do when you reach the end of use and are prompted to get the next node
    public  DialogNode getNextNode(){
        return nextNode;
    }

    //get the text of this dialog node
    public virtual string getDialog(){
        return contents;
    }

    //get the special text box if there is one
    public TMP_Text GetSpecialTextBox(){
        return textBox;
    }

    //get the special text speed if there is one
    public float GetSpecialTextSpeed(){
        return charPerSec;
    }

    //checks if there are any special behaviors at the end of the text scroll
    public virtual bool OnEndTextScroll(out GameObject[] objectsToDisplay, bool runEvents = true){
        objectsToDisplay = null;
        if(runEvents){
            dialogEndEvent.Invoke();
        }
        return false;
    }

    //checks if there are any special behaviors at the start of the text scroll
    public virtual bool OnStartTextScroll(out GameObject[] objectsToDisplay, bool runEvents = true){
        objectsToDisplay = null;
        if(runEvents){
            dialogStartEvent.Invoke();
        }
        return false;
    }

    //checks if the continue text should be displayed. will always return true, but should be overriden by the choice node
    public virtual bool DisplayContinueText(){
        return true;
    }
}
