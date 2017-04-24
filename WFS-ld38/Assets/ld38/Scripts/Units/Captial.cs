using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captial : UnitController
{
    protected override void ChildUnitInit()
    {
        Debug.LogFormat("Setting player HP to {0}", health);
        player.health = health;
    }

    public override bool Damage(int damage)
    {
        bool killed = base.Damage(damage);
        player.health = health;
        return killed;
    }

    public override void Killed()
    {
        base.Killed();
        //Set a game over.
    }
}
