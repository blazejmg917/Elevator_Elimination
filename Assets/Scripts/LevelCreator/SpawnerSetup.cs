using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSetup : MonoBehaviour
{
    [SerializeField, Tooltip("the spawner prefab")]
    private GameObject spawnerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> people = TileManager.Instance.GetPeople();
        if (spawnerPrefab)
        {
            foreach (GameObject p in people)
            {
                GameObject newSpawner = Instantiate(spawnerPrefab, transform);
                newSpawner.GetComponent<DraggableSpawner>().SetPrefab(p);
                newSpawner.GetComponent<DraggableSpawner>().SetImage(p.GetComponent<SpriteRenderer>().sprite);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
