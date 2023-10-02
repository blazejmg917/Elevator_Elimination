using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtonScript : MonoBehaviour
{
    [SerializeField] private Sprite pressed;
    private bool timerStart = false;
    private int timer = 0;
    [SerializeField] private Canvas levelSel;
    // Start is called before the first frame update
    void Start()
    {
        levelSel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timerStart) {
            timer++;
            if (timer == 5) {
                timerStart = false;
                timer = 0;
                transform.parent.gameObject.SetActive(false);
                levelSel.gameObject.SetActive(true);
            }
        }
    }

    public void Click() {
        GetComponent<Button>().image.sprite = pressed;
        timerStart = true;
    }
}
