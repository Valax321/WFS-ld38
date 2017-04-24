using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class RangedAttack : AbilityBehaviour
{
    void Awake()
    {
        //Load effects
    }

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);

        if (tile.occupyingUnit != null)
        {
            var other = tile.occupyingUnit;
            if (other.Damage(Random.Range(ourAbility.damageMin, ourAbility.damageMax)))
            {
                owner.StealAbility(other);
            }
        }
    }
}
