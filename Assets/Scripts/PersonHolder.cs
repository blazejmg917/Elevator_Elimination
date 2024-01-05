using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonHolder : MonoBehaviour
{
    [SerializeField, Tooltip("a list of the all the people prefabs that have been created for the game")]List<GameObject> people = new List<GameObject>();
    Dictionary<string, GameObject> peopleMap = new Dictionary<string, GameObject>();
    Dictionary<string, string> peopleKeyMap = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMap(){
        Debug.Log("updating person map: " + people + ", " +people.Count);
        peopleMap = new Dictionary<string, GameObject>();
        peopleKeyMap = new Dictionary<string, string>();
        foreach(GameObject person in people)
        {
            Person personScript = person.GetComponent<Person>();
            peopleMap.Add(personScript.GetId(),person);
            peopleKeyMap.Add(personScript.GetKey().ToUpper(),personScript.GetId());
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

    public string GetIdByKey(string key)
    {
        string result;
        if (peopleKeyMap.TryGetValue(key, out result))
        {
            return result;
        }
        Debug.Log("Could not find id for person with key of " + key);
        return null;
    }

    public List<GameObject> GetPersonList()
    {
        return people;
    }

    // void OnValidate(){
    //     foreach()
    // }
}
