using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorMove : MonoBehaviour
{
    [System.Serializable]
    public class ElevatorEnterEvent : UnityEvent{};
    [System.Serializable]
    public class ElevatorExitEvent : UnityEvent{};
    [SerializeField, Tooltip("the elevator object to move. \nShould contain all tiles and people")]private Transform elevator;
    [SerializeField, Tooltip("Main Game Pos. Will move to this spot on startup and leave it on level complete")]private Vector3 mainGamePos = Vector3.zero;
    private Vector3 bounceMaxPos = Vector3.zero;
    [SerializeField, Tooltip("Off Screen Position. Will move to this spot on startup and leave it on level complete")]private Vector3 OffScreenPos = Vector3.zero;
    [SerializeField, Tooltip("the time it takes for the elevator to enter and leave")]private float transitionTime = 2;
    [SerializeField, Tooltip("the time it takes for the elevator to bounce")]private float bounceTime = .5f;
    [SerializeField, Tooltip("the amplitude of the bounce")]private float bounceAmplitude = .5f;
    private float transitionTimer;
    private bool enteringScreen = false;
    private bool exitingScreen = false;
    private bool bouncing = false;
    [SerializeField, Tooltip("immediately enter the screen on startup for demos")]private bool demoEnterScreen = false;
    [SerializeField, Tooltip("the elevator exit event")]private ElevatorExitEvent exitEvent = new ElevatorExitEvent();
    [SerializeField, Tooltip("the elevator enter event")]private ElevatorEnterEvent enterEvent = new ElevatorEnterEvent();
    
    // Start is called before the first frame update

    void Start()
    {
        if(demoEnterScreen){
            EnterScreen(false);
        }
        bounceMaxPos = mainGamePos + new Vector3(0,1,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(bouncing){
            float t = transitionTimer / bounceTime;
            t = 1 - t;
            elevator.position = Bouncerp(mainGamePos, bounceMaxPos, t);
            transitionTimer -= Time.fixedDeltaTime;
            //Debug.Log(t);
            if(transitionTimer < 0){
                bouncing = false;
                Debug.Log("finished bouncing");
                elevator.position = mainGamePos;
                if(enteringScreen){
                    enteringScreen = false;
                    
                }
                else if(exitingScreen){
                    transitionTimer = transitionTime;
                }
            }
        }
        else if(enteringScreen){
            float t = transitionTimer / transitionTime;
            t = 1 - t;
            elevator.position = Vector3.Lerp(OffScreenPos, mainGamePos, t);
            transitionTimer -= Time.fixedDeltaTime;
            //Debug.Log(t);
            if(transitionTimer < 0){
                enteringScreen = false;
                Debug.Log("finished entering");
                //elevator.position = mainGamePos;
                bouncing = true;
                transitionTimer = bounceTime;
                enterEvent.Invoke();
            }
        }
        else if(exitingScreen){
            float t = transitionTimer / transitionTime;
            t = 1-t;
            elevator.position = Vector3.Lerp(OffScreenPos, mainGamePos, t);
            transitionTimer -= Time.fixedDeltaTime;
            if(transitionTimer < 0){
                exitingScreen = false;
                Debug.Log("finished exiting");
                elevator.position = OffScreenPos;
                exitEvent.Invoke();
            }
        }
    }

    public void HideElevator(){
        elevator.position = OffScreenPos;
    }

    public void EnterScreen(bool SceneChange = false){
        if(SceneChange){
            return;
        }
        HideElevator();
        enteringScreen = true;
        transitionTimer = transitionTime;
        bouncing = false;
        Debug.Log("elevator entering");
    }
    public void ExitScreen(bool SceneChange = false){
        exitingScreen = true;
        transitionTimer = transitionTime;
        bouncing = true;
        Debug.Log("elevator exiting");
        HideElevator();
    }

    public Vector3 Bouncerp(Vector3 start, Vector3 end, float t){
        if(t >= 1.0f){
            return end;
        }
        Vector3 movePath = end-start;
        //float result = (-3*Mathf.Pow((t+1),2)) + 10*(t+1)-8;
        float a = 4 * bounceAmplitude;
        float result = -a *(Mathf.Pow(t,2) - t);
        Vector3 resultPos = movePath * result;
        Vector3 rand = new Vector3(Random.Range(-.01f,.01f), Random.Range(-.01f,.01f), 0);
        //Debug.Log("start: " + start + "\nend: " + end + "resultPos: " + resultPos + "");
        return start + resultPos + rand;
    }

}
