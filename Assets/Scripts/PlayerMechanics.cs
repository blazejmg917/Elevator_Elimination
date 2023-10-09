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
    public enum Action
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        Tap,
        Push,
        Kill,
    }
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
    private float animOffset;
    [SerializeField, Tooltip("Number of frames offset to start the player's idle animation")] private float initialOffset = 4f;
    private Stack<Action> actions;
    private Stack<DirectionFacing> directions;
    private bool undoPressed = false;
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
        actions = new Stack<Action>();
        directions = new Stack<DirectionFacing>();
        
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
        hasKilled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(waitingForLevel || gameMan.GetLoseCon()){
            return;
        }
        if(entering){
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, enterExitSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                Debug.Log("player entered");
                entering = false;
                levelStart.Invoke();
                isInteractible = true;
                exitPosition = transform.position;
                LevelManager.Instance.EnablePause(true);
            }
            return;
        }
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
        if (gameMan.GetLoseCon()) {
            //Remove return, set to go to game over screen and wait until player clicks on a button to reset lose con
            return;
            //gameMan.SetLoseCon(false);
            //gameOverAnimation.Play(gameOverAnimation);
        }
        if (!cautious && Mathf.Abs(controlDirection.x) < 0.1f && Mathf.Abs(controlDirection.y) < 0.1f) {
            movedLeft = false;
            movedRight = false;
            movedUp = false;
            movedDown = false;
            neutral = true;
        }
        if (isStarting) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                isStarting = false;
                isInteractible = true;
            }
        }
        if (!isInteractible && gameMan.GetWinCon() && isExiting && !gameMan.GetLoseCon() && !isStarting) {
            WalkOut();
            if (transform.position == exitPosition) {
                gameMan.SetWinCon(false);
                //load next level along with animations
            }
        }
        if (!isInteractible && !isExiting && !gameMan.GetLoseCon() && !isStarting) {
            targetPosition = currentTile.GetPersonLocation();//new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                isInteractible = true;
                undoPressed = false;
                MusicScript.Instance.StepSFX();
                if (!tileMan.UpdateLevel()){
                    GameManager.Instance.GameOver("SEEN");
                }
            }
        }
        //UpdateDirection();
    }

    public void Turn(InputAction.CallbackContext ctx) {
        //anim.speed = 10000;
        if(gameMan.GetLoseCon() || waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float x = ctx.ReadValue<Vector2>().x;
        float y = ctx.ReadValue<Vector2>().y;
        cautious = gameMan.GetControlStyle();
        controlDirection = ctx.ReadValue<Vector2>();
        //Debug.Log("Turn " + x + ", " + y);
        if (Mathf.Abs(x) > Mathf.Abs(y)) {
            if (x < -0.1f) {
                if (facing != DirectionFacing.Left)
                {
                    animOffset = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
                    facing = DirectionFacing.Left;
                    UpdateDirection();
                }
                if (!cautious && currentTile.GetLeft() && currentTile.GetLeft().IsWalkable() && !movedLeft && neutral && isInteractible) {
                    currentTile = currentTile.GetLeft();
                    isInteractible = false;
                    movedLeft = true;
                    neutral = false;
                    actions.Push(Action.MoveLeft);
                    directions.Push(facing);
                }
            } else if (x > 0.1f) {
                if (facing != DirectionFacing.Right)
                {
                    animOffset = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
                    facing = DirectionFacing.Right;
                    UpdateDirection();
                }
                if (!cautious && currentTile.GetRight() && currentTile.GetRight().IsWalkable() && !movedRight && neutral && isInteractible) {
                    currentTile = currentTile.GetRight();
                    isInteractible = false;
                    movedRight = true;
                    neutral = false;
                    actions.Push(Action.MoveRight);
                    directions.Push(facing);
                }
            }
        } else if (Mathf.Abs(x) < Mathf.Abs(y)) {
            if (y < -0.1f) {
                if (facing != DirectionFacing.Down)
                {
                    animOffset = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
                    facing = DirectionFacing.Down;
                    UpdateDirection();
                }
                if (!cautious && currentTile.GetBottom() && currentTile.GetBottom().IsWalkable() && !movedDown && neutral && isInteractible) {
                    currentTile = currentTile.GetBottom();
                    isInteractible = false;
                    movedDown = true;
                    neutral = false;
                    actions.Push(Action.MoveDown);
                    directions.Push(facing);
                }
            } else if (y > 0.1f) {
                if (facing != DirectionFacing.Up)
                {
                    animOffset = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
                    facing = DirectionFacing.Up;
                    UpdateDirection();
                }
                if (!cautious && currentTile.GetTop() && currentTile.GetTop().IsWalkable() && !movedUp && neutral && isInteractible) {
                    currentTile = currentTile.GetTop();
                    isInteractible = false;
                    movedUp = true;
                    neutral = false;
                    actions.Push(Action.MoveUp);
                    directions.Push(facing);
                }
                if (currentTile == exitTile && facing == DirectionFacing.Up && gameMan.GetWinCon() && !gameMan.GetLoseCon() && !cautious && isInteractible) {
                    //door animation start
                    isInteractible = false;
                    isExiting = true;

                } 
            }
        }
    }
    
    public void UpdateDirection() {
        //sets the anim speed back to one (was super high to get through end of last anim)
        //anim.speed = 1;
        MusicScript.Instance.RotateSFX();
        //Debug.Log(animOffset);
        anim.SetFloat("NormalizedTime", animOffset);
        switch(facing) {
            case DirectionFacing.Left:
                anim.SetInteger("Facing Direction", 3);
                //anim.speed = 1;
                break;
            case DirectionFacing.Right:
                anim.SetInteger("Facing Direction", 1);
                //anim.speed = 1;
                break;
            case DirectionFacing.Up:
                anim.SetInteger("Facing Direction", 0);
                //anim.speed = 1;
                break;
            case DirectionFacing.Down:
                anim.SetInteger("Facing Direction", 2);
                //anim.speed = 1;
                break;
        }
    }

    public void Move(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        cautious = gameMan.GetControlStyle();
        Debug.Log("Move " + pressed);
        if (pressed > 0.5f && isInteractible && !movePressed) {
            movePressed = true;
            if (currentTile == exitTile && facing == DirectionFacing.Up && gameMan.GetWinCon() && !gameMan.GetLoseCon()) {
                //door animation start
                isInteractible = false;
                isExiting = true;

            } 
            if(cautious){
            switch(facing) {
                case DirectionFacing.Left:
                    if (currentTile.GetLeft() && currentTile.GetLeft().IsWalkable()) {
                        currentTile = currentTile.GetLeft();
                        isInteractible = false;
                        //MusicScript.Instance.StepSFX();
                        actions.Push(Action.MoveLeft);
                        directions.Push(facing);
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Right:
                    if (currentTile.GetRight() && currentTile.GetRight().IsWalkable()) {
                        currentTile = currentTile.GetRight();
                        isInteractible = false;
                        //MusicScript.Instance.StepSFX();
                        actions.Push(Action.MoveRight);
                        directions.Push(facing);
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Up:
                    if (currentTile.GetTop() && currentTile.GetTop().IsWalkable()) {
                        currentTile = currentTile.GetTop();
                        isInteractible = false;
                        //MusicScript.Instance.StepSFX();
                        actions.Push(Action.MoveUp);
                        directions.Push(facing);
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Down:
                    if (currentTile.GetBottom() && currentTile.GetBottom().IsWalkable()) {
                        currentTile = currentTile.GetBottom();
                        isInteractible = false;
                        //MusicScript.Instance.StepSFX();
                        actions.Push(Action.MoveDown);
                        directions.Push(facing);
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
            }}
        } else if (pressed <= 0.5f && movePressed) {
            movePressed = false;
        }
    }

    public void Tap(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        //Debug.Log("Tap " + pressed);
        if (pressed > 0.5f && isInteractible && !hasTapped) {
            hasTapped = true;
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Left)) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Right)) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Up)) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Down)) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.TapSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
            actions.Push(Action.Tap);
            directions.Push(facing);
            //tileMan.AddLevelState();
        } else if(pressed <=.5) {
            hasTapped = false;
            adjacentPerson = null;
        }
        else {
            adjacentPerson = null;
        }
    }

    public void Push(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        //Debug.Log("Push " + pressed);
        if (pressed > 0.5f && isInteractible && !hasPushed) {
            hasPushed = true;
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Left)) {
                        MusicScript.Instance.PushSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Right)) {
                        MusicScript.Instance.PushSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Up)) {
                        MusicScript.Instance.PushSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Down)) {
                        MusicScript.Instance.PushSFX();
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
            actions.Push(Action.Push);
            directions.Push(facing);
            
            //tileMan.AddLevelState();
        } else if (pressed <= 0.5f && hasPushed) {
            adjacentPerson = null;
            hasPushed = false;
        } else {
            adjacentPerson = null;
        }
    }

    public void Kill(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused()){
            return;
        }
        float pressed = ctx.ReadValue<float>();
        Debug.Log("Kill " + pressed);
        if (pressed > 0.5f && isInteractible && !hasKilled) {
            hasKilled = true;
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        
                        MusicScript.Instance.StabbyStabby();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.StabbyStabby();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.StabbyStabby();
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        MusicScript.Instance.StabbyStabby();
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
    public void Undo(InputAction.CallbackContext ctx) {
        if(gameMan.GetLoseCon()|| waitingForLevel || LevelManager.Instance.IsPaused() || actions.Count == 0 || hasKilled){
            return;
        }
        Debug.Log("Undo");
        Debug.Log(actions.Count);
        if (ctx.ReadValue<float>() > 0.5f && isInteractible && !movePressed) {
            movePressed = true;
            Action lastAction = actions.Pop();
            DirectionFacing lastDirection = directions.Pop();
            Debug.Log(lastAction.ToString() + ", " + lastDirection.ToString());
            facing = lastDirection;
            UpdateDirection();

            switch(lastAction) {
                case Action.MoveLeft:
                    currentTile = currentTile.GetRight();
                    targetPosition = currentTile.transform.position;
                    isInteractible = false;
                    movePressed = false;

                    break;
                case Action.MoveRight:
                    currentTile = currentTile.GetLeft();
                    targetPosition = currentTile.transform.position;
                    isInteractible = false;
                    movePressed = false;

                    break;
                case Action.MoveUp:
                    currentTile = currentTile.GetBottom();
                    targetPosition = currentTile.transform.position;
                    isInteractible = false;
                    movePressed = false;

                    break;
                case Action.MoveDown:
                    currentTile = currentTile.GetTop();
                    targetPosition = currentTile.transform.position;
                    isInteractible = false;
                    movePressed = false;
                    
                    break;
                case Action.Tap:
                    isInteractible = false;
                    hasTapped = false;
                    if (GetAdjacentPerson()) {
                        GetAdjacentPerson().UndoTap();
                    }
                    break;
                case Action.Push:
                    isInteractible = false;
                    hasPushed = false;
                    Debug.Log("hi");
                    if (GetAdjacentPerson()) {
                        GetAdjacentPerson().UndoPush();
                    }
                    break;
                case Action.Kill:
                    break;
                default:
                    //No more actions
                    break;
            }
            //tileMan.UndoLevelState();
            gameMan.UndoFloor();
        } else if (ctx.ReadValue<float>() <= 0.5f && undoPressed) {
            undoPressed = false;
        }
    }

    public Person GetAdjacentPerson() {
        Person per = null;
        switch(facing) {
            case DirectionFacing.Left:
                per = currentTile.GetLeft().GetPerson();
                break;
            case DirectionFacing.Right:
                per = currentTile.GetRight().GetPerson();
                break;
            case DirectionFacing.Up:
                per = currentTile.GetTop().GetPerson();
                break;
            case DirectionFacing.Down:
                per = currentTile.GetBottom().GetPerson();
                break;
        }
        return per;
    }
    public void UpdateControlStyle() {
        cautious = !cautious;
    }

    private bool pressedRestart = false;
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
