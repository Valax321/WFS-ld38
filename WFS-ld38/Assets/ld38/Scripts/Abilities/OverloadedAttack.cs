using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class OverloadedAttack : AbilityBehaviour
{
    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);
        if (!owner.Damage(RandomDamage() / 2))
        {
            if (tile.occupyingUnit != null)
            {
                var other = tile.occupyingUnit;
                if (tile.occupyingUnit.Damage(RandomDamage()))
                {
                    owner.StealAbility(other);
                }
            }
        }
    }

    int RandomDamage()
    {
        return Random.Range(ourAbility.damageMin, ourAbility.damageMax);
    }
}
