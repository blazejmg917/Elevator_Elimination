using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour
{
    private enum DirectionFacing
    {
        Left,
        Right,
        Up,
        Down
    }
    DirectionFacing facing;
    // Start is called before the first frame update
    void Start()
    {
        facing = DirectionFacing.Up;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Turn(InputAction.CallbackContext ctx) {
        //ctx.ReadValue<Vector2>()
    }

    public void Move() {
        
    }
}
