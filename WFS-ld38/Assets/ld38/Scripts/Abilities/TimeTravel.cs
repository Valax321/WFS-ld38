using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class TimeTravel : AbilityBehaviour
{
    VoronoiTile teleportedTile;
    bool isTravelling;
    int travelledTurns;

    const int TURNS_TO_TRAVEL = 3;
    
    public override void OnEndOfTurn()
    {
        base.OnEndOfTurn();
        if (isTravelling)
        {
            teleportedTile.occupyingUnit = null;
            //Find a way to hide.
        }
    }

    public override void OnTurn()
    {
        base.OnTurn();
        if (isTravelling)
        {
            travelledTurns++;
            if (travelledTurns >= TURNS_TO_TRAVEL)
            {
                isTravelling = false;
            }
        }
    }

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);
        teleportedTile = owner.currentTile; //Store the current location.
        isTravelling = true;
    }

    void ComeBackAndMurder()
    {
        if (teleportedTile.occupyingUnit != null)
        {
            teleportedTile.occupyingUnit.Killed();
        }

        teleportedTile.occupyingUnit = owner;
    }
}
