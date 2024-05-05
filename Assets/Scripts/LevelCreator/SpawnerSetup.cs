using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerSetup : MonoBehaviour
{
    [SerializeField, Tooltip("the spawner prefab")]
    private GameObject spawnerPrefab;
    [SerializeField, Tooltip("the people/targets/objects to spawn in this section")]
    private List<GameObject> spawnerList = new List<GameObject>();
    [SerializeField, Tooltip("the rectTransform to rebuild")]
    private RectTransform rect;
    [SerializeField, Tooltip("the overarching rebuilder")]
    private PeoplePanelRebuilder rebuilder;
    // Start is called before the first frame update
    void Start()
    {
        if (!rect)
        {
            rect = GetComponent<RectTransform>();
        }
        if (spawnerPrefab)
        {
            foreach (GameObject p in spawnerList)
            {
                GameObject newSpawner = Instantiate(spawnerPrefab, transform);
                newSpawner.GetComponent<DraggableSpawner>().SetPrefab(p);
                newSpawner.GetComponent<DraggableSpawner>().SetImage(p.GetComponent<SpriteRenderer>().sprite);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        if (rebuilder)
        {
            rebuilder.RequestRebuild();
        }
    }
}
