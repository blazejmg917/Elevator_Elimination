using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Tooltip("The character currently on this tile")] private Person currentPerson;
    private Person lastPerson = null;
    [SerializeField, Tooltip("the id for the player on this tile"), ReadOnly(true)]private string personId;
    [SerializeField, Tooltip("the look direction for the player on this tile"), ReadOnly(true)]private Person.Direction personDirection;
    [SerializeField, Tooltip("The tile to the left")] private Tile leftTile;
    [SerializeField, Tooltip("The tile to the right")] private Tile rightTile;
    [SerializeField, Tooltip("The tile to the top")] private Tile topTile;
    [SerializeField, Tooltip("The tile to the bottom")] private Tile bottomTile;
    [SerializeField, Tooltip("the person/character offset for this tile")]private Vector3 offset;
    [SerializeField, Tooltip("the sprite renderer for this tile")]private SpriteRenderer sRenderer;
    private int x = 0;
    private int y = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Tile GetTop()
    {
        return topTile;
    }
    public Tile GetBottom()
    {
        return bottomTile;
    }
    public Tile GetLeft()
    {
        return leftTile;
    }
    public Tile GetRight()
    {
        return rightTile;
    }

    public void SetTop(Tile top)
    {
        topTile = top;
    }
    public void SetBottom(Tile bottom)
    {
        bottomTile = bottom;
    }
    public void SetRight(Tile right)
    {
        rightTile = right;
    }
    public void SetLeft(Tile left)
    {
        leftTile = left;
    }

    public void TileSetupAdjacentTiles(Tile right, Tile left, Tile top, Tile bottom)
    {
        rightTile = right;
        leftTile = left;
        topTile = top;
        bottomTile = bottom;
    }

    public int getX()
    {
        return x;
    }
    public int getY()
    {
        return y;
    }

    public int[] getCoords()
    {
        return new int[]{x, y };
    }

    public void setLocation(int thisX, int thisY)
    {
        x = thisX;
        y = thisY;
    }

    public bool IsWalkable()
    {
        if(!currentPerson || !currentPerson.TakesUpSpace())
        {
            return true;
        }
        return false;
    }

    public Person GetPerson()
    {
        return currentPerson;
    }

    public void SetPerson(Person per) {
        currentPerson = per;
    }

    public string GetPersonId(){
        return personId;
    }

    public Person.Direction GetDirection(){
        return personDirection;
    }

    public void SetDirection(Person.Direction direction){
        personDirection = direction;
    }

    void OnValidate(){
        if(lastPerson != currentPerson && currentPerson != null){
            lastPerson = currentPerson;
            personId = currentPerson.GetId();
            //personDirection = currentPerson.GetDirection();
        }
        else if(lastPerson != currentPerson){
            lastPerson = null;
            personId = "";
            personDirection = Person.Direction.NONE;
        }
    }

    public Vector3 GetPersonLocation(){
        return transform.position + offset;
    }

    public void SetSprite(Sprite sprite){
        if(!sRenderer){
            sRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        sRenderer.sprite = sprite;
    }

    public void SetOffset(Vector3 tileOffset){
        offset = tileOffset;
    }
}
