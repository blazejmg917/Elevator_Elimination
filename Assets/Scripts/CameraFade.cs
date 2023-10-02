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
    // Start is called before the first frame update
    void Start()
    {
        if(!fadeBlackRenderer){
            fadeBlackRenderer = GetComponent<Renderer>();
        }
    }

    void FixedUpdate(){

        if(fadingIn){
            float t = fadeTimer / fadeTime;
            t = 1-t;
            fadeBlackRenderer.material.color = new Color(blackColor.r, blackColor.g, blackColor.b, Mathf.Lerp(0,1,t));
        }
        else if(fadingOut){
            float t = fadeTimer / fadeTime;
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
        if(fadeTime < 0){
            thisFadeTime = defaultFadeTime;
        }
        fadeTime = thisFadeTime;
        fadingIn = true;
        if(OnSceneChange){
            sceneChange = OnSceneChange;
        }
    }

    public void StartFadeOut(float thisFadeTime = -1, bool OnSceneChange = false){
        if(fadeTime < 0){
            thisFadeTime = defaultFadeTime;
        }
        fadeTime = thisFadeTime;
        fadingOut = true;
        if(OnSceneChange){
            sceneChange = OnSceneChange;
        }
    }

    
}
