using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class TimeTravel : AbilityBehaviour
{
    VoronoiTile teleportedTile;
    bool isTravelling;
    bool travellingThisTurn;
    int travelledTurns;

    const int TURNS_TO_TRAVEL = 3;
    
    public override void OnEndOfTurn()
    {
        base.OnEndOfTurn();
        if (travellingThisTurn)
        {
            teleportedTile.occupyingUnit = null;
            Debug.Log("Hiding...");
            isTravelling = true;
            //Find a way to hide.
        }
    }

    public override void OnTurn()
    {
        base.OnTurn();
        travellingThisTurn = false;
        if (isTravelling)
        {
            travelledTurns++;
            if (travelledTurns >= TURNS_TO_TRAVEL)
            {
                isTravelling = false;
                ComeBackAndMurder();
            }
        }
    }

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);
        teleportedTile = owner.currentTile; //Store the current location.
        travellingThisTurn = true;
    }

    void ComeBackAndMurder()
    {
        Debug.Log("TIME TRAVEL SURPRISE!");
        if (teleportedTile.occupyingUnit != null)
        {
            teleportedTile.occupyingUnit.Killed();
        }

        teleportedTile.occupyingUnit = owner;
    }
}
