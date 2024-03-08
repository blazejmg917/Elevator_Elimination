using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour
{
    public enum DirectionFacing
    {
        Left,
        Right,
        Up,
        Down
    }
    // public enum Action
    // {
    //     MoveLeft,
    //     MoveRight,
    //     MoveUp,
    //     MoveDown,
    //     Tap,
    //     Push,
    //     Kill,
    // }
    [System.Serializable]public class PlayerStartEvent : UnityEvent{};
    [System.Serializable]public class PlayerEscapeEvent : UnityEvent{};
    [System.Serializable]public class GameOverEvent : UnityEvent{};
    [SerializeField] private DirectionFacing facing = DirectionFacing.Down;
    [SerializeField, Tooltip("the player's current tile")]private Tile currentTile;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField, Tooltip("move speed for enter/exit")]private float enterExitSpeed = 3f;
    private bool isInteractible = false;
    private Person adjacentPerson = null;
    [SerializeField]private Vector3 targetPosition;
    private TileManager tileMan;
    private GameManager gameMan;
    private bool movePressed = false;
    private bool undoPressed = false;
    [SerializeField]private Tile exitTile;
    [SerializeField]private Vector3 exitPosition;
    [SerializeField]private bool isExiting = false;
    [SerializeField]private bool isStarting = true;
    private Vector2 controlDirection;
    private bool movedLeft = false;
    private bool movedRight = false;
    private bool movedUp = false;
    private bool movedDown = false;
    [SerializeField]private bool cautious;
    private bool neutral = true;
    private bool hasTapped = false;
    private bool hasKilled = false;
    private bool hasPushed = false;
    [SerializeField]private bool escaping = false;
    [SerializeField]private bool entering = false;
    [SerializeField]private bool waitingForLevel = true;
    [SerializeField]private Vector3 currentTilePos;
    [SerializeField] private Animation gameOverAnimation;
    private Animator anim;
    private SpriteRenderer spriteRen;
    [SerializeField, Tooltip("Starting/exit position")]private Transform startEndPos;
    [SerializeField, Tooltip("event for when player enters level")]private PlayerStartEvent levelStart = new PlayerStartEvent();
    [SerializeField, Tooltip("event for when player escapes level")]private PlayerEscapeEvent levelEnd = new PlayerEscapeEvent();
    [SerializeField, Tooltip("event for when player loses the level")]private GameOverEvent playerFail = new GameOverEvent();
    //Offsets the animation time to sync up with the people around it
    private float animOffset;
    [SerializeField, Tooltip("Number of frames offset to start the player's idle animation")] private float initialOffset = 4f;
    //Stack to store the previous actions, previous directions the player was facing, and previous people the player was next to if they exist
    private Stack<(Tile tile, DirectionFacing direction, int floorNumber)> playerStates;
    private bool targetDead = false;
    private PersonHolder personHolder;
    // Start is called before the first frame update
    void Start()
    {
        //Setup();
        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();
    }

    public void WalkIn() {
        Debug.Log("player entering");
        currentTile = tileMan.GetStartTile();
        currentTilePos = currentTile.transform.position;
        exitTile = currentTile;
        entering = true;
        spriteRen.enabled = true;
        //gameObject.SetActive(true);
        transform.position = new Vector3(currentTilePos.x, currentTilePos.y + 0.25f, transform.position.z);
        targetPosition = currentTile.GetPersonLocation();//new Vector3(currentTilePos.x, currentTilePos.y, transform.position.z);
        waitingForLevel = false;
        //gameObject.SetActive(true);
        spriteRen.enabled = true;
        MusicScript.Instance.BellSFX();
    }

    public void WalkOut() {
        targetPosition = new Vector3(startEndPos.position.x, startEndPos.position.y, transform.position.z);
        escaping = true;
        isInteractible = false;
        MusicScript.Instance.ExitDoorSFX();
        spriteRen.enabled = false;
        facing = DirectionFacing.Down;
    }

    public void Setup(){
        Debug.Log("player setup");
        tileMan = TileManager.Instance;
        gameMan = GameManager.Instance;
        playerStates = new Stack<(Tile, DirectionFacing, int)>();
        personHolder = PersonManager.Instance.GetPHolder();
        if(startEndPos){
            transform.position = startEndPos.position;
        }
        //WalkIn();
        cautious = gameMan.GetControlStyle();
        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();
        //gameObject.SetActive(false);
        spriteRen.enabled = false;
        escaping = false;
        gameMan.SetLoseCon(false);
        gameMan.SetWinCon(false);
        waitingForLevel=true;
        isStarting=true;
        isInteractible=false;
        facing = DirectionFacing.Down;
        adjacentPerson = null;
        anim.SetFloat("NormalizedTime", initialOffset / 56);
        initialOffset = 0;
        anim.SetInteger("Facing Direction", 2);
        anim.Rebind();
        anim.Update(0f);
        targetDead = false;
        movePressed = false;
        hasTapped = false;
        hasPushed = false;
        undoPressed = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(waitingForLevel || gameMan.GetLoseCon()){
            return;
        }
        //Runs the walk up animation and ends with the player able to perform actions
        if(entering){
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, enterExitSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                Debug.Log("player entered");
                entering = false;
                levelStart.Invoke();
                isInteractible = true;
                isStarting = false;
                exitPosition = transform.position;
                LevelManager.Instance.EnablePause(true);
            }
            return;
        }
        //Runs the escape animation and ends with the game loading the next animation
        if(escaping){
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, enterExitSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                Debug.Log("player escaped");
                escaping = false;
                isExiting = false;
                waitingForLevel = true;
                //gameObject.SetActive(false);
                spriteRen.enabled = false;
                levelEnd.Invoke();
                MusicScript.Instance.MischiefManaged();
                LevelManager.Instance.EnablePause(false);
            }
            return;
        }
        //Resets the booleans for if the player has moved in quick mode to false and neutral to true
        if (!cautious && Mathf.Abs(controlDirection.x) < 0.1f && Mathf.Abs(controlDirection.y) < 0.1f) {
            movedLeft = false;
            movedRight = false;
            movedUp = false;
            movedDown = false;
            neutral = true;
        }
        //Walks the player out of the elevator if the necessary requirements are met
        if (!isInteractible && gameMan.GetWinCon() && isExiting && !gameMan.GetLoseCon() && !isStarting) {
            WalkOut();
            if (transform.position == exitPosition) {
                gameMan.SetWinCon(false);
                //load next level along with animations
            }
        }
        //Moves the player to the target tile if the player is moving and checks to see if people are looking at the player
        if (!isInteractible && !isExiting && !gameMan.GetLoseCon() && !isStarting) {
            targetPosition = currentTile.GetPersonLocation();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                isInteractible = true;
                movePressed = false;
                undoPressed = false;
                MusicScript.Instance.StepSFX();
                if (playerStates.Count == 0 && facing != DirectionFacing.Down) {
                    facing = DirectionFacing.Down;
                    UpdateDirection();
                }
                if (!tileMan.UpdateLevel()){
                    GameManager.Instance.GameOver("SEEN");
                }
            }
        }
        //UpdateDirection();
    }

    /*
     * Turns the player depending on which direction the control stick is facing, which d-pad direction is pressed, or which directional key is pressed
     * @param ctx control input
     */
    public void Turn(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon() || waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float x = ctx.ReadValue<Vector2>().x;
        float y = ctx.ReadValue<Vector2>().y;
        cautious = gameMan.GetControlStyle();
        controlDirection = ctx.ReadValue<Vector2>();
        //Debug.Log("Turn " + x + ", " + y);
        //Checks to see if the player pressed a horizontal direction
        if (Mathf.Abs(x) > Mathf.Abs(y)) {
            if (x < -0.1f) {
                //Changes direction to left if not already facing left
                if (facing != DirectionFacing.Left)
                {
                    facing = DirectionFacing.Left;
                    UpdateDirection();
                }
                //If in quick mode, checks to see if the player can move left and then moves left
                if (!cautious && currentTile.GetLeft() && currentTile.GetLeft().IsWalkable() && !movedLeft && neutral && isInteractible) {
                    playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    currentTile = currentTile.GetLeft();
                    isInteractible = false;
                    movedLeft = true;
                    neutral = false;
                }
            } else if (x > 0.1f) {
                //Changes direction to right if not already facing right
                if (facing != DirectionFacing.Right)
                {
                    facing = DirectionFacing.Right;
                    UpdateDirection();
                }
                //If in quick mode, checks to see if the player can move right and then moves right
                if (!cautious && currentTile.GetRight() && currentTile.GetRight().IsWalkable() && !movedRight && neutral && isInteractible) {
                    playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    currentTile = currentTile.GetRight();
                    isInteractible = false;
                    movedRight = true;
                    neutral = false;
                }
            }
        }
        //Checks to see if the player pressed a vertical direction 
        else if (Mathf.Abs(x) < Mathf.Abs(y)) {
            if (y < -0.1f) {
                //Changes direction to right if not already facing down
                if (facing != DirectionFacing.Down)
                {
                    facing = DirectionFacing.Down;
                    UpdateDirection();
                }
                if (!cautious && currentTile.GetBottom() && currentTile.GetBottom().IsWalkable() && !movedDown && neutral && isInteractible) {
                    playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    currentTile = currentTile.GetBottom();
                    isInteractible = false;
                    movedDown = true;
                    neutral = false;
                }
            } else if (y > 0.1f) {
                //Changes direction to right if not already facing up
                if (facing != DirectionFacing.Up)
                {
                    facing = DirectionFacing.Up;
                    UpdateDirection();
                }
                //If in quick mode, checks to see if the player can move down and then moves up
                if (!cautious && currentTile.GetTop() && currentTile.GetTop().IsWalkable() && !movedUp && neutral && isInteractible) {
                    playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    currentTile = currentTile.GetTop();
                    isInteractible = false;
                    movedUp = true;
                    neutral = false;
                }
                //Checks to see if the player is in front of the elevator door and then starts the exit animation if the player has killed the target without being caught
                if (currentTile == exitTile && facing == DirectionFacing.Up && gameMan.GetWinCon() && !gameMan.GetLoseCon() && !cautious && isInteractible) {
                    //door animation start
                    isInteractible = false;
                    isExiting = true;

                } 
            }
        }
    }

    /*
     * Updates the animation of the player based on which direction the player is facing
     */
    public void UpdateDirection() {
        MusicScript.Instance.RotateSFX();
        animOffset = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
        anim.SetFloat("NormalizedTime", animOffset);
        switch(facing) {
            case DirectionFacing.Left:
                anim.SetInteger("Facing Direction", 3);
                break;
            case DirectionFacing.Right:
                anim.SetInteger("Facing Direction", 1);
                break;
            case DirectionFacing.Up:
                anim.SetInteger("Facing Direction", 0);
                break;
            case DirectionFacing.Down:
                anim.SetInteger("Facing Direction", 2);
                break;
        }
    }

    /*
     * Moves the player depending on if the control mode is wary and if the player has pressed the move button or not
     * @param ctx control input
     */
    public void Move(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        cautious = gameMan.GetControlStyle();
        Debug.Log("Move " + pressed);
        //Checks to see if the move button is pressed
        if (pressed > 0.5f && isInteractible && !movePressed) {
            movePressed = true;
            //Checks to see if the player is in front of the elevator door and then starts the exit animation if the player has killed the target without being caught
            if (currentTile == exitTile && facing == DirectionFacing.Up && gameMan.GetWinCon() && !gameMan.GetLoseCon()) {
                //door animation start
                isInteractible = false;
                isExiting = true;

            }
            //If the control style is wary, moves the player in the direction that the player is facing depending on if the tile in front of it is unoccupied
            if(cautious){
                switch(facing) {
                    case DirectionFacing.Left:
                        if (currentTile.GetLeft() && currentTile.GetLeft().IsWalkable()) {
                            playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                            currentTile = currentTile.GetLeft();
                            isInteractible = false;
                            //MusicScript.Instance.StepSFX();
                            
                        } else {
                            //Trigger bump sound
                        }
                        break;
                    case DirectionFacing.Right:
                        if (currentTile.GetRight() && currentTile.GetRight().IsWalkable()) {
                            playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                            currentTile = currentTile.GetRight();
                            isInteractible = false;
                            //MusicScript.Instance.StepSFX();
                            
                        } else {
                            //Trigger bump sound
                        }
                        break;
                    case DirectionFacing.Up:
                        if (currentTile.GetTop() && currentTile.GetTop().IsWalkable()) {
                            playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                            currentTile = currentTile.GetTop();
                            isInteractible = false;
                            //MusicScript.Instance.StepSFX();
                            
                        } else {
                            //Trigger bump sound
                        }
                        break;
                    case DirectionFacing.Down:
                        if (currentTile.GetBottom() && currentTile.GetBottom().IsWalkable()) {
                            playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                            currentTile = currentTile.GetBottom();
                            isInteractible = false;
                            //MusicScript.Instance.StepSFX();
                            
                        } else {
                            //Trigger bump sound
                        }
                        break;
                }
            }
        } else if (pressed <= 0.5f && movePressed) {
            movePressed = false;
        }
    }

    /*
     * Taps the person adjacent to the player if one exists, if it can be tapped, and if the person is not already facing the player
     * @param ctx control input
     */
    public void Tap(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        Debug.Log("Tap " + pressed);
        //Checks to see if the tap button is pressed
        if (pressed > 0.5f && isInteractible && !hasTapped) {
            hasTapped = true;
            //Checks to see if there is a person is in front of the player and taps it if one exists, if it can be tapped, and if it is not already facing the player
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Left)) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Right)) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Up)) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Down)) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
            //tileMan.AddLevelState();
        } else if(pressed <=.5) {
            hasTapped = false;
            adjacentPerson = null;
        }
        else {
            adjacentPerson = null;
        }
    }

    /*
     * Pushes the person adjacent to the player if one exists, if the person can be pushed, and if the spot the person is being pushed to is unoccupied
     * @param ctx control input
     */
    public void Push(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        Debug.Log("Push " + pressed);
        //Checks to see if the push button is pressed
        if (pressed > 0.5f && isInteractible && !hasPushed) {
            hasPushed = true;
            //Checks to see if there is a person is in front of the player and pushes it if one exists, if it can be pushed, and if the tile it is being pushed to is unoccupied
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Left)) {
                        MusicScript.Instance.PushSFX();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Right)) {
                        MusicScript.Instance.PushSFX();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Up)) {
                        MusicScript.Instance.PushSFX();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Down)) {
                        MusicScript.Instance.PushSFX();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
            
            //tileMan.AddLevelState();
        } else if (pressed <= 0.5f && hasPushed) {
            adjacentPerson = null;
            hasPushed = false;
        } else {
            adjacentPerson = null;
        }
    }

    /*
     * Kills the person adjacent to the player if one exists and if the person can be killed
     * @param ctx control input
     */
    public void Kill(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        Debug.Log("Kill " + pressed);
        //Checks to see if there is a person is in front of the player and kills it if one exists and if it can be killed
        if (pressed > 0.5f && isInteractible && !hasKilled) {
            hasKilled = true;
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.StabbyStabby();
                        targetDead = true;
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.StabbyStabby();
                        targetDead = true;
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.StabbyStabby();
                        targetDead = true;
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        playerStates.Push((currentTile, facing, gameMan.GetCurrentFloor()));
                        MusicScript.Instance.StabbyStabby();
                        targetDead = true;
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
        } else if (pressed <= .5) {
            hasKilled = false;
            adjacentPerson = null;
        }
        else{
            adjacentPerson = null;
        }
    }
    
    /*
     * Undoes the last action whether the player moved, tapped, or pushed
     * Cannot undo a kill
     * @param ctx control input
     */
    public void Undo(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused() || playerStates.Count == 0){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        //Checks to see if the undo button is pressed and can be pressed then finds the last action, direction, and adjacent person and reverts the state of the player and that adjacent person
        if (pressed > 0.5f && isInteractible && !undoPressed) {
            undoPressed = true;
            Tile lastTile = playerStates.Peek().tile;
            DirectionFacing lastDirection = playerStates.Peek().direction;
            int oldFloor = playerStates.Peek().floorNumber + 1;
            playerStates.Pop();
            Debug.Log("Last Tile: " + lastTile.getCoords() + ", Last Direction: " + lastDirection.ToString());
            if (facing != lastDirection) {
                facing = lastDirection;
                UpdateDirection();
            }
            if (currentTile.transform.position != lastTile.transform.position) {
                currentTile = lastTile;
                targetPosition = currentTile.transform.position;
                isInteractible = false;
                adjacentPerson = null;
            }
            for (int i = 0; i < personHolder.transform.childCount; i++)
            {
                if (personHolder.transform.GetChild(i).GetComponent<Person>().UndoState()) {
                    personHolder.UpdateMap();
                }
            }
            gameMan.UndoFloor(oldFloor);
            // //Undoes a movement option, tap action, or push action depending on which was last performed
            // switch(lastAction) {
            //     //Moves the player back to the right and adds two floors
            //     case Action.MoveLeft:
            //         currentTile = currentTile.GetRight();
            //         targetPosition = currentTile.transform.position;
            //         isInteractible = false;
            //         undoPressed = false;
            //         gameMan.UndoFloor(2);
            //         break;
            //     //Moves the player back to the left and adds two floors
            //     case Action.MoveRight:
            //         currentTile = currentTile.GetLeft();
            //         targetPosition = currentTile.transform.position;
            //         isInteractible = false;
            //         undoPressed = false;
            //         gameMan.UndoFloor(2);
            //         break;
            //     //Moves the player back to the up and adds two floors
            //     case Action.MoveUp:
            //         currentTile = currentTile.GetBottom();
            //         targetPosition = currentTile.transform.position;
            //         isInteractible = false;
            //         undoPressed = false;
            //         gameMan.UndoFloor(2);
            //         break;
            //     //Moves the player back to the down and adds two floors
            //     case Action.MoveDown:
            //         currentTile = currentTile.GetTop();
            //         targetPosition = currentTile.transform.position;
            //         isInteractible = false;
            //         undoPressed = false;
            //         gameMan.UndoFloor(2);
            //         break;
            //     //Reverts the adjacent person that was last tapped to its previous direction and adds two floors
            //     case Action.Tap:
            //         lastAdjacentPerson.UndoState();
            //         isInteractible = false;
            //         hasTapped = false;
            //         undoPressed = false;
            //         gameMan.UndoFloor(2);
            //         break;
            //     //Reverts the adjacent person that was last pushed to its previous location and adds three floors
            //     case Action.Push:
            //         lastAdjacentPerson.UndoState();
            //         isInteractible = false;
            //         hasPushed = false;
            //         undoPressed = false;
            //         gameMan.UndoFloor(3);
            //         break;
            //     default:
            //         //No more actions
            //         break;
            // }
            //tileMan.UndoLevelState();
        } else if (ctx.ReadValue<float>() <= 0.5f && undoPressed) {
            undoPressed = false;
        }
    }

    /*
     * Flips the control style when the toggle is clicked
     */
    public void UpdateControlStyle() {
        cautious = !cautious;
    }

    private bool pressedRestart = false;
    /*
     * Restarts the level depending on if the reset button is pressed or not
     * @param ctx control input
     */
    public void Restart(InputAction.CallbackContext ctx){
        float pressed = ctx.ReadValue<float>();
        if(pressed > .5 && !pressedRestart){
            pressedRestart = true;
            if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
            GameManager.Instance.GameOver();
        }
        else if(pressed <= .5){
            pressedRestart = false;
        }
    }
}