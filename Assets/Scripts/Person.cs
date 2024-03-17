using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.EditorTools;
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
    [System.Serializable]public struct personUniqueActions{
        [Tooltip("If this person ill try to eat a person directly in front of them")]public bool eatInFront;
        [Tooltip("if this person will alert all direct line of sight people in all directions from it")]public bool alertSurrounding;
        [Tooltip("if this person will sound the alarm and fail the level")]public bool soundAlarm;

        public personUniqueActions(bool hungry = true, bool loud = true, bool skeptical = true){
            eatInFront = hungry;
            alertSurrounding = loud;
            soundAlarm = skeptical;
        }
    }
    [System.Serializable]
    public struct personBehavior
    {
        [Tooltip("if this character can be pushed")] public bool canPush;
        [Tooltip("if this character can be turned")] public bool canTurn;
        [Tooltip("if this character can be killed")] public bool canBeKilled;
        [Tooltip("if this character can see")] public bool canSee;
        [Tooltip("if this character can be eaten")]public bool canEat;

        [Tooltip("the actions this character can take before being interacted with")]public personUniqueActions beforeInteract;
        [Tooltip("the actions this character can take after being interacted with")]public personUniqueActions afterInteract;
        [Tooltip("the actions this character can take on turn change")]public personUniqueActions onTurnChange;
        [Tooltip("NOT WORKING YET. PLACEHOLDER \n the actions this character can take when they see the player")]public personUniqueActions onSeePlayer;
        

        public personBehavior(bool pushable = true, bool turnable = true, bool killable = true, bool hasSight = true, bool yummy = true)
        {
            canPush = pushable;
            canTurn = turnable;
            canBeKilled = killable;
            canSee = hasSight;
            canEat = yummy;
            beforeInteract = new personUniqueActions();
            afterInteract = new personUniqueActions();
            onTurnChange = new personUniqueActions();
            onSeePlayer = new personUniqueActions();
        }
    }

    [SerializeField, Tooltip("this person's id")] private string personId;

    [SerializeField, Tooltip("the key used for this person in level creation")]
    private string personKey;

    [SerializeField, Tooltip("the sprite for when the player is dead")]private Sprite deadSprite;
    [SerializeField, Tooltip("mark true if this person is the target for this level")] private bool isTarget = false;
    [SerializeField, Tooltip("if this person blocks a space")] private bool takesUpSpace = true;
    [SerializeField, Tooltip("if this person will fail the level if seen")] private bool triggerAlarmOnSeen = false;
    [SerializeField, Tooltip("if this person can be seen through")] private bool transparent = false;
    [SerializeField, Tooltip("the Tile this Person is on")] private Tile currentTile;
    [SerializeField, Tooltip("the direction this person is facing")]private Direction currentFacing = Direction.LEFT;
    private Direction lastFacing;
    [SerializeField, Tooltip("if this object has any direction to it. If not, will be set to no direction by default")]
    private bool hasDirection;
    [SerializeField, Tooltip("this person's behavior")] private personBehavior behavior = new personBehavior();

    private bool isMoving = false;
    private Vector3 goalPos = Vector3.zero;
    [SerializeField, Tooltip("the speed at which this person gets shoved")] private float pushSpeed;
    private Animator anim;
    private SpriteRenderer spriteRen;
    //Offsets the animation time to sync up with the people around it
    //private float animOffset;
    [SerializeField, Tooltip("Number of frames offset to start the player's idle animation")] private float initialOffset = 4f;
    public enum Action {
        TAPPED,
        PUSHED,
        ALERTED,
        KILLED,
        NONE
    }
    //Stack to store the person's last tile it was on, the last direction it was facing, and what floor number the action was made on
    private Stack<(Tile tile, Direction direction, int floorNumber, Action action)> states;
    // Start is called before the first frame update
    void Start()
    {
        if (!hasDirection)
        {
            currentFacing = Direction.NONE;
        }
        states = new Stack<(Tile, Direction, int, Action)>();
        if (currentTile)
        {
            transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
        }

        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();
        if (!anim) {
            return;
        }
        anim.Rebind();
        anim.Update(0f);
        TurnSprite();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            goalPos = currentTile.GetPersonLocation();//new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, goalPos, pushSpeed * Time.fixedDeltaTime);
            if(transform.position == goalPos)
            {
                isMoving = false;
                AfterInteract();
                TileManager.Instance.UpdateLevel();
            }
        }
    }

    public bool IsTarget(){
        return isTarget;
    }

    public bool IsEdible(){
        return behavior.canEat;
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
        if(!anim){
            return;
        }
        // if (anim.GetInteger("FacingDirection") == 1) {
        //     spriteRen.flipX = true;
        // } else {
        //     spriteRen.flipX = false;
        // }
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

    public void OnBob(bool goingDown)
    {
        if (anim) {
            anim.SetBool("BobbedDown", goingDown);
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
            AudioScript.Instance.GuhSFX();
            return true;
        }
        return false;
    }

    /*
     * Undoes tap or push depending on if the floor number of the last action matches the current floor
     */
    public bool UndoState() {
        if (states.Count != 0) {
            Debug.Log("floor number: " + states.Peek().floorNumber);
        }
        if (states.Count != 0 && states.Peek().floorNumber == GameManager.Instance.GetCurrentFloor() + 1) {
            Tile lastTile = states.Peek().tile;
            if (currentTile.transform.position != lastTile.transform.position) {
                currentTile.SetPerson(null);
                currentTile = lastTile;
                lastTile.SetPerson(this);
                isMoving = true;
            }
            Direction lastFacing = states.Peek().direction;
            Action lastAction = states.Peek().action;
            if (currentFacing != lastFacing) {
                currentFacing = lastFacing;
                TurnSprite();
                //Next two lines fix the undo issue with tap by artificially increasing floor count when undoing a tap
                // if (lastAction == Action.ALERTED && gameObject.tag == "SleepyGuy" && anim) {
                //     anim.SetBool("Alarm", false);
                //     GameManager.Instance.UndoFloor(states.Peek().floorNumber + 1);
                //     TileManager.Instance.UpdateLevel();
                // }
                if (lastAction == Action.TAPPED || lastAction == Action.ALERTED) {
                    GameManager.Instance.UndoFloor(states.Peek().floorNumber + 1);
                    TileManager.Instance.UpdateLevel();
                }
            }
            if (lastAction == Action.KILLED) {
                OnRevive();
                GameManager.Instance.UndoFloor(states.Peek().floorNumber + 1);
                TileManager.Instance.UpdateLevel();
            } 
            states.Pop();
            return true;
        }
        return false;
    }

    private void BeforeInteract(){
        HandleActions(behavior.beforeInteract);
    }

    private void AfterInteract(){
        HandleActions(behavior.afterInteract);
    }



    private void OnSeePlayer(){
        HandleActions(behavior.onSeePlayer);
    }

    private void HandleActions(personUniqueActions actions){
        if(actions.alertSurrounding){
            //check top
            if (gameObject.tag == "SleepyGuy" && anim) {
                anim.SetBool("Alarm", true);
            }
            Tile thisTile = currentTile.GetTop();
            while(thisTile){
                if(thisTile && thisTile.GetPerson()){
                    thisTile.GetPerson().SetAlarmDirection(Direction.DOWN);
                    
                }
                thisTile = thisTile.GetTop();
            }
            //check bottom
            thisTile = currentTile.GetBottom();
            while(thisTile){
                if(thisTile && thisTile.GetPerson()){
                    thisTile.GetPerson().SetAlarmDirection(Direction.UP);
                    
                }
                thisTile = thisTile.GetBottom();
            }
            //check right
            thisTile = currentTile.GetRight();
            while(thisTile){
                if(thisTile && thisTile.GetPerson()){
                    thisTile.GetPerson().SetAlarmDirection(Direction.LEFT);
                    
                }
                thisTile = thisTile.GetRight();
            }
            //check left
            thisTile = currentTile.GetLeft();
            while(thisTile){
                if(thisTile && thisTile.GetPerson()){
                    thisTile.GetPerson().SetAlarmDirection(Direction.RIGHT);
                    
                }
                thisTile = thisTile.GetLeft();
            }
        }
        if(actions.eatInFront){
            Tile frontTile = null;
            switch(currentFacing){
                case Direction.LEFT:
                    frontTile = currentTile.GetLeft();
                    break;
                case Direction.RIGHT:
                    frontTile = currentTile.GetRight();
                    break;
                case Direction.UP:
                    frontTile = currentTile.GetTop();
                    break;
                case Direction.DOWN:
                    frontTile = currentTile.GetBottom();
                    break;
            }
            if(frontTile && frontTile.GetPerson() && frontTile.GetPerson().IsEdible()){
                frontTile.GetPerson().OnKill(true);
            }
        }
        if(actions.soundAlarm){
            GameManager.Instance.GameOver("SEEN");
        }
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
            BeforeInteract();
            states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), Action.PUSHED));
            currentTile.SetPerson(null);
            //positions.Push(currentTile.transform.position);
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
            if ((dir == PlayerMechanics.DirectionFacing.Right && currentFacing == Direction.LEFT) || (dir == PlayerMechanics.DirectionFacing.Left && currentFacing == Direction.RIGHT) || (dir == PlayerMechanics.DirectionFacing.Down && currentFacing == Direction.UP) || (dir == PlayerMechanics.DirectionFacing.Up && currentFacing == Direction.DOWN)) {
                return false;
            }
            BeforeInteract();
            //if (gameObject.tag == "SleepyGuy") {
                //states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), Action.ALERTED));
            //} else {
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), Action.TAPPED));
            //}
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
            AudioScript.Instance.HuhSFX();
            AfterInteract();
            return true;
        }
        return false;
    }
    public void OnRevive() {
        SetAliveAnimation();
        takesUpSpace = true;
        triggerAlarmOnSeen = false;
        LevelManager.Instance.TargetRevived();
        
    }
    public bool OnKill(bool overrideKillable = false)
    {
        if (behavior.canBeKilled || overrideKillable)
        {
            SetDeadSprite();
            takesUpSpace = false;
            triggerAlarmOnSeen = true;
            if (isTarget)
            {
                
                LevelManager.Instance.TargetKilled();
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), Action.KILLED));
                //GameManager.Instance.SetWinCon(true);
                //call target killed
                return true;
            }
            else
            {
                GameManager.Instance.GameOver("EATEN");
                //call level failed
                return true;
            }
        }
        return false;
        
    }

    public bool OnFloorChange()
    {
        HandleActions(behavior.onTurnChange);
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
                        GameManager.Instance.GameOver("SEEN");
                        AudioScript.Instance.ScreamSFX();
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
    public void SetAliveAnimation() {
        GetComponent<Animator>().enabled = true;
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
    /**
     * Sets the direction according to which way the bird triggers the alarm
     * @param direction the direction to change the person's animation
     */
    public void SetAlarmDirection(Direction direction) {
        if(currentFacing != Direction.NONE || currentFacing != direction){
            states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), Action.ALERTED));
            currentFacing = direction;
        }
        TurnSprite();
    }
    public void SetDirection(Direction direction){
        if(currentFacing != Direction.NONE){
            currentFacing = direction;
        }
        TurnSprite();
    }

    public string GetKey()
    {
        return personKey;
    }

    public void SetColor(Color color)
    {
        spriteRen.color = color;
    }

    public bool HasDirection()
    {
        return hasDirection;
    }
}