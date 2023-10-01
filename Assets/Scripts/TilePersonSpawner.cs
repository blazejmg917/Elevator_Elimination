using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePersonSpawner : MonoBehaviour
{
    private Tile tileScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SpawnObjectOnTile(){
        if(!tileScript){
            tileScript = gameObject.GetComponent<Tile>();
        }
        Person person = tileScript.GetPerson();
        if(person){
            PersonManager.Instance.pHolder.UpdateMap();
            GameObject goalPerson = PersonManager.Instance.pHolder.GetPersonById(tileScript.GetPersonId());
            if(goalPerson){
                GameObject personObj = Instantiate(goalPerson);
                tileScript.SetPerson(personObj.GetComponent<Person>());
                personObj.transform.position = tileScript.GetPersonLocation();
                personObj.GetComponent<Person>().SetCurrentTile(tileScript);
                personObj.GetComponent<Person>().SetDirection(tileScript.GetDirection());
            }
            
        }
    }
}
