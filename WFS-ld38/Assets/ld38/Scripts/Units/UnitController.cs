using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets;

[AddComponentMenu("Gameplay/Unit")]
public class UnitController : MonoBehaviour
{
    AbilityBehaviour aScript1;
    AbilityBehaviour aScript2;
    public Ability ability1;
    public Ability ability2;
    public int movesThisTurn;
    bool invalid;

    public VoronoiTile currentTile { get; private set; }

    void Awake()
    {
        try
        {
            aScript1 = gameObject.AddComponent(Type.GetType(ability1.abilityScript)) as AbilityBehaviour;
            aScript2 = gameObject.AddComponent(Type.GetType(ability2.abilityScript)) as AbilityBehaviour;
        }
        catch
        {
            Debug.LogError("Could not add ability scripts! Check your ability class names!");
            invalid = true;
        }

        if (!invalid)
        {
            aScript1.ourAbility = ability1;
            aScript2.ourAbility = ability2;
        }
    }

    public void UseAbility(bool isAbility1)
    {
        if (isAbility1)
        {
            aScript1.Activate();
        }
        else
        {
            aScript2.Activate();
        }
    }
}
