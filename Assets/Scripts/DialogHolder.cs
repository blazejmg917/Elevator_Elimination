using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHolder : MonoBehaviour
{
    [SerializeField, Tooltip("the starter node for this dialog")]private DialogNode starterNode;
    // Start is called before the first frame update
    void Start()
    {
        if(!starterNode){
            Debug.LogError("no starter node set. Will attempt to autoconnect, but could start dialog in the wrong spot");
            starterNode = gameObject.GetComponent<DialogNode>();
        }
    }

    public DialogNode GetStarterNode(){
        return starterNode;
    }
}
