using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorScript : MonoBehaviour
{
    [SerializeField] private int id;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click() {
        GameManager.Instance.SetCurrentLevel(id);
        GameManager.Instance.StartGame();
    }
}
