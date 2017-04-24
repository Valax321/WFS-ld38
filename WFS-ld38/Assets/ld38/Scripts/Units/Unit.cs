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
        Sea,
        Captial,
        CurrencyGenerator
    }

    [Header("Unit Properties")]
    public string nameKey; //can use formatters e.g. {prefix} of {someother tag}
    public string description;
    public UnitType moveType;
    public int healthMin;
    public int healthMax;
    public int baseSpeed; //Actually moves BUT I CANT NAME THINGS PROPERLY
    public int currency;

    [Header("Unit Appearance")]
    public GameObject unitPrefab;
    public GameObject unitPrefabSpecial;
    public AudioClip[] moveSounds;
    public AudioClip[] dieSounds;
    public AudioClip[] spawnSounds;

    [Header("Abilities")]
    public bool hasAbilities;
    public AbilityPool abilityPool;
    public Ability ability1;
    public Ability ability2;

    private string aName;
    private bool hasSetName;
    public string unitName
    {
        get
        {
            if (!hasSetName)
            {
                aName = MakeAbilityName();
                hasSetName = true;
            }

            return aName;
        }
    }

    /// <summary>
    /// Warning: this will change every use, so save the result the first time!
    /// </summary>
    /// <returns></returns>
    public string MakeAbilityName()
    {
        bool isKey = false;
        string name = "";
        string key = "";
        foreach (char c in nameKey)
        {
            if (c == '{')
            {
                isKey = true;
                continue;
            }
            else if (c == '}')
            {
                var generated = NameGenerator.Instance.GetRandomName(key);
                key = "";
                name += generated;
                isKey = false;
                continue;
            }
            else
            {
                if (isKey)
                {
                    key += c;
                }
                else
                {
                    name += c;
                }
            }
        }

        return name;
    }
}
