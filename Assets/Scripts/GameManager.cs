using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int maxFloors;
    private int currentFloor;
    private static GameManager _instance;
    private bool winCon = false;
    private bool loseCon = false;
    private bool cautious = false;
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFloors(int max, int current) {
        maxFloors = max;
        currentFloor = current;
    }

    public void ChangeFloor() {
        currentFloor--;
        if (currentFloor == 0) {
            GameOver();
        }
    }

    public void GameOver() {
        //reload scene with UI popup
        
    }

    public void TransitionLevel() {

    }

    public void StartGame() {
        //Show tutorial and controller prompts
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
    }

    public bool GetControlStyle() {
        return cautious;
    }

    public void FlipControlStyle() {
        cautious = !cautious;
    }
}
