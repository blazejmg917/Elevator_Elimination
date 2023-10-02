using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CameraFade : MonoBehaviour
{
    [System.Serializable]
    public class EndFadeInEvent : UnityEvent<bool>{};
    [System.Serializable]
    public class EndFadeOutEvent : UnityEvent<bool>{};
    [SerializeField, Tooltip("The camera blocking object")]private Renderer fadeBlackRenderer;
    [SerializeField, Tooltip("default fade length")]private float defaultFadeTime;
    private float fadeTime;
    private float fadeTimer;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private bool sceneChange = false;
    [SerializeField, Tooltip("event that is called whenever the fade in completes")]private EndFadeInEvent fadeInEvent = new EndFadeInEvent();
    [SerializeField, Tooltip("event that is called whenever the fade out completes")]private EndFadeOutEvent fadeOutEvent = new EndFadeOutEvent();
    private Color blackColor = Color.black;
    [SerializeField, Tooltip("Will show up at game start if marked true")]private bool displayOnGameStart = true;
    // Start is called before the first frame update
    void Start()
    {
        if(!fadeBlackRenderer){
            fadeBlackRenderer = GetComponent<Renderer>();
        }
        if(displayOnGameStart){
            fadeBlackRenderer.enabled = true;
        }
        //fadeTime = deaf
    }

    void FixedUpdate(){

        if(fadingIn){
            float t = fadeTimer / fadeTime;
            
            fadeBlackRenderer.material.color = new Color(blackColor.r, blackColor.g, blackColor.b, Mathf.Lerp(0,1,t));
            //Debug.Log("t: " + t);
        }
        else if(fadingOut){
            float t = fadeTimer / fadeTime;
            t = 1-t;
            fadeBlackRenderer.material.color = new Color(blackColor.r, blackColor.g, blackColor.b, Mathf.Lerp(0,1,t));
        }
        fadeTimer -= Time.fixedDeltaTime;
        if(fadeTimer < 0){
            if(fadingIn){
                fadingIn = false;
                fadeInEvent.Invoke(sceneChange);
                sceneChange = false;
            }
            else if(fadingOut){
                fadingOut = false;
                
                fadeOutEvent.Invoke(sceneChange);
                sceneChange = false;
            }
        }
    }

    public void EndLevel(){
        StartFadeOut();
    }


    public void StartFadeIn(float thisFadeTime = -1, bool OnSceneChange = false){
        fadeBlackRenderer.enabled = true;
        if(thisFadeTime < 0){
            thisFadeTime = defaultFadeTime;
        }
        fadeTime = thisFadeTime;
        fadeTimer=fadeTime;
        fadingIn = true;
        if(OnSceneChange){
            sceneChange = OnSceneChange;
        }
        Debug.Log("starting camera fade in" + fadeTimer);
    }

    public void StartFadeOut(float thisFadeTime = -1, bool OnSceneChange = false){
        fadeBlackRenderer.enabled = true;
        if(thisFadeTime < 0){
            thisFadeTime = defaultFadeTime;
        }
        fadeTime = thisFadeTime;
        fadeTimer=fadeTime;
        fadingOut = true;
        if(OnSceneChange){
            sceneChange = OnSceneChange;
        }
        Debug.Log("starting camera fade out " + fadeTimer);
    }



    
}
