using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour
{
    public PersonHolder pHolder;
    private static PersonManager _instance;
    public static PersonManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PersonManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<PersonManager>();
                    Debug.Log("Generating new person manager");
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!pHolder)
        {
            pHolder = gameObject.GetComponentInChildren<PersonHolder>();
            if (!pHolder)
            {
                Debug.LogWarning("WARNING: no Person Holder in person manager. will be unable to generate people.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PersonHolder GetPHolder()
    {
        if(!pHolder){
            GameObject pHolderObj = new GameObject("Person Holder");
            pHolderObj.transform.parent = transform;
            pHolder = pHolderObj.AddComponent<PersonHolder>();
        }
        return pHolder;
    }
}
