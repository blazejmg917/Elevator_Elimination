using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonHolder : MonoBehaviour
{
    List<GameObject> people = new List<GameObject>();
    Dictionary<string, GameObject> peopleMap = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMap(){
        peopleMap = new Dictionary<string, GameObject>();
        foreach(GameObject person in people){
            peopleMap.Add(person.GetComponent<Person>().GetId(),person);
        }
    }

    public GameObject GetPersonById(string id){
        GameObject result;
        if(peopleMap.TryGetValue(id, out result)){
            return result;
        }
        Debug.Log("Could not find person with id of " + id);
        return null;

    }

    // void OnValidate(){
    //     foreach()
    // }
}
