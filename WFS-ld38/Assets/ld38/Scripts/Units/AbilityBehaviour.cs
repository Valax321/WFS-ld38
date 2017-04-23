using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets;

[AddComponentMenu("Gameplay/Unit Behaviour")]
public class AbilityBehaviour : MonoBehaviour
{
    public UnitController owner { get { return GetComponent<UnitController>(); } }
    public Ability ourAbility;

    public virtual void Activate(VoronoiTile tile)
    {
        Debug.LogFormat("{0} activated!", ourAbility.abilityName);
    }

    public virtual void OnTurn()
    {

    }

    public virtual void OnMove(VoronoiTile oldTile)
    {

    }
}
