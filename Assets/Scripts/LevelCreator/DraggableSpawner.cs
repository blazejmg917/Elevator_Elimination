using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("the image for this spawner")]
    private UnityEngine.UI.Image image;

    [SerializeField, Tooltip("the prefab to be spawned from this object")]
    private GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public GameObject GetPrefab()
    {
        return Instantiate(prefab);
    }

    public void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
    }
}
