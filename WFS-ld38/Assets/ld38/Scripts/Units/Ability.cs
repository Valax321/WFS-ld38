using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Ability")]
public class Ability : ScriptableObject
{
    public string nameKey; //can use formatters e.g. {prefix} of {someother tag}
    public string description;
    //public Sprite sprite;
    //public AudioClip sound;
    public int range;
    public bool isAOE;
    public bool isLastingDamage;
    public bool usesTarget;
    public bool hasLastingEffects;
    public int damageMin;
    public int damageMax;
    public string abilityScript;

    [Header("Setting up the Unit")]
    public int currencyMin;
    public int currencyMax;
    public int healthMin;
    public int healthMax;
    public int movesCost;

    private string aName;
    private bool hasSetName;
    public string abilityName
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
    string MakeAbilityName()
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