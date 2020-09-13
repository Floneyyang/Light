using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    ~Command() { }
    public virtual void execute(Actor actor) { }
    public virtual void executePlayer(Player actor) { }

}

public class MoveCommand : Command
{
    public MoveCommand(Vector3 v)
    {
        Velocity = v;
    }


    public override void execute(Actor actor) { Move(actor); }

    Actor _actor;
    private Vector3 Velocity;

    private void Move(Actor actor)
    {
        _actor = actor;
        Velocity *= actor.speed;
        _actor.ActorRd.MovePosition(_actor.ActorRd.position + Velocity * Time.fixedDeltaTime);
    }

}

public class HideCommand : Command
{
    public override void executePlayer(Player actor) { Hide(actor); }

    private void Hide(Player actor)
    {
        actor.speed = actor.hideSpeed;
        actor.light.GetComponent<Light>().intensity = 1.5f;
        actor.gameObject.tag = "Hidden";
        actor.renderer.enabled = true; 
    }
}

public class RevealCommand : Command
{
    public override void executePlayer(Player actor) { Reveal(actor); }

    private void Reveal(Player actor)
    {
        actor.speed = actor.normalSpeed;
        actor.light.GetComponent<Light>().intensity = 3.54f;
        actor.gameObject.tag = "Player";
        actor.renderer.enabled = false;
    }
}


