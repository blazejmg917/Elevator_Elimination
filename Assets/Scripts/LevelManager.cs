using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]public class GameOverEvent : UnityEvent<string>{};
    [System.Serializable]public class ResetEvent : UnityEvent{};
    [System.Serializable]public class LevelCompleteEvent : UnityEvent<bool>{};
    [System.Serializable]public class TutorialStartEvent : UnityEvent<DialogNode>{};
    [System.Serializable]public class TutorialEndEvent : UnityEvent{};
    
    [SerializeField, Tooltip("the elevator move object")]private ElevatorMove eMove;

    [SerializeField, Tooltip("the Level Holder")]private LevelsHolder levelHolder;
    [SerializeField, Tooltip("the camera fade component")]private CameraFade cameraFade;
    [SerializeField, Tooltip("the pause menu")]private GameObject pauseMenu;
    [SerializeField, Tooltip("the tutorial dialog holder")]private TutorialHolder tHolder;
    [SerializeField, Tooltip("if the game is paused")]private bool paused;
    [SerializeField, Tooltip("mark true to skip tutorials")]private bool skipTutorials = false;
    private bool pausePressed = false;
    private bool pauseAllowed = false;
    [SerializeField, Tooltip("the game over event for this project")]private GameOverEvent gameOver = new GameOverEvent();
    [SerializeField, Tooltip("event played when level is reset")]private ResetEvent reset = new ResetEvent();
    [SerializeField, Tooltip("event played when level is completed")]private LevelCompleteEvent levelComplete = new LevelCompleteEvent();
    [SerializeField, Tooltip("event played when tutorial dialog should start")]private TutorialStartEvent tutorialStart = new TutorialStartEvent();
    [SerializeField, Tooltip("event played when tutorial dialog completes")]private TutorialEndEvent tutorialEnd = new TutorialEndEvent();
    private bool waitingForTutorial = false;
    [SerializeField, Tooltip("current level ")]private int currentLevel = 0;
    [SerializeField, Tooltip("current custom level filename")] private string currentLevelFile = "";
    [SerializeField, Tooltip("mark true once player has completed final level")]private bool completedFinalLevel = false;
    [SerializeField, Tooltip("the number of targets left to be killed in the current level")]private int numTargets = 0;
    private bool customLevel = false;

    
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
        if(!tHolder){
            tHolder = transform.GetComponentInChildren<TutorialHolder>();
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
        MusicScript.Instance.PauseAdjust();
    }
    public void UnPause(){
        paused = false;
        pauseMenu.SetActive(false);
        MusicScript.Instance.UnpauseAdjust();
    }
    public void GameOver(string reason = ""){
        Debug.Log("LMGO " + reason);
        gameOver.Invoke(reason);
    }

    public void CustomLevelStart(string filename = "")
    {
        customLevel = true;
        if (String.IsNullOrEmpty(filename))
        {
            filename = currentLevelFile;
        }
        Debug.Log("starting custom level");
        reset.Invoke();
        eMove.SetElevatorTransform(TileManager.Instance.gameObject.transform.parent);
        currentLevelFile = filename;
        int error;
        if (!TileManager.Instance.LoadLevelFromFile(filename, out error,true))
        {
            Debug.LogWarning("level manager found error in file");
            GameManager.Instance.SetError(error);
            GameManager.Instance.QuitToMenu();
            //SceneManager.LoadScene(0);
        }
        eMove.HideElevator();
        cameraFade.StartFadeIn();
    }

    public void LevelStart(int id = -1)
    {
        customLevel = false;
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
        
        if(currentLevel < levelHolder.GetLevelsCount() && !customLevel){
            
        }
        else{
            completedFinalLevel = true;
        }
        levelComplete.Invoke(completedFinalLevel);
    }

    public void NextLevel(){
        if(completedFinalLevel){
            GameManager.Instance.QuitToMenu();
            return;
        }
        Debug.Log("Move to next level");
        currentLevel++;
        //GameManager.
        GameManager.Instance.LevelStart(currentLevel);
    }
    public bool CheckLevelForTutorial(int level = -1){
        if(skipTutorials){
            return false;
        }
        if(level == -1){
            level = currentLevel;
        }
        DialogNode tutorialNode;
        if(!tHolder.getTutorialNode(level, out tutorialNode)){
            Debug.Log("No tutorial nodes found");
            return false;
        }
        tutorialStart.Invoke(tutorialNode);
        waitingForTutorial = true;
        return true;
    }

    public void TutorialComplete(){
        if(waitingForTutorial){
            waitingForTutorial = false;
            tutorialEnd.Invoke();
            Debug.Log("tutorial complete");
        }
    }

    public void TargetKilled(){
        numTargets--;
        if(numTargets <= 0){
            GameManager.Instance.SetWinCon(true);
        }
    }

    public void SetNumTargets(int howManyTargets = 1){
        numTargets = howManyTargets;
    }
}
