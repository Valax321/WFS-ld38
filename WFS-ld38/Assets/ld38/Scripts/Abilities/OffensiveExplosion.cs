using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class OffensiveExplosion : AbilityBehaviour
{
    public override void Activate(VoronoiTile tile)
    {
        //base.Activate(tile);
        Debug.LogFormat("Using {0} on {1}", ourAbility.abilityName, tile.occupyingUnit == null ? "nothing" : tile.occupyingUnit.unitType.unitName);

        if (tile != null)
        {
            if (tile.occupyingUnit != null)
            {
                var enemy = tile.occupyingUnit;
                enemy.Damage(Random.Range(ourAbility.damageMin, ourAbility.damageMax));
            }
        }
    }
}
