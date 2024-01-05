using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    [SerializeField, Tooltip("drag attach point. Transform by default")]
    private Transform dragAttachPoint;
    //[SerializeField, Tooltip("the current transform to follow")]
    //private Transform dragFollowPoint;
    private bool dragged = false;

    [SerializeField]private bool wasPlaced = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!dragAttachPoint)
        {
            dragAttachPoint = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dragged)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0) + (dragAttachPoint.position - transform.position);
        }
    }

    public void SetDragging(bool dragging)
    {
        dragged = dragging;
    }

    public void SetPlaced()
    {
        wasPlaced = true;
    }

    public bool WasPlaced()
    {
        return wasPlaced;
    }
}
