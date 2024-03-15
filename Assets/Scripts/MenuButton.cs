using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Sprite pressed;
    [SerializeField] private String typeOfButton;
    [SerializeField] private Canvas levelSel;
    // Start is called before the first frame update
    void Start()
    {
        if (levelSel) {
            levelSel.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Click() {
        GetComponent<Button>().image.sprite = pressed;
        AudioScript.Instance.ButtonClick();
        switch (typeOfButton) {
            case "Play":
                GameManager.Instance.SetCurrentLevel(1);
                GameManager.Instance.StartGame();
                break;
            case "Level Select":
                transform.parent.gameObject.SetActive(false);
                levelSel.gameObject.SetActive(true);
                break;
            case "Quit":
                Application.Quit();
                break;
        }
    }

    public void Hover()
    {
        AudioScript.Instance.ButtonHover();
    }
}
