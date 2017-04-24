using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class Heal : AbilityBehaviour
{
    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);
        if (tile.occupyingUnit != null)
        {
            tile.occupyingUnit.Damage(-Random.Range(ourAbility.damageMin, ourAbility.damageMax));
        }
    }
}
