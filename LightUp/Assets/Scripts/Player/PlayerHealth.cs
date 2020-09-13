using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public MeshRenderer healthRenderer;
    public Material[] playerHealth_mat = new Material[6];

    private int currentHealth;

    public override void updateHealth(Actor actor)
    {
        if(currentHealth != (int)actor.health)
        {
            healthRenderer.material = playerHealth_mat[(int)actor.health];
            currentHealth = (int)actor.health;
        }
    }
}
