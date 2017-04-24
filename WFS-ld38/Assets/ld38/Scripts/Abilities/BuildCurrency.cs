using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class BuildCurrency : AbilityBehaviour
{
    const int TURNS_TO_BUILD = 1;
    int buildTurns;
    VoronoiTile buildTile;

    bool hasBuilt;
    Unit spawnUnit;

    void Awake()
    {
        spawnUnit = Resources.Load<Unit>("Units/Currency Mine");
    }

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);
        
        if (tile.occupyingUnit == null)
        {
            owner.moveSpeedMultiplier = 0;
            tile.occupyingUnit = owner;
            buildTile = tile;
        }
    }

    public override void OnTurn()
    {
        base.OnTurn();
        if (hasBuilt)
        {
            buildTurns++;
            if (buildTurns > TURNS_TO_BUILD)
            {
                hasBuilt = false;
                buildTurns = 0;
                //Build it!

                var go = new GameObject("Generated Mine", typeof(CurrencyGenerator));
                var gen = go.GetComponent<CurrencyGenerator>();

            }
            else
            {
                owner.moveSpeedMultiplier = 0;
            }
        }
    }
}
