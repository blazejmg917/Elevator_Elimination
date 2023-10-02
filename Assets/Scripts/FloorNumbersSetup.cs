using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class FloorNumbersSetup : MonoBehaviour
{
    [SerializeField, Tooltip("List of all sprites to be used as numbers. Put them in order (0 first, 9 last)")]private List<Sprite> numbers = new List<Sprite>();
    [SerializeField, Tooltip("the arrow to replace the number images")]private Sprite arrowSprite;
    [SerializeField, Tooltip("the ones place image")]private UnityEngine.UI.Image onesImage;
    [SerializeField, Tooltip("the tens place image")]private UnityEngine.UI.Image tensImage;
    [SerializeField, Tooltip("the length of the bounce when updating (should be very short)")]private float bounceDuration =.1f;
    [SerializeField, Tooltip("the amplitude of the bounce when updating (should be very small)")]private float bounceAmp =.1f;
    private Vector3 bounceMaxPos;
    private Vector3 startPos;
    private float bounceTimer = 0;
    private bool bouncing = false;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        bounceMaxPos = startPos + new Vector3(0,-1,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(bouncing){
            float t = bounceTimer / bounceDuration;
            t = 1 - t;
            transform.position = Bouncerp(startPos, bounceMaxPos, t);
            bounceTimer -= Time.fixedDeltaTime;
            //Debug.Log(t);
            if(bounceTimer < 0){
                bouncing = false;
                Debug.Log("finished bouncing");
                transform.position = startPos;
                
            }
        }
    }

    public void SetupNumbers(int floor){
        if(floor > 99 || floor < 0){
            Debug.LogError("Invalid floor number " + floor);
            return;
        }
        Debug.Log("going to floor " + floor);
        int ones = floor / 10;
        int tens = floor % 10;
        if(onesImage != null){
            onesImage.sprite = numbers[ones];
            // if(ones == 0){
            //     onesImage.sprite = arrowSprite;
            // }
        }
        else{
            Debug.Log("no image for the ones place floor number");
        }
        if(tensImage != null){
            tensImage.sprite = numbers[tens];
            
        }
        else{
            Debug.Log("no image for the ones place floor number");
        }
        bouncing = true;
        bounceTimer = bounceDuration;
    }
    public Vector3 Bouncerp(Vector3 start, Vector3 end, float t){
        if(t >= 1.0f){
            return end;
        }
        Vector3 movePath = end-start;
        //float result = (-3*Mathf.Pow((t+1),2)) + 10*(t+1)-8;
        float a = 4 * bounceAmp;
        float result = -a *(Mathf.Pow(t,2) - t);
        Vector3 resultPos = movePath * result;
        Vector3 rand = new Vector3(Random.Range(-.01f,.01f), Random.Range(-.01f,.01f), 0);
        //Debug.Log("start: " + start + "\nend: " + end + "resultPos: " + resultPos + "");
        return start + resultPos + rand;
    }
}
