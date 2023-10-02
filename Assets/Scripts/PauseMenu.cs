using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField,Tooltip("the pause menu")]private GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        if(!pauseMenu){
            pauseMenu = transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Retry(){
        pauseMenu.SetActive(false);
        LevelManager.Instance.UnPause();
        GameManager.Instance.SetLoseCon(true);
    }

    public void Resume(){
        pauseMenu.SetActive(false);
        LevelManager.Instance.UnPause();
    }
    public void QuitToMenu(){
        GameManager.Instance.QuitToMenu();
    }
}
