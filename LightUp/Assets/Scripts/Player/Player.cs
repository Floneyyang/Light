using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    [Header("Class References")]
    public InputHandler input;
    public PlayerHealth healthIndicator;

    [Header("Player Settings")]
    public float normalSpeed;
    public float hideSpeed;

    [Header("Object References")]
    public GameObject light;
    public MeshRenderer renderer;

    
    private void Update()
    {
        Command command = input.handleInput();
        if (command != null && command.GetType() != typeof(MoveCommand))
        {
            command.executePlayer(this);
        }

        healthIndicator.updateHealth(this);
    }


    private void FixedUpdate()
    {
        Command commandPhysics = input.handleMovement();
        if (commandPhysics != null && commandPhysics.GetType() == typeof(MoveCommand))
        {
            commandPhysics.execute(this);
        }


    }
}
