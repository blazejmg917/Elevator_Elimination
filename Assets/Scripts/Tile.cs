using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.EditorTools;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Tooltip("The character currently on this tile")] private Person currentPerson;
    private Person lastPerson = null;
    [SerializeField, Tooltip("the id for the player on this tile"), ReadOnly(true)]private string personId;
    [SerializeField, Tooltip("The tile to the left")] private Tile leftTile;
    [SerializeField, Tooltip("The tile to the right")] private Tile rightTile;
    [SerializeField, Tooltip("The tile to the top")] private Tile topTile;
    [SerializeField, Tooltip("The tile to the bottom")] private Tile bottomTile;
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

    void OnValidate(){
        if(lastPerson != currentPerson){
            currentPerson = lastPerson;
            personId = currentPerson.GetId();
        }
    }
}
