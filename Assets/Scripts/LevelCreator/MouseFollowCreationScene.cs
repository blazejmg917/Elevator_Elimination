using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowCreationScene : MonoBehaviour
{
    [SerializeField, Tooltip("the object that is currently being dragged by the mouse")]
    private Draggable currentObject;

    private bool currentlyDragging;

    [SerializeField, Tooltip("the Tile that was most recently hovered over")]
    private Collider2D prevHoverTile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("raycasting " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0,10f), Color.green, 10f);
        RaycastHit2D hit = Physics2D.Raycast( Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, LayerMask.NameToLayer("Tile"));
        if (hit.collider)
        {
            Debug.Log("hit something");
            if (hit.collider != prevHoverTile)
            {
                if (prevHoverTile)
                {
                    prevHoverTile.GetComponent<TileHighlight>().SetDefaultColor();
                }
                prevHoverTile = hit.collider;
                prevHoverTile.GetComponent<TileHighlight>().SetHoverColor(currentlyDragging);
            }
        }
        else
        {
            if (prevHoverTile)
            {
                //turn off highlight
                prevHoverTile.GetComponent<TileHighlight>().SetDefaultColor();
                prevHoverTile = null;

            }
        }
        if (!currentlyDragging)
        {
            if (prevHoverTile)
            {
                //turn off highlight
                prevHoverTile.GetComponent<TileHighlight>().SetDefaultColor();
                prevHoverTile = null;
            }
        }
    }

    public void OnClick()
    {
        if (currentlyDragging || currentObject)
        {
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero, 0, LayerMask.NameToLayer("Draggable"));
        if (hit.collider)
        {
            DraggableSpawner thisObj = hit.collider.gameObject.GetComponent<DraggableSpawner>();
            if (thisObj)
            {
                GameObject drag = thisObj.GetPrefab();
                if (!drag)
                {
                    return;
                }
                currentObject = drag.GetComponent<Draggable>();
                if (currentObject && currentObject.GetComponent<Person>())
                {
                    currentObject.transform.parent = TileManager.Instance.GetPersonHolderTransform();
                    currentlyDragging = true;
                }

            }
        }
    }



    public void OnRelease()
    {
        if (prevHoverTile)
        {
            Tile thisTile = prevHoverTile.GetComponent<Tile>();
            Person thisPerson = currentObject.GetComponent<Person>();
            if (thisTile && !thisTile.GetPerson())
            {
                thisTile.SetPerson(thisPerson, true);
                thisPerson.SetCurrentTile(thisTile);
                currentObject = null;
                currentlyDragging = false;
            }
            else
            {
                Destroy(currentObject);
            }
            prevHoverTile.GetComponent<TileHighlight>().SetHoverColor(false);
        }
        else
        {
            Destroy(currentObject);
        }
        currentObject = null;
        currentlyDragging = false;
    }
}
