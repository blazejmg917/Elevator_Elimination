using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField, Tooltip("this person's id")] private string personId;

    [SerializeField, Tooltip("the sprite for when the player is dead")]private Sprite deadSprite;
    [SerializeField, Tooltip("mark true if this person is the target for this level")] private bool isTarget = false;
    [SerializeField, Tooltip("if this person blocks a space")] private bool takesUpSpace = true;
    [SerializeField, Tooltip("if this person will fail the level if seen")] private bool triggerAlarmOnSeen = false;
    [SerializeField, Tooltip("if this person can be seen through")] private bool transparent = false;
    [SerializeField, Tooltip("the Tile this Person is on")] private Tile currentTile;
    [SerializeField, Tooltip("the direction this person is facing")]private Direction currentFacing = Direction.LEFT;
    private Direction lastFacing;
    [SerializeField, Tooltip("this person's behavior")] private personBehavior behavior = new personBehavior();

    private bool isMoving = false;
    private Vector3 goalPos = Vector3.zero;
    [SerializeField, Tooltip("the speed at which this person gets shoved")] private float pushSpeed;
    private Animator anim;
    private SpriteRenderer spriteRen;
    private float animOffset;
    [SerializeField, Tooltip("Number of frames offset to start the player's idle animation")] private float initialOffset = 4f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();
        if (!anim) {
            return;
        }
        //anim.SetFloat("NormalizedTime", initialOffset / 56);
        anim.Rebind();
        anim.Update(0f);
        TurnSprite();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            goalPos = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, goalPos, pushSpeed * Time.fixedDeltaTime);
            if(transform.position == goalPos)
            {
                isMoving = false;
                TileManager.Instance.UpdateLevel();
            }
        }
    }

    public bool TakesUpSpace()
    {
        return takesUpSpace;
    }

    private void OnValidate()
    {
        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();
        if (!anim) {
            return;
        }
        if(lastFacing  != currentFacing ){
            lastFacing = currentFacing;
            if(currentTile){
                currentTile.SetDirection(currentFacing);
            }
            TurnSprite();
        }
    }

    private void TurnSprite()
    {
        // if (anim.GetInteger("FacingDirection") == 1) {
        //     spriteRen.flipX = true;
        // } else {
        //     spriteRen.flipX = false;
        // }
        animOffset = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
        anim.SetFloat("NormalizedTime", animOffset);
        switch(currentFacing) {
            case Direction.LEFT:
                anim.SetInteger("FacingDirection", 3);
                break;
            case Direction.RIGHT:
                anim.SetInteger("FacingDirection", 1);
                break;
            case Direction.UP:
                anim.SetInteger("FacingDirection", 0);
                break;
            case Direction.DOWN:
                anim.SetInteger("FacingDirection", 2);
                break;
        }
    }

    public bool OnPush(PlayerMechanics.DirectionFacing dir)
    {
        if (behavior.canPush && !isMoving)
        {
            switch (dir) {
                case PlayerMechanics.DirectionFacing.Left:
                    TryMove(currentTile.GetLeft());
                    break;

                case PlayerMechanics.DirectionFacing.Right:
                    TryMove(currentTile.GetRight());
                    break;

                case PlayerMechanics.DirectionFacing.Down:
                    TryMove(currentTile.GetBottom());
                    break;

                case PlayerMechanics.DirectionFacing.Up:
                    TryMove(currentTile.GetTop());
                    break;
            }
            MusicScript.Instance.GuhSFX();
            return true;
            
        }
        return false;
    }

    public string GetId(){
        return personId;
    }

    private bool TryMove(Tile newTile)
    {
        if (!newTile)
        {
            return false;
        }
        if (newTile.IsWalkable())
        {
            currentTile.SetPerson(null);
            currentTile = newTile;
            newTile.SetPerson(this);
            isMoving = true;
            return true;
        }
        return false;

    }

    public bool OnTap(PlayerMechanics.DirectionFacing dir)
    {
        if (behavior.canTurn)
        {
            switch (dir)
            {
                case PlayerMechanics.DirectionFacing.Left:
                    currentFacing = Direction.RIGHT;
                    break;

                case PlayerMechanics.DirectionFacing.Right:
                    currentFacing = Direction.LEFT;
                    break;

                case PlayerMechanics.DirectionFacing.Down:
                    currentFacing = Direction.UP;
                    break;

                case PlayerMechanics.DirectionFacing.Up:
                    currentFacing = Direction.DOWN;
                    break;
            }
            TurnSprite();
            MusicScript.Instance.HuhSFX();
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
                triggerAlarmOnSeen = true;
                GameManager.Instance.SetWinCon(true);
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

    public bool OnFloorChange()
    {
        bool sightlineCleared = false;
        Tile tileSeen = currentTile;
        while (!sightlineCleared)
        {
            if (!tileSeen) {
                return true;
            }
            switch (currentFacing) {
                case Direction.LEFT:
                    tileSeen = tileSeen.GetLeft();
                    break;
                case Direction.RIGHT:
                    tileSeen = tileSeen.GetRight();
                    break;
                case Direction.UP:
                    tileSeen = tileSeen.GetTop();
                    break;
                case Direction.DOWN:
                    tileSeen = tileSeen.GetBottom();
                    break;
                default: 
                    return true;
                
            }
            if (tileSeen)
            {
                Person seenPerson = tileSeen.GetPerson();
                if (seenPerson)
                {
                    Debug.Log("Seen");
                    if (seenPerson.CallAlarmWhenSeen())
                    {
                        GameManager.Instance.SetLoseCon(true);
                        MusicScript.Instance.ScreamSFX();
                        Debug.Log("WE WOOOH");
                        //call game over
                        return false;
                    }
                    return true;
                }

            }
            else {
                return true;
            }
        }
        return true;
    }

    public void SetDeadSprite()
    {
        GetComponent<SpriteRenderer>().sprite = deadSprite;
        GetComponent<Animator>().enabled = false;
    }

    public bool CallAlarmWhenSeen()
    {
        return triggerAlarmOnSeen;
    }

    public Tile GetCurrentTile() {
        return currentTile;
    }
    public void SetCurrentTile(Tile newTile){
        currentTile = newTile;
    }
    
    public Direction GetDirection() {
        return currentFacing;
    }

    public void SetDirection(Direction direction){
        currentFacing = direction;

    }
    
}
