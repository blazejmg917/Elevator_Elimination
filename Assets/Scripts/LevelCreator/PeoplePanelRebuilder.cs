using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeoplePanelRebuilder : MonoBehaviour
{
    private int numRequests = 0;
    private RectTransform rect;


    private void Start()
    {
        numRequests = 0;
        if (!rect)
        {
            rect = GetComponent<RectTransform>();
        }
    }

    public void RequestRebuild()
    {
        if (!rect)
        {
            rect = GetComponent<RectTransform>();
        }

        numRequests++;
        if(numRequests >= 3)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

}
