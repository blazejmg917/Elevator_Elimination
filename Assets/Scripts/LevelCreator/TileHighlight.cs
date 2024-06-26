using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    [SerializeField, Tooltip("default color")]
    private Color defaultColor = Color.white;

    [SerializeField, Tooltip("the highlighted color")]
    private Color highlightColor = Color.cyan;

    [SerializeField, Tooltip("the error color")]
    private Color errorColor = Color.red;

    [SerializeField, Tooltip("the renderer for this object")]
    private SpriteRenderer renderer;

    [SerializeField, Tooltip("the Tile script")]
    private Tile tileScript;

    private int numberOfPersonsLookingAtTile;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        tileScript = GetComponent<Tile>();
        numberOfPersonsLookingAtTile = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDefaultColor()
    {
        //Debug.Log("set default color");
        renderer.color = defaultColor;
        if (tileScript.GetPerson())
        {
            tileScript.GetPerson().SetColor(renderer.color);
        }
    }

    public void SetHoverColor(bool dragging)
    {
        //Debug.Log("Set hover color");
        if (dragging && !tileScript.CanPlaceHere())
        {
            renderer.color = errorColor;
        }
        else
        {
            renderer.color = highlightColor;
        }

        if (tileScript.GetPerson())
        {
            tileScript.GetPerson().SetColor(renderer.color);
        }
    }

    public void addLOSHighlight(){
        numberOfPersonsLookingAtTile++;
        renderer.color = highlightColor;
        if (tileScript.GetPerson())
        {
            tileScript.GetPerson().SetColor(renderer.color);
        }
    }

    public void removeLOSHighlight(){
        if (numberOfPersonsLookingAtTile > 0){
            numberOfPersonsLookingAtTile--;
        }
        
        if (numberOfPersonsLookingAtTile <= 0 && tileScript.GetPerson()){
            renderer.color = defaultColor;
            tileScript.GetPerson().SetColor(renderer.color);
        }
    }
}
