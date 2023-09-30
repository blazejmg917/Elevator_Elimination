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
    private DirectionFacing facing;
    [SerializeField] Tile currentTile;
    [SerializeField] float movementSpeed = 5f;
    private bool isInteractible = true;
    private Person adjacentPerson = null;
    // Start is called before the first frame update
    void Start()
    {
        facing = DirectionFacing.Down;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isInteractible) {
            transform.position = Vector3.MoveTowards(transform.position, currentTile.transform.position, movementSpeed * Time.fixedDeltaTime);
            if (transform.position == currentTile.transform.position) {
                isInteractible = true;
            }
        }
    }

    public void Turn(InputAction.CallbackContext ctx) {
        float x = ctx.ReadValue<Vector2>().x;
        float y = ctx.ReadValue<Vector2>().y;
        if (Mathf.Abs(x) > Mathf.Abs(y)) {
            if (x < 0) {
                facing = DirectionFacing.Left;
                //change sprite
            } else {
                facing = DirectionFacing.Right;
                //change sprite
            }
        } else if (Mathf.Abs(x) < Mathf.Abs(y)) {
            if (y < 0) {
                facing = DirectionFacing.Down;
                //change sprite
            } else {
                facing = DirectionFacing.Up;
                //change sprite
            }
        }
    }

    public void Move(InputAction.CallbackContext ctx) {
        float pressed = ctx.ReadValue<float>();
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    if (currentTile.GetLeft() != null /*&& currentTile.GetLeft().IsWalkable()*/) {
                        currentTile = currentTile.GetLeft();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Right:
                    if (currentTile.GetRight() != null /*&& currentTile.GetRight().IsWalkable()*/) {
                        currentTile = currentTile.GetRight();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Up:
                    if (currentTile.GetTop() != null /*&& currentTile.GetTop().IsWalkable()*/) {
                        currentTile = currentTile.GetTop();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
                case DirectionFacing.Down:
                    if (currentTile.GetBottom() != null /*&& currentTile.GetBottom().IsWalkable()*/) {
                        currentTile = currentTile.GetBottom();
                        isInteractible = false;
                        //Trigger step sound
                        
                    } else {
                        //Trigger bump sound
                    }
                    break;
            }
        }
    }

    public void Tap(InputAction.CallbackContext ctx) {
        float pressed = ctx.ReadValue<float>();
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    //adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() != null && adjacentPerson != null /*&& adjacentPerson.OnTap(DirectionFacing.Left)*/) {
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    //adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() != null && adjacentPerson != null /*&& adjacentPerson.OnTap(DirectionFacing.Right)*/) {
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    //adjacentPerson = currentTile.GetUp().GetPerson();
                    if (currentTile.GetTop() != null && adjacentPerson != null /*&& adjacentPerson.OnTap(DirectionFacing.Up)*/) {
                        //Trigger tap noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    //adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() != null && adjacentPerson != null /*&& adjacentPerson.OnTap(DirectionFacing.Bottom)*/) {
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
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    //adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() != null && adjacentPerson != null /*&& adjacentPerson.OnPush(DirectionFacing.Left)*/) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    //adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() != null && adjacentPerson != null /*&& adjacentPerson.OnPush(DirectionFacing.Right)*/) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    //adjacentPerson = currentTile.GetUp().GetPerson();
                    if (currentTile.GetTop() != null && adjacentPerson != null /*&& adjacentPerson.OnPush(DirectionFacing.Up)*/) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    //adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() != null && adjacentPerson != null /*&& adjacentPerson.OnPush(DirectionFacing.Bottom)*/) {
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
        if (pressed > 0.5f && isInteractible) {
            switch(facing) {
                case DirectionFacing.Left:
                    //adjacentPerson = currentTile.GetLeft().GetPerson();
                    if (currentTile.GetLeft() != null && adjacentPerson != null /*&& adjacentPerson.OnKill(DirectionFacing.Left)*/) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Right:
                    //adjacentPerson = currentTile.GetRight().GetPerson();
                    if (currentTile.GetRight() != null && adjacentPerson != null /*&& adjacentPerson.OnKill(DirectionFacing.Right)*/) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Up:
                    //adjacentPerson = currentTile.GetUp().GetPerson();
                    if (currentTile.GetTop() != null && adjacentPerson != null /*&& adjacentPerson.OnKill(DirectionFacing.Up)*/) {
                        //Trigger push noise
                    } else {
                        //Trigger error sound
                    }
                    break;
                case DirectionFacing.Down:
                    //adjacentPerson = currentTile.GetBottom().GetPerson();
                    if (currentTile.GetBottom() != null && adjacentPerson != null /*&& adjacentPerson.OnKill(DirectionFacing.Bottom)*/) {
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
}
