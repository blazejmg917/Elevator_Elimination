using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenUiDisplay : MonoBehaviour
{
    [SerializeField,Tooltip("the objects to display")]private List<GameObject> objectsToBeDisplayed = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

        if(objectsToBeDisplayed != null){
            foreach(GameObject go in objectsToBeDisplayed){
                go.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayObjects(){
        if(objectsToBeDisplayed != null){
            foreach(GameObject go in objectsToBeDisplayed){
                go.SetActive(true);
            }
        }
    }
}
