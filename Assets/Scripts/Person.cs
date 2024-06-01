using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
        [Tooltip("if this person will push things directly in front of them")]public bool pushInFront;

        public personUniqueActions(bool hungry = true, bool loud = true, bool skeptical = true, bool pushy = true){
            eatInFront = hungry;
            alertSurrounding = loud;
            soundAlarm = skeptical;
            pushInFront = pushy;
        }
    }
    [System.Serializable]
    public struct personBehavior
    {
        [Tooltip("if this character can be pushed")] public bool canPush;
        [Tooltip("if this character can be turned")] public bool canTurn;
        [Tooltip("if this character can be killed")] public bool canBeKilled;
        [Tooltip("if this character can see")] public bool canSee;
        [Tooltip("if this character can eat")]public bool canEat;

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
    [SerializeField, Tooltip("the description for this person to be displayed in tooltips")] private string tooltipDescription;

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
    private Animator bubbleAnim;
    private Animator chatAnim;
    private SpriteRenderer spriteRen;
    [SerializeField, Tooltip("number of turns it takes for a shark to eat")] private int numberOfTurnsBeforeSharkCanEat = 3;
    //Offsets the animation time to sync up with the people around it
    //private float animOffset;
    [SerializeField, Tooltip("Number of frames offset to start the player's idle animation")] private float initialOffset = 4f;
    public enum Action {
        TAPPED,
        PUSHED,
        ALERTED,
        KILLED,
        AWOKEN,
        EAT,
        GORILLAPUSHED,
        NONE
    }
    //Stack to store the person's last tile it was on, the last direction it was facing, what floor number the action was made on, and (if the shark) what person was last next to it
    private Stack<(Tile tile, Direction direction, int floorNumber, Person lastPersonInRange, Action action)> states;
    private int numberOfTurnsInSharkRange = 0;
    private bool undoGorilla = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!hasDirection)
        {
            currentFacing = Direction.NONE;
        }
        states = new Stack<(Tile, Direction, int, Person, Action)>();
        if (currentTile)
        {
            transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
        }

        anim = GetComponent<Animator>();
        if (transform.childCount > 0) {
            bubbleAnim = transform.GetChild(0).GetComponent<Animator>();
            bubbleAnim.enabled = false;
            chatAnim = transform.GetChild(1).GetComponent<Animator>();
            chatAnim.enabled = false;
        }
        spriteRen = GetComponent<SpriteRenderer>();
        if (!anim) {
            return;
        }
        anim.Rebind();
        anim.Update(0f);
        TurnSprite();
        if (behavior.canEat) {
            CheckIfGameStartsWithPersonInSharkRange();
        }
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
                if (undoGorilla) {
                    UndoState();
                    return;
                }
                TileManager.Instance.UpdateLevel();
            }
        }
    }

    public bool IsTarget(){
        return isTarget;
    }

    public bool IsEdible(){
        return !behavior.canEat;
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
    /**
     * Restarts the bubble reaction animation
     * @param bool that sets the animation to either play the exclamation mark animation if true or the question mark animation if false
     */
    public void StartBubbleReaction(bool exclamation) {
        if (bubbleAnim) {
            bubbleAnim.enabled = true;
            bubbleAnim.Rebind();
            bubbleAnim.Update(0.9f);
            bubbleAnim.SetBool("ExclamationReaction", exclamation);
            chatAnim.enabled = true;
            chatAnim.Rebind();
            chatAnim.Update(0.9f);
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
                    TryMove(currentTile.GetLeft(), false);
                    break;

                case PlayerMechanics.DirectionFacing.Right:
                    TryMove(currentTile.GetRight(), false);
                    break;

                case PlayerMechanics.DirectionFacing.Down:
                    TryMove(currentTile.GetBottom(), false);
                    break;

                case PlayerMechanics.DirectionFacing.Up:
                    TryMove(currentTile.GetTop(), false);
                    break;
            }
            SFXManager.Instance.GuhSFX();
            return true;
        }
        return false;
    }
    
    /*
     * Undoes tap or push depending on if the floor number of the last action matches the current floor
     */
    public bool UndoState() {
        if (states.Count == 0) {
            undoGorilla = false;
            return false;
        }
        Debug.Log("floor number: " + states.Peek().floorNumber);
        // if (CompareTag("SleepyGuy")) {
        //     Debug.Log(states.Peek().direction);
        // }
        if ((states.Count > 0 && states.Peek().floorNumber == GameManager.Instance.GetCurrentFloor() + 1) || undoGorilla) {
            Direction lastFacing = states.Peek().direction;
            Action lastAction = states.Peek().action;
            Tile lastTile = states.Peek().tile;
            int lastFloorNumber = states.Peek().floorNumber;
            if (currentTile.getCoords() != lastTile.getCoords()) {
                if (currentTile.GetPerson() == this) {
                    currentTile.SetPerson(null);
                } 
                currentTile = lastTile;
                lastTile.SetPerson(this);
                isMoving = true;
            }
            Debug.Log("Before Gorilla Undo");
            if (lastAction == Action.GORILLAPUSHED) {
                states.Pop();
                undoGorilla = true;
                return true;
            }
            if (currentFacing != lastFacing) {
                currentFacing = lastFacing;
                TurnSprite();
                //Next two lines fix the undo issue with tap by artificially increasing floor count when undoing a tap
                // if (lastAction == Action.ALERTED && gameObject.tag == "SleepyGuy" && anim) {
                //     anim.SetBool("Alarm", false);
                //     GameManager.Instance.UndoFloor(states.Peek().floorNumber + 1);
                //     TileManager.Instance.UpdateLevel();
                // }
                if (lastAction == Action.TAPPED) {
                    // GameManager.Instance.UndoFloor(lastFloorNumber + 1);
                    // TileManager.Instance.UpdateLevel();
                }
                if (lastAction == Action.AWOKEN && anim) {
                    anim.SetTrigger("WakeUp");
                    GameManager.Instance.UndoAwake = true;
                }
            }
            if (lastAction == Action.KILLED) {
                OnRevive();
                // GameManager.Instance.UndoFloor(lastFloorNumber + 1);
                // TileManager.Instance.UpdateLevel();
            }
            if (lastAction == Action.EAT && states.Peek().lastPersonInRange && states.Peek().lastPersonInRange.GetSharkTurns() > 0) {
                Person lastSharkPerson = states.Peek().lastPersonInRange;
                lastSharkPerson.SetSharkTurns(lastSharkPerson.GetSharkTurns() - 2);
                Debug.Log("Unddid Shark: " + lastSharkPerson.GetSharkTurns());
            } 
            states.Pop();
            undoGorilla = false;
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
            Tile frontTile = GetFrontTile(null);
            if(!frontTile.GetPerson() && states.Count > 0 && states.Peek().lastPersonInRange) {
                states.Peek().lastPersonInRange.SetSharkTurns(0);
            }
            if(frontTile && frontTile.GetPerson() && frontTile.GetPerson().IsEdible()){
                if (states.Count > 0) {
                    Person lastPersonInSharkRange = states.Peek().lastPersonInRange;

                    //If the last person in the shark range is not the same person as the current person in the range, reset the last person's counter to 0
                    if (lastPersonInSharkRange.GetInstanceID() != frontTile.GetPerson().GetInstanceID()) {
                        lastPersonInSharkRange.SetSharkTurns(0);
                    }
                }
                Person newSharkPerson = frontTile.GetPerson();
                newSharkPerson.SetSharkTurns(newSharkPerson.GetSharkTurns() + 1);
                if(newSharkPerson.GetSharkTurns() == numberOfTurnsBeforeSharkCanEat) {
                    anim.SetTrigger("Eat");
                    newSharkPerson.OnKill(true);
                    newSharkPerson.SetSharkTurns(0);
                }
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), newSharkPerson, Action.EAT));
            }
        }
        if(actions.pushInFront) {
            Tile frontTile = GetFrontTile(null);
            if(frontTile && frontTile.GetPerson()) {
                Tile tileInFrontOfFrontTile = GetFrontTile(frontTile);
                if (tileInFrontOfFrontTile) {
                    Person personMoving = frontTile.GetPerson();
                    personMoving.TryMove(tileInFrontOfFrontTile, true);
                }
            }
            else if(frontTile && frontTile.GetPlayer()) {
                Tile tileInFrontOfFrontTile = GetFrontTile(frontTile);
                if (tileInFrontOfFrontTile) {
                    PlayerMechanics playerMoving = frontTile.GetPlayer();
                    playerMoving.GorillaMove(tileInFrontOfFrontTile);
                }
            }
        }
        if(actions.soundAlarm){
            GameManager.Instance.GameOver("SEEN");
        }
    }
    /**
     * Gets tile in front of current tile or tile passed in from the parameter according to direction facing
     */
    private Tile GetFrontTile(Tile newTile) {
        Tile tileToCheck = currentTile;
        if (newTile) {
            tileToCheck = newTile;
        }
        Tile frontTile = null;
        switch(currentFacing){
            case Direction.LEFT:
                frontTile = tileToCheck.GetLeft();
                break;
            case Direction.RIGHT:
                frontTile = tileToCheck.GetRight();
                break;
            case Direction.UP:
                frontTile = tileToCheck.GetTop();
                break;
            case Direction.DOWN:
                frontTile = tileToCheck.GetBottom();
                break;
        }
        return frontTile;
    }
    private void CheckIfGameStartsWithPersonInSharkRange() {
        Tile frontTile = GetFrontTile(null);
        if(frontTile.GetPerson() && frontTile.GetPerson().IsEdible()) {
            frontTile.GetPerson().SetSharkTurns(1);
        }
    }
    private void SetSharkTurns(int turns) {
        numberOfTurnsInSharkRange = turns;
    }
    private int GetSharkTurns() {
        return numberOfTurnsInSharkRange;
    }
    public string GetId(){
        return personId;
    }
    /**
     * Tries to move to a new tile, bool stores whether or not it was triggered by a gorilla
     */
    private bool TryMove(Tile newTile, bool gorillaMove)
    {
        if (!newTile)
        {
            return false;
        }
        if (newTile.IsWalkable())
        {
            BeforeInteract();
            if (gorillaMove) {
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), null, Action.GORILLAPUSHED));
                GameManager.Instance.UndoFloor(GameManager.Instance.GetCurrentFloor() + 1);
            } else {
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), null, Action.PUSHED));
            }
            if (currentTile.GetPerson() == this) {
                currentTile.SetPerson(null);
            }
            //positions.Push(currentTile.transform.position);
            currentTile = newTile;
            newTile.SetPerson(this);
            isMoving = true;
            StartBubbleReaction(true);
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
            if (CompareTag("SleepyGuy") && anim) {
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), null, Action.AWOKEN));
                anim.SetTrigger("WakeUp");
            } else {
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), null, Action.TAPPED));
            }
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
            StartBubbleReaction(false);
            SFXManager.Instance.HuhSFX();
            AfterInteract();
            return true;
        }
        return false;
    }
    public void OnRevive() {
        Animator doorAnim = GameObject.FindGameObjectWithTag("Elevator Door").GetComponent<Animator>();
        if (doorAnim) {
            doorAnim.Rebind();
            doorAnim.Update(0f);
            doorAnim.SetBool("Open Door", false);
            doorAnim.SetBool("Close Door", true);
        }
        SetAliveAnimation();
        takesUpSpace = true;
        triggerAlarmOnSeen = false;
        LevelManager.Instance.TargetRevived();
        
    }
    public bool OnKill(bool overrideKillable = false)
    {
        if ((behavior.canBeKilled || overrideKillable) && GetComponent<Animator>().enabled )
        {
            SetDeadSprite();
            takesUpSpace = false;
            triggerAlarmOnSeen = true;
            if (isTarget)
            {
                
                LevelManager.Instance.TargetKilled();
                states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), null, Action.KILLED));
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
                        //Temp reaction to kill to show who caused the failed level
                        StartBubbleReaction(true);
                        GameManager.Instance.GameOver("SEEN");
                        SFXManager.Instance.ScreamSFX();
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
            states.Push((currentTile, currentFacing, GameManager.Instance.GetCurrentFloor(), null, Action.ALERTED));
            currentFacing = direction;
        }
        TurnSprite();
        StartBubbleReaction(false);
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

    public string GetDescription()
    {
        return tooltipDescription;
    }
}