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
        Building
    }

    [Header("Unit Properties")]
    public string nameKey; //can use formatters e.g. {prefix} of {someother tag}
    public UnitType moveType;
    public int healthMin;
    public int healthMax;
    public int baseSpeed;

    [Header("Unit Appearance")]
    public GameObject unitPrefab;

    [Header("Abilities")]
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
