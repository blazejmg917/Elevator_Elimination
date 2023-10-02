using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private DirectionFacing facing = DirectionFacing.Down;
    private Tile currentTile;
    [SerializeField] private float movementSpeed = 5f;
    private bool isInteractible = false;
    private Person adjacentPerson = null;
    private Vector3 targetPosition;
    private TileManager tileMan;
    private GameManager gameMan;
    private bool movePressed = false;
    private Tile exitTile;
    private Vector3 exitPosition;
    private bool isExiting = false;
    private bool isStarting = true;
    private Vector2 controlDirection;
    private bool movedLeft = false;
    private bool movedRight = false;
    private bool movedUp = false;
    private bool movedDown = false;
    private bool cautious;
    private bool neutral = true;
    private Vector3 currentTilePos;
    [SerializeField] private Animation gameOverAnimation;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        tileMan = TileManager.Instance;
        gameMan = GameManager.Instance;
        currentTile = tileMan.GetStartTile();
        currentTilePos = currentTile.transform.position;
        exitTile = currentTile;
        WalkIn();
        exitPosition = transform.position;
        cautious = gameMan.GetControlStyle();
        anim = GetComponent<Animator>();
    }

    void WalkIn() {
        gameObject.SetActive(true);
        transform.position = new Vector3(currentTilePos.x, currentTilePos.y + 0.25f, transform.position.z);
        targetPosition = new Vector3(currentTilePos.x, currentTilePos.y, transform.position.z);
    }

    void WalkOut() {
        transform.position = Vector3.MoveTowards(transform.position, exitPosition, movementSpeed * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            targetPosition = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.fixedDeltaTime);
            if (transform.position == targetPosition) {
                isInteractible = true;
                tileMan.UpdateLevel();
            }
        }
    }

    public void Turn(InputAction.CallbackContext ctx) {
        float x = ctx.ReadValue<Vector2>().x;
        float y = ctx.ReadValue<Vector2>().y;
        cautious = gameMan.GetControlStyle();
        controlDirection = ctx.ReadValue<Vector2>();
        //Debug.Log("Turn " + x + ", " + y);
        if (Mathf.Abs(x) > Mathf.Abs(y)) {
            if (x < -0.1f) {
                facing = DirectionFacing.Left;
                if (!cautious && currentTile.GetLeft() && currentTile.GetLeft().IsWalkable() && !movedLeft && neutral && isInteractible) {
                    currentTile = currentTile.GetLeft();
                    isInteractible = false;
                    movedLeft = true;
                    neutral = false;
                }
            } else if (x > 0.1f) {
                facing = DirectionFacing.Right;
                if (!cautious && currentTile.GetRight() && currentTile.GetRight().IsWalkable() && !movedRight && neutral && isInteractible) {
                    currentTile = currentTile.GetRight();
                    isInteractible = false;
                    movedRight = true;
                    neutral = false;
                }
            }
            UpdateDirection();
        } else if (Mathf.Abs(x) < Mathf.Abs(y)) {
            if (y < -0.1f) {
                facing = DirectionFacing.Down;
                if (!cautious && currentTile.GetBottom() && currentTile.GetBottom().IsWalkable() && !movedDown && neutral && isInteractible) {
                    currentTile = currentTile.GetBottom();
                    isInteractible = false;
                    movedDown = true;
                    neutral = false;
                }
            } else if (y > 0.1f) {
                facing = DirectionFacing.Up;
                if (!cautious && currentTile.GetTop() && currentTile.GetTop().IsWalkable() && !movedUp && neutral && isInteractible) {
                    currentTile = currentTile.GetTop();
                    isInteractible = false;
                    movedUp = true;
                    neutral = false;
                }
            }
            UpdateDirection();
        }
    }
    
    public void UpdateDirection() {
        switch(facing) {
            case DirectionFacing.Left:
                // if (transform.localScale.x < 0) {
                //     transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                // }
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetInteger("Facing Direction", 3);
                break;
            case DirectionFacing.Right:
                // if (transform.localScale.x > 0) {
                //     transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                // }
                GetComponent<SpriteRenderer>().flipX = true;
                anim.SetInteger("Facing Direction", 1);
                break;
            case DirectionFacing.Up:
                // if (transform.localScale.x < 0) {
                //     transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                // }
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetInteger("Facing Direction", 0);
                break;
            case DirectionFacing.Down:
                // if (transform.localScale.x < 0) {
                //     transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                // }
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetInteger("Facing Direction", 2);
                break;
        }
    }

    public void Move(InputAction.CallbackContext ctx) {
        float pressed = ctx.ReadValue<float>();
        cautious = gameMan.GetControlStyle();
        //Debug.Log("Move " + pressed);
        if (pressed > 0.5f && isInteractible && !movePressed && cautious) {
            movePressed = true;
            if (currentTile == exitTile && facing == DirectionFacing.Up && gameMan.GetWinCon() && !gameMan.GetLoseCon()) {
                //door animation start
                isInteractible = false;
                isExiting = true;
            } 
            switch(facing) {
                case DirectionFacing.Left:
                    if (currentTile.GetLeft() && currentTile.GetLeft().IsWalkable()) {
                        currentTile = currentTile.GetLeft();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Right:
                    if (currentTile.GetRight() && currentTile.GetRight().IsWalkable()) {
                        currentTile = currentTile.GetRight();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Up:
                    if (currentTile.GetTop() && currentTile.GetTop().IsWalkable()) {
                        currentTile = currentTile.GetTop();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Down:
                    if (currentTile.GetBottom() && currentTile.GetBottom().IsWalkable()) {
                        currentTile = currentTile.GetBottom();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
            }
        } else if (pressed <= 0.5f && movePressed) {
            movePressed = false;
        }
    }

    public void Tap(InputAction.CallbackContext ctx) {
        float pressed = ctx.ReadValue<float>();
        //Debug.Log("Tap " + pressed);
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Left)) {
                        tileMan.UpdateLevel();
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Right)) {
                        tileMan.UpdateLevel();
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Up)) {
                        tileMan.UpdateLevel();
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnTap(DirectionFacing.Down)) {
                        tileMan.UpdateLevel();
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
        } else {
            adjacentPerson = null;
        }
    }

    public void Push(InputAction.CallbackContext ctx) {
        float pressed = ctx.ReadValue<float>();
        //Debug.Log("Push " + pressed);
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Left)) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Right)) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Up)) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnPush(DirectionFacing.Down)) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
        } else {
            adjacentPerson = null;
        }
    }

    public void Kill(InputAction.CallbackContext ctx) {
        float pressed = ctx.ReadValue<float>();
        Debug.Log("Kill " + pressed);
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    adjacentPerson = currentTile.GetTop().GetPerson();
                    if (currentTile.GetTop() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() && adjacentPerson && adjacentPerson.OnKill()) {
                        tileMan.UpdateLevel();
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
            }
        } else {
            adjacentPerson = null;
        }
    }
    public void UpdateControlStyle() {
        cautious = !cautious;
    }
}
