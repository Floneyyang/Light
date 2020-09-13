using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public Command handleInput()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            return new HideCommand();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            return new RevealCommand();
        }

        return null;
    }

    public Command handleMovement()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            Vector3 v = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            return new MoveCommand(v);
        }

        return null;
    }


}
