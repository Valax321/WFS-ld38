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
            hasBuilt = true;
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
                Debug.Log("Building...");
                var go = new GameObject(spawnUnit.unitName, typeof(CurrencyGenerator));
                var gen = go.GetComponent<CurrencyGenerator>();
                go.transform.position = buildTile.centerPoint;
                gen.unitType = spawnUnit;
                gen.currentTile = buildTile;
                gen.player = owner.player;
                gen.InitUnit();
                buildTile.occupyingUnit = gen;
                owner.player.AddUnitToList(gen);
            }
            else
            {
                owner.moveSpeedMultiplier = 0;
            }
        }
    }
}
