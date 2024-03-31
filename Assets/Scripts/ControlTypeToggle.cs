using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlTypeToggle : MonoBehaviour
{
    bool cautious = true;
    UnityEngine.UI.Image image;
    // Start is called before the first frame update
    void Start()
    {
        if(!image){
            image = GetComponent<Image>();
        }
        cautious = GameManager.Instance.GetControlStyle();
        UpdateVisual();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick(){
        cautious = !cautious;
        GameManager.Instance.FlipControlStyle();
        UpdateVisual();
    }
    

    public void UpdateVisual()
    {
        if (!cautious)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
