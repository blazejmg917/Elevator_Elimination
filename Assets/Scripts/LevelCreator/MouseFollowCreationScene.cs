using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollowCreationScene : MonoBehaviour
{
    [SerializeField, Tooltip("the object that is currently being dragged by the mouse")]
    private Draggable currentObject;

    private bool currentlyDragging;

    [SerializeField, Tooltip("the Tile that was most recently hovered over")]
    private Collider2D prevHoverTile;

    private bool justClicked = false;

    private static MouseFollowCreationScene _instance;

    public static MouseFollowCreationScene Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MouseFollowCreationScene>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<MouseFollowCreationScene>();
                    Debug.Log("Generating new MouseFollowCreationScene");
                }
            }

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0,10f), Color.green, 10f);
        //RaycastHit2D hit = Physics2D.Raycast( Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, LayerMask.NameToLayer("Tile"));
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition),
            ~LayerMask.NameToLayer("Tile"));
        //Debug.Log("raycasting " + Camera.main.ScreenToWorldPoint(Input.mousePosition) + ", " + hit);
        if (hit)
        {
            //Debug.Log("hit something");
            if (hit != prevHoverTile)
            {
                if (prevHoverTile)
                {
                    prevHoverTile.GetComponent<TileHighlight>().SetDefaultColor();
                }

                prevHoverTile = hit;
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
        //if (!currentlyDragging)
        //{
        //    if (prevHoverTile)
        //    {
        //        //turn off highlight
        //        prevHoverTile.GetComponent<TileHighlight>().SetHoverColor();
        //        prevHoverTile = null;
        //    }
        //}
    }

    public void Grab(GameObject newObj)
    {
        if (!newObj || currentlyDragging)
        {
            return;
        }

        currentObject = newObj.GetComponent<Draggable>();
        if (currentObject && currentObject.GetComponent<Person>())
        {
            Debug.Log("now dragging object");
            currentObject.transform.parent = TileManager.Instance.GetPersonHolderTransform();
            currentlyDragging = true;
            currentObject.SetDragging(true);
        }
        else
        {
            Debug.Log("Invalid drag object");
            Destroy(currentObject.gameObject);
        }
    }

    public void OnClick()
    {
        Debug.Log("clicked");
        if (currentlyDragging || currentObject)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero, 0, ~LayerMask.NameToLayer("Draggable"));
        if (hit.collider)
        {
            Debug.Log("clicked spawner");
            DraggableSpawner thisObj = hit.collider.gameObject.GetComponent<DraggableSpawner>();
            if (thisObj)
            {
                GameObject drag = thisObj.GetPrefab();
                Grab(drag);

            }

            return;
        }

        Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition),
            ~LayerMask.NameToLayer("Tile"));
        if (col)
        {
            Debug.Log("clicked Tile");
            Tile thisTile = col.gameObject.GetComponent<Tile>();
            if (thisTile && thisTile.GetPerson())
            {
                currentObject = thisTile.GetPerson().GetComponent<Draggable>();
                //thisTile.GetPerson().SetCurrentTile(null);
                currentObject.SetDragging(true);
                thisTile.SetPerson(null, true);
                Grab(currentObject.gameObject);
            }

            return;
        }
    }

    public void OnClickChange(InputAction.CallbackContext ctx)
    {
        Debug.Log("Click Changed");
        if (ctx.ReadValue<float>() < .5)
        {
            justClicked = false;
            OnRelease();
        }
        else
        {
            if (justClicked)
            {
                return;
            }
            else
            {
                justClicked = true;
            }

            OnClick();
        }
    }

    public void OnRelease()
    {
        Debug.Log("released");
        if (!currentlyDragging || !currentObject)
        {
            return;
        }

        currentObject.SetDragging(false);
        if (prevHoverTile)
        {
            Tile thisTile = prevHoverTile.GetComponent<Tile>();
            Person thisPerson = currentObject.GetComponent<Person>();
            if (thisTile && thisTile.CanPlaceHere())
            {
                thisTile.SetPerson(thisPerson, true);
                thisPerson.SetCurrentTile(thisTile);
                thisPerson.transform.position = thisTile.GetPersonLocation();
                currentObject.SetPlaced();
                currentObject = null;
                currentlyDragging = false;

            }
            else
            {
                if (currentObject.WasPlaced())
                {
                    thisPerson.GetCurrentTile().SetPerson(thisPerson);
                    thisPerson.transform.position = thisPerson.GetCurrentTile().GetPersonLocation();
                    thisPerson.GetCurrentTile().GetComponent<TileHighlight>().SetDefaultColor();
                }
                else
                {
                    Destroy(currentObject.gameObject);
                }

            }

            prevHoverTile.GetComponent<TileHighlight>().SetHoverColor(false);
        }
        else
        {
            Destroy(currentObject.gameObject);
        }

        currentObject = null;
        currentlyDragging = false;
    }

    public void SetDirection(InputAction.CallbackContext ctx)
    {
        if (!currentlyDragging || !currentObject)
        {
            return;
        }

        Person person = currentObject.GetComponent<Person>();

        if (!person || !person.HasDirection())
        {
            return;
        }

        float x = ctx.ReadValue<Vector2>().x;
        float y = ctx.ReadValue<Vector2>().y;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < -.1f)
            {
                person.SetDirection(Person.Direction.LEFT);
            }
            else if (x > .1f)
            {
                person.SetDirection(Person.Direction.RIGHT);
            }
        }
        else if (Mathf.Abs(y) > Mathf.Abs(x))
        {
            if (y < -.1f)
            {
                person.SetDirection(Person.Direction.DOWN);
            }
            else if (y > .1f)
            {
                person.SetDirection(Person.Direction.UP);
            }
        }
    }
}
