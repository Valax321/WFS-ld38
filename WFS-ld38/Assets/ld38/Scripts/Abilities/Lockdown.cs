using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class Lockdown : AbilityBehaviour
{
    void Awake()
    {
        //Load effects
    }

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);

        owner.moveSpeedMultiplier = 0;
        if (tile.occupyingUnit != null)
        {
            tile.occupyingUnit.moveSpeedMultiplier = 0;
        }
    }
}
