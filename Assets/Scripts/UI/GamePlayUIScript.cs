using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUIScript : MonoBehaviour
{
    [SerializeField, Tooltip("the canvas object")]private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        if(!canvas){
            canvas = GetComponent<Canvas>();
        }
        Debug.Log("start in camera");
            GetCameraFromGameManager();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCamera(Camera thisCam){
        canvas.worldCamera = thisCam;
        Debug.Log(canvas.worldCamera);
    }

    public void GetCameraFromGameManager(){
        SetCamera(GameManager.Instance.GetCamera());
    }
}
