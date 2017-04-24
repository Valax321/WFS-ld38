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

    public override void Damage(int damage)
    {
        base.Damage(damage);

        player.health = health;
    }

    public override void Killed()
    {
        base.Killed();
    }
}
