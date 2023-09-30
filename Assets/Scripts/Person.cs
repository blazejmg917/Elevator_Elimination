using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        NONE
    }
    [System.Serializable]
    public struct personBehavior
    {
        [Tooltip("if this character can be pushed")] public bool canPush;
        [Tooltip("if this character can be turned")] public bool canTurn;
        [Tooltip("if this character can be killed")] public bool canBeKilled;
        [Tooltip("if this character can see")] public bool canSee;

        public personBehavior(bool pushable = true, bool turnable = true, bool killable = true, bool hasSight = true)
        {
            canPush = pushable;
            canTurn = turnable;
            canBeKilled = killable;
            canSee = hasSight;
        }
    }
    [SerializeField, Tooltip("mark true if this person is the target for this level")] private bool isTarget = false;
    [SerializeField, Tooltip("if this person blocks a space")] private bool takesUpSpace = true;
    [SerializeField, Tooltip("if this person will fail the level if seen")] private bool triggerAlarmOnSeen = false;
    [SerializeField, Tooltip("if this person can be seen through")] private bool transparent = false;
    [SerializeField, Tooltip("the Tile this Person is on")] private Tile currentTile;
    [SerializeField, Tooltip("the direction this person is facing")]private Direction currentFacing = Direction.LEFT;
    [SerializeField, Tooltip("this person's behavior")] private personBehavior behavior = new personBehavior();

    private bool isMoving = false;
    private Vector3 goalPos = Vector3.zero;
    [SerializeField, Tooltip("the speed at which this person gets shoved")] private float pushSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalPos, pushSpeed * Time.deltaTime);
            if(transform.position == goalPos)
            {
                isMoving = false;
            }
        }
    }

    public bool TakesUpSpace()
    {
        return takesUpSpace;
    }

    private void OnValidate()
    {
        TurnSprite();
    }

    private void TurnSprite()
    {
        //update sprite to match direction
    }

    public bool OnPush(Direction dir)
    {
        if (behavior.canPush)
        {
            switch (dir) {
                case Direction.LEFT:
                    TryMove(currentTile.GetLeft());
                    break;

                case Direction.RIGHT:
                    TryMove(currentTile.GetRight());
                    break;

                case Direction.DOWN:
                    TryMove(currentTile.GetBottom());
                    break;

                case Direction.UP:
                    TryMove(currentTile.GetTop());
                    break;
            }
            return true;
            
        }
        return false;
    }



    private bool TryMove(Tile newTile)
    {
        if (!newTile)
        {
            return false;
        }
        if (newTile.IsWalkable())
        {
            currentTile = newTile;
            return true;
        }
        return false;

    }

    public bool OnTap(Direction dir)
    {
        if (behavior.canTurn)
        {
            switch (dir)
            {
                case Direction.LEFT:
                    currentFacing = Direction.RIGHT;
                    break;

                case Direction.RIGHT:
                    currentFacing = Direction.LEFT;
                    break;

                case Direction.DOWN:
                    currentFacing = Direction.UP;
                    break;

                case Direction.UP:
                    currentFacing = Direction.DOWN;
                    break;
            }
            TurnSprite();
            return true;
        }
        return false;
    }

    public bool OnKill()
    {
        if (behavior.canBeKilled)
        {
            if (isTarget)
            {
                SetDeadSprite();
                takesUpSpace = false;
                //call target killed
                return true;
            }
            else
            {
                //call level failed
                return true;
            }
        }
        return false;
        
    }

    public void OnLevelUpdate()
    {
        bool sightlineCleared = false;
        Tile tileSeen = currentTile;
        while (!sightlineCleared)
        {
            switch (currentFacing) {
                case Direction.LEFT:
                    tileSeen = currentTile.GetLeft();
                    break;
                case Direction.RIGHT:
                    tileSeen = currentTile.GetRight();
                    break;
                case Direction.UP:
                    tileSeen = currentTile.GetTop();
                    break;
                case Direction.DOWN:
                    tileSeen = currentTile.GetBottom();
                    break;
            }
            if (tileSeen)
            {
                
            }
                
        }
    }

    public void SetDeadSprite()
    {

    }

    public bool CallAlarmWhenSeen()
    {
        return triggerAlarmOnSeen;
    }
}