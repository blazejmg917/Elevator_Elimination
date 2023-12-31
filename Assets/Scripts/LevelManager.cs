using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]public class GameOverEvent : UnityEvent{};
    [System.Serializable]public class ResetEvent : UnityEvent{};
    [System.Serializable]public class LevelCompleteEvent : UnityEvent<bool>{};
    
    [SerializeField, Tooltip("the elevator move object")]private ElevatorMove eMove;

    [SerializeField, Tooltip("the Level Holder")]private LevelsHolder levelHolder;
    [SerializeField, Tooltip("the camera fade component")]private CameraFade cameraFade;
    [SerializeField, Tooltip("the pause menu")]private GameObject pauseMenu;
    [SerializeField, Tooltip("if the game is paused")]private bool paused;
    private bool pausePressed = false;
    private bool pauseAllowed = false;
    [SerializeField, Tooltip("the game over event for this project")]private GameOverEvent gameOver = new GameOverEvent();
    [SerializeField, Tooltip("event played when level is reset")]private ResetEvent reset = new ResetEvent();
    [SerializeField, Tooltip("event played when level is completed")]private LevelCompleteEvent levelComplete = new LevelCompleteEvent();
    [SerializeField, Tooltip("current level ")]private int currentLevel = 0;
    [SerializeField, Tooltip("mark true once player has completed final level")]private bool completedFinalLevel = false;
    
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<LevelManager>();
                    Debug.Log("Generating new game manager");
                }
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(!levelHolder){
            
            levelHolder = GetComponentInChildren<LevelsHolder>();
            if(!levelHolder){
                Debug.LogError("NO LEVEL HOLDER IS FOUND. WILL BE EMPTY");
            }
        }
        if(!cameraFade){
            cameraFade = transform.GetComponentInChildren<CameraFade>();
        }
        if(!eMove){
            eMove = GetComponent<ElevatorMove>();
        }
        if(pauseMenu){
            pauseMenu.SetActive(false);
        }
        paused = false;
        UnPause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePause(bool enable){
        pauseAllowed = enable;
    }

    public bool IsPaused(){
        return paused;
    }
    public void PressedPause(InputAction.CallbackContext ctx){
        float val = ctx.ReadValue<float>();
        if(val > .5 && !pausePressed){
            pausePressed = true;
            TogglePause();
        }
        else if(val <= .5){
            pausePressed = false;
        }
        
    }
    public void TogglePause(){
        if(paused){
                UnPause();
            }
            else if(pauseAllowed){
                Pause();
            }
    }
    public void Pause(){
        paused = true;
        pauseMenu.SetActive(true);
    }
    public void UnPause(){
        paused = false;
        pauseMenu.SetActive(false);
    }
    public void GameOver(){
        gameOver.Invoke();
    }

    public void LevelStart(int id = -1){
        if(id == -1){
            id = currentLevel;
        }
        Debug.Log("starting level");
        reset.Invoke();
        eMove.SetElevatorTransform(TileManager.Instance.gameObject.transform.parent);
        currentLevel = id;
        id--;
        LevelStructure startLevel = levelHolder.getLevelById(id);
        TileManager.Instance.LoadLevelList(startLevel, true);
        eMove.HideElevator();
        cameraFade.StartFadeIn();
        
    }

    public void CompleteLevel(){
        
        if(currentLevel < levelHolder.GetLevelsCount()){
            
        }
        else{
            completedFinalLevel = true;
        }
        levelComplete.Invoke(completedFinalLevel);
    }

    public void NextLevel(){
        if(completedFinalLevel){
            GameManager.Instance.QuitToMenu();
        }
        Debug.Log("Move to next level");
        currentLevel++;
        //GameManager.
        GameManager.Instance.LevelStart(currentLevel);
    }
}
