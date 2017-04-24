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

    Material lavaMat;
    AudioClip travelSound;

    const int TURNS_TO_TRAVEL = 3;

    void Awake()
    {
        lavaMat = Resources.Load<Material>("Lava");
        travelSound = Resources.Load<AudioClip>("Time Travel/travel");
    }
    
    public override void OnEndOfTurn()
    {
        base.OnEndOfTurn();
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
        isTravelling = true;
        teleportedTile.occupyingUnit = null;
        Debug.Log("Hiding...");
        owner.activeObject.SetActive(false);
        travelledTurns = 0;
        owner.unitSound.PlayOneShot(travelSound);

        if (Random.Range(0f, 1f) > 0.995f)
        {
            Debug.Log("This traveller was lost in the time vortex.");
            owner.player.RemoveUnitsFromList(owner);
            Destroy(gameObject, 3f);
            return;
        }
    }

    void ComeBackAndMurder()
    {
        Debug.Log("TIME TRAVEL SURPRISE!");
        owner.activeObject.SetActive(true);

        if (teleportedTile.occupyingUnit != null)
        {
            owner.StealAbility(teleportedTile.occupyingUnit);
            teleportedTile.occupyingUnit.Killed();
        }

        teleportedTile.occupyingUnit = owner;

        if (lavaMat != null)
        {
            teleportedTile.GetComponent<MeshRenderer>().material = lavaMat;
        }
        else
        {
            Debug.LogError("Failed to find lava material!");
        }

        owner.unitSound.PlayOneShot(travelSound);
    }
}
