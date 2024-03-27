using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverUIScript : MonoBehaviour
{
    [SerializeField, Tooltip("the game over animation")]private Animator gameOverAnimation;
    [SerializeField, Tooltip("the game over delay before buttons become active")] private float gameOverDelay = 1;
    private float gameOverTimer;
    private bool waitingToDisplay = false;
    [SerializeField, Tooltip("All of the game over buttons and visuals that should show up after game over is displayed")]private List<GameObject> gameOverObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if(!gameOverAnimation){
            gameOverAnimation = gameObject.GetComponent<Animator>();
        }
        HideGameOverObjects();
        
    }
    void FixedUpdate(){
        if(waitingToDisplay){
            gameOverTimer -= Time.fixedDeltaTime;
            if(gameOverTimer < 0){
                waitingToDisplay = false;
                DisplayGameOverObjects();
            }
        }
    }

    // Update is called once per frame
    public void StartGameOver(){
        gameOverAnimation.SetTrigger("GameOver");
        waitingToDisplay = true;
        gameOverTimer = gameOverDelay;
        Debug.Log("game over");
        //Invoke("DisplayGameOverObject", gameOverDelay);
    }

    public void DisplayGameOverObjects(){
        foreach(GameObject go in gameOverObjects){
            go.SetActive(true);
        }
    }
    public void HideGameOverObjects(){
        foreach(GameObject go in gameOverObjects){
            go.SetActive(false);
        }
    }

    public void CloseGameOver(){
        HideGameOverObjects();
        gameOverAnimation.SetTrigger("Reset");
    }

    public void QuitToMenu(){
        SFXManager.Instance.MischiefManaged(false);
        GameManager.Instance.QuitToMenu();
    }

    public void Retry(){
        if(GameManager.Instance.GetWinCon()){
            SFXManager.Instance.MischiefManaged();
        }
        CloseGameOver();
        GameManager.Instance.RetryLevel();
    }
}
