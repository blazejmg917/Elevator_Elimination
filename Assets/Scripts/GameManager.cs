using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    
    
    private int maxFloors;
    [SerializeField]private int currentFloor;
    private static GameManager _instance;
    private bool winCon = false;
    private bool loseCon = false;
    [SerializeField][Tooltip("Control style: true is cautious, false is quick")] private bool cautious = true;
    //[SerializeField, Tooltip("the Level Holder")]private LevelsHolder levelHolder;
    [SerializeField, Tooltip("the current level")]private int currentLevel = -1;
    [SerializeField, Tooltip("if marked true, will try to set up demo level that is the number specified above")]private bool tryDemoLevel = false;
    //[SerializeField, Tooltip("the camera fade component")]private CameraFade cameraFade;
    //[SerializeField, Tooltip("the camera for the object")]private Camera gameCam;
    [SerializeField, Tooltip("the name of the main level scene")]private string levelSceneName = "GameLoopSetupScene";
    // [SerializeField, Tooltip("event played every time a turn changes")]private TurnChangeEvent turnChangeEvent = new TurnChangeEvent();
    // [SerializeField, Tooltip("event played on game loss")]private GameLoseEvent gameOver = new GameLoseEvent();
    //[SerializeField, Tooltip("the elevator move object")]private ElevatorMove eMove;

    private enum GameState
    {
        MainMenu,
        GameStart,
        Paused,
        GameEnd,
        GameOver
    }
    [SerializeField] GameState state = GameState.MainMenu;
    private String[] menuOptions = {"Play", "Level Select", "Quit", "Control Mode"};
    private int menuIndex = 0;
    [SerializeField] private String highlightedMenu = "Play";
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<GameManager>();
                    Debug.Log("Generating new game manager");
                }
            }
            
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded(scene, mode);
    }

    public int GetLevelId(){
        return currentLevel;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (Instance != this) return;
        if (scene.name == "MainMenu") {
            state = GameState.MainMenu;
        } else if (scene.name == "HaleyTest") {
            state = GameState.GameStart;
        } else if (scene.name == "GameOver") {
            state = GameState.GameOver;
        } else if (scene.name == levelSceneName){
            state = GameState.GameStart;
            LevelStart(currentLevel);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Start(){
        
        if(tryDemoLevel && SceneManager.GetActiveScene().name != "MainMenu"){
            
            LevelStart(currentLevel);
        }
        if(Instance != this){
            Destroy(gameObject);
        }
        
    }

    // public Camera GetCamera(){
    //     return gameCam;
    // }

    public void SetFloors(int max, int current) {
        maxFloors = max;
        currentFloor = current;
        //turnChangeEvent.Invoke(currentFloor);
    }

    public int ChangeFloor() {
        currentFloor-= 1;
        //turnChangeEvent.Invoke(currentFloor);
        if (currentFloor <= 0) {
            GameOver("TURNS");
        }
        return currentFloor;
    }

    /*
     * Undoes the floor change
     * @param change The number of floors to add to the current counter
     */
    public void UndoFloor(int change) {
        currentFloor += change;
    }
    /*
     * Gets the current floor number
     * @return Returns the current floor number
     */
    public int GetCurrentFloor() {
        return currentFloor;
    }

    public void GameOver(string reason = "") {
        Debug.Log("GMGO " + reason);
        //reload scene with UI popup
        //SceneManager.LoadScene("GameOver");
        //gameOver.Invoke();
        if(!loseCon){
            SetLoseCon(true);
        }
        LevelManager.Instance.GameOver(reason);
    }

    public void TransitionLevel() {

    }

    public void StartGame() {
        //Show tutorial and controller prompts
        SceneManager.LoadScene(levelSceneName);
    }

    public bool GetWinCon() {
        return winCon;
    }

    public void SetWinCon(bool con) {
        winCon = con;
    }

    public bool GetLoseCon() {
        return loseCon;
    }

    public void SetLoseCon(bool con) {
        loseCon = con;
        // if(loseCon){
        //     GameOver("SET MANUALLY");
        // }
    }

    public bool GetControlStyle() {
        return cautious;
    }

    public void FlipControlStyle() {
        cautious = !cautious;
        //change toggle
    }

    public void UpdateMenuHighlight() {
        highlightedMenu = menuOptions[menuIndex];
        //change highlight
    }

    

    public void LevelStart(int id){
        
        LevelManager.Instance.LevelStart(id);
        currentLevel = id;
        //turnChangeEvent.Invoke(currentFloor);
        //currentLevel = id;
        // eMove.SetElevatorTransform(TileManager.Instance.gameObject.transform.parent);

        // id--;
        // LevelStructure startLevel = levelHolder.getLevelById(id);
        // TileManager.Instance.LoadLevelList(startLevel, true);
        // eMove.HideElevator();
        // cameraFade.StartFadeIn();
        SetLoseCon(false);
        SetWinCon(false);
    }

    // public void Menuing(InputAction.CallbackContext ctx) {
    //     if (state == GameState.MainMenu) {
    //         float y = ctx.ReadValue<Vector2>().y;
    //         if (y < -0.1f) {
    //             menuIndex++;
    //             if (menuIndex > 3) {
    //                 menuIndex = 0;
    //             }
    //             UpdateMenuHighlight();
    //         } else if (y > 0.1f) {
    //             menuIndex--;
    //             if (menuIndex < 0) {
    //                 menuIndex = 3;
    //             }
    //             UpdateMenuHighlight();
    //         }
    //     }
    // }

    // public void MenuSelect(InputAction.CallbackContext ctx) {
    //     if (state == GameState.MainMenu) {
    //         float pressed = ctx.ReadValue<float>();
    //         if (pressed > 0.5f) {
    //             switch(highlightedMenu) {
    //                 case "Play":
    //                     StartGame();
    //                     break;
    //                 case "Level Select":
    //                     LevelSelect();
    //                     break;
    //                 case "Quit":
    //                     Application.Quit();
    //                     break;
    //                 case "Control Mode":
    //                     FlipControlStyle();
    //                     break;
    //             }
    //         }
    //     }
    // }

    public void LevelSelect() {
        //Show Level Select Menu
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnFadeOutEnd(bool sceneChange){
        Debug.Log("fade out ended");
    }

    public void RetryLevel(){
        LevelStart(currentLevel);
    }

    public void QuitToMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void SetCurrentLevel(int levelID) {
        currentLevel = levelID;
    }

    public int GetMaxFloors(){
        return maxFloors;
    }


}
