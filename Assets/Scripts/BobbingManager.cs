using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingManager : MonoBehaviour
{

    //public Sprite Unbobbed[4];
    //public Sprite bobbed[4];
    //public SpriteRenderer sp;

    private float timePassed = 0f;

    private readonly float beatTime = (60f / 130f);

    private bool isBobbed = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= beatTime)
        {
            isBobbed = !isBobbed;

            timePassed %= beatTime;
            print(isBobbed + " " + timePassed);
        }
        
        //do some math
        //sp.sprite = //correct sprite
    }
}
