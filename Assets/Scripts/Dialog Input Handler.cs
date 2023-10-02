using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogInputHandler : MonoBehaviour
{
    [SerializeField, Tooltip("All input actions that can be used to click through dialog")]
    private List<InputActionProperty> dialogContinueActions = new List<InputActionProperty>();
    [SerializeField]private bool inputreceived = false;
    [SerializeField, Tooltip("if this can accept input to continue dialog")]
    private bool canAcceptInput = false;

    void Update(){
        if(canAcceptInput){
            foreach(InputActionProperty action in dialogContinueActions){
                if(action.action.ReadValue<float>() > .1f){
                    if(!inputreceived){
                        DialogManager.Instance.HandleInput();
                        inputreceived = true;
                    }
                    return;
                }
            }
        }
        inputreceived = false;
    }

    public void AcceptInput(bool accept){
        canAcceptInput = accept;
    }
}
