using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captial : UnitController
{
    protected override void BaseUpdate()
    {
        base.BaseUpdate();        
    }

    protected override void ChildUnitInit()
    {

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
    }
}
