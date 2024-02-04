using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class FloorNumbersSetup : MonoBehaviour
{
    [SerializeField, Tooltip("List of all sprites to be used as numbers. Put them in order (0 first, 9 last)")]private List<Sprite> numbers = new List<Sprite>();
    [SerializeField, Tooltip("the arrow to replace the number images")]private Sprite arrowSprite;
    [SerializeField, Tooltip("the ones place image")]private UnityEngine.UI.Image onesImage;
    [SerializeField, Tooltip("the tens place image")]private UnityEngine.UI.Image tensImage;
    [SerializeField, Tooltip("the default number color")]private Color defaultColor;
    [SerializeField, Tooltip("the color to display when low on turns")]private Color stressColor;
    [SerializeField, Tooltip("the percentage of max turns left to turn to stressColor")]private float stressPercent = .25f;
    [SerializeField, Tooltip("the sprite for ones place when on floor 9 or lower")]private UnityEngine.UI.Image bigOnesImage;
    [SerializeField, Tooltip("the sprite for the arrow on floor 9 or lower")]private UnityEngine.UI.Image arrowImage;
    [SerializeField, Tooltip("the length of the bounce when updating (should be very short)")]private float bounceDuration =.1f;
    [SerializeField, Tooltip("the amplitude of the bounce when updating (should be very small)")]private float bounceAmp =.1f;
    [SerializeField, Tooltip("the max shake amplitude per axis in stress mode")]private float shakeAmp = .02f;
    private Vector3 bounceMaxPos;
    private Vector3 startPos;
    private float bounceTimer = 0;
    private bool bouncing = false;
    private bool stressed = false;
    private Vector3 oneStartPos;
    private Vector3 tenStartPos;
    private Vector3 bigOneStartPos;
    private Vector3 arrowStartPos;
    private bool SetupStartPositions = false;
    // Start is called before the first frame update
    void Start()
    {
        if(SetupStartPositions){
            return;
        }
        startPos = transform.position;
        bounceMaxPos = startPos + new Vector3(0,-1,0);
        oneStartPos = onesImage.transform.localPosition;
        bigOneStartPos = bigOnesImage.transform.localPosition;
        tenStartPos = tensImage.transform.localPosition;
        arrowStartPos = arrowImage.transform.localPosition;
        SetupStartPositions = true;
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
        else{
            transform.position = startPos;
        }
        if(stressed){
            onesImage.transform.localPosition = oneStartPos + GetShakeVals();
            tensImage.transform.localPosition = tenStartPos + GetShakeVals();
            bigOnesImage.transform.localPosition = bigOneStartPos + GetShakeVals();
            arrowImage.transform.localPosition = arrowStartPos + GetShakeVals();
        }
        else{
            onesImage.transform.localPosition = oneStartPos;
            tensImage.transform.localPosition = tenStartPos;
            bigOnesImage.transform.localPosition = bigOneStartPos;
            arrowImage.transform.localPosition = arrowStartPos;
        }
    }

    public void SetupNumbers(int floor){
        if (floor == -1)
        {
            floor = GameManager.Instance.GetCurrentFloor();
        }
        int maxFloors = GameManager.Instance.GetMaxFloors();
        if(floor > 99 || floor < 0){
            Debug.LogError("Invalid floor number " + floor);
            return;
        }
        if(floor < 10){
            onesImage.enabled = false;
            tensImage.enabled = false;
            bigOnesImage.enabled = true;
            arrowImage.enabled = true;
            bigOnesImage.sprite = numbers[floor];
        }
        else{
            onesImage.enabled = true;
            tensImage.enabled = true;
            bigOnesImage.enabled = false;
            arrowImage.enabled = false;
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
            
        }
        if(!stressed && CheckStressed(floor,maxFloors)){
            onesImage.color = stressColor;
            tensImage.color = stressColor;
            bigOnesImage.color = stressColor;
            arrowImage.color = stressColor;
            stressed = true;
        }
        else if(stressed && !CheckStressed(floor,maxFloors)){
            onesImage.color = defaultColor;
            tensImage.color = defaultColor;
            bigOnesImage.color = defaultColor;
            arrowImage.color = defaultColor;
            stressed = false;
        }
        //Debug.Log("going to floor " + floor);
        bouncing = true;
            bounceTimer = bounceDuration;
        
    }

    public bool CheckStressed(int current, int max){
        return ((float)current / (float)max <= stressPercent);
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

    public Vector3 GetShakeVals(){
        return new Vector3(Random.Range(-shakeAmp,shakeAmp),Random.Range(-shakeAmp,shakeAmp),0);
    }
}
