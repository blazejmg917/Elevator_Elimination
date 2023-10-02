using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtonScript : MonoBehaviour
{
    [SerializeField] private Sprite pressed;
    private bool timerStart = false;
    private int timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timerStart) {
            timer++;
            if (timer == 50) {
                timerStart = false;
                timer = 0;
                //Move To Level Select
            }
        }
    }

    public void Click() {
        GetComponent<Button>().image.sprite = pressed;
        timerStart = true;
    }
}
