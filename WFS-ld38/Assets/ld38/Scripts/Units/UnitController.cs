﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets;

[AddComponentMenu("Gameplay/Unit")]
public class UnitController : MonoBehaviour
{
    AbilityBehaviour aScript1;
    AbilityBehaviour aScript2;
    public Unit unitType;
    public Ability ability1;
    public Ability ability2;
    public int movesThisTurn;
    bool invalid;

    public int health;

    public VoronoiTile currentTile { get; private set; }

    void Awake()
    {
        ability1 = unitType.ability1;
        ability2 = unitType.ability2;

        try
        {
            aScript1 = gameObject.AddComponent(System.Type.GetType(ability1.abilityScript)) as AbilityBehaviour;
            aScript2 = gameObject.AddComponent(System.Type.GetType(ability2.abilityScript)) as AbilityBehaviour;
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
            var go = Instantiate(unitType.unitPrefab);
            go.transform.SetParent(transform, false);

            health = Random.Range(unitType.healthMin, unitType.healthMax + 1);
        }        
    }

    void GetSurroundingTiles(int depth)
    {
        if (currentTile != null)
        {
            //MAKE THIS WORK
        }
    }

    void Update()
    {
        transform.up = transform.position - Vector3.zero;
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
