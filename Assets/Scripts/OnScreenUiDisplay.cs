using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenUiDisplay : MonoBehaviour
{
    [SerializeField,Tooltip("the objects to display")]private GameObject objectToBeDisplayed;
    // Start is called before the first frame update
    void Start()
    {
        if(!objectToBeDisplayed){
            objectToBeDisplayed = gameObject;
        }
        if(objectToBeDisplayed){
            objectToBeDisplayed.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayObjects(){
        if(objectToBeDisplayed){
            objectToBeDisplayed.SetActive(true);
        }
    }
}
