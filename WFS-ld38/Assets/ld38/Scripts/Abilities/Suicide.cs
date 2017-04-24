using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class Suicide : AbilityBehaviour
{
    const int BLAST_RADIUS = 3;
    bool isDying;

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);
        ExplodeWithExtremePrejudice(true);
    }

    public override void OnDeath()
    {
        if (!isDying)
        {
            ExplodeWithExtremePrejudice(false);
        }
        base.OnDeath();        
    }

    void ExplodeWithExtremePrejudice(bool shouldDie)
    {
        var curTile = owner.currentTile;

        if (shouldDie)
        {
            isDying = true;
            owner.Killed();
        }

        var tiles = VoronoiTile.FindTilesInRange(curTile, BLAST_RADIUS);
        //bool killself = false;
        foreach (var tileList in tiles.ToArray())
        {
            foreach (var tile in tileList.ToArray())
            {
                if (tile.occupyingUnit != null && tile.occupyingUnit != owner)
                {
                    tile.occupyingUnit.Damage(Random.Range(ourAbility.damageMin, ourAbility.damageMax));
                }
            }
        }
    }
}