using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]public class GameOverEvent : UnityEvent{};
    [System.Serializable]public class ResetEvent : UnityEvent{};
    
    [SerializeField, Tooltip("the elevator move object")]private ElevatorMove eMove;

    [SerializeField, Tooltip("the Level Holder")]private LevelsHolder levelHolder;
    [SerializeField, Tooltip("the camera fade component")]private CameraFade cameraFade;
    [SerializeField, Tooltip("the game over event for this project")]private GameOverEvent gameOver = new GameOverEvent();
    [SerializeField, Tooltip("event played when level is reset")]private ResetEvent reset = new ResetEvent();
    
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver(){
        gameOver.Invoke();
    }

    public void LevelStart(int id){
        
        reset.Invoke();
        eMove.SetElevatorTransform(TileManager.Instance.gameObject.transform.parent);

        id--;
        LevelStructure startLevel = levelHolder.getLevelById(id);
        TileManager.Instance.LoadLevelList(startLevel, true);
        eMove.HideElevator();
        cameraFade.StartFadeIn();
        
    }
}
