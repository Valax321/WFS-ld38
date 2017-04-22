using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Unit")]
public class Unit : ScriptableObject
{
    public enum UnitType
    {
        Land,
        Air,
        Sea
    }

    [Header("Unit Properties")]
    public string nameKey; //can use formatters e.g. {prefix} of {someother tag}
    public UnitType moveType;
    public int healthMin;
    public int healthMax;
    public int baseSpeed;

    [Header("Abilities")]
    public Ability ability1;
    public Ability ability2;
}
