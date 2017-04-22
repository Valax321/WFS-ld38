using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Ability")]
public class Ability : ScriptableObject
{
    public string nameKey; //can use formatters e.g. {prefix} of {someother tag}
    //public Sprite sprite;
    //public AudioClip sound;
    public int range;
    public bool isAOE;
    public bool isLastingDamage;
    public bool hasLastingEffects;
    public int damageMin;
    public int damageMax;
    public int currencyMin;
    public int currencyMax;
    public string abilityScript;

    public string GetAbilityName()
    {
        bool isKey = false;
        string name = "";
        foreach (char c in nameKey)
        {
            if (c == '{')
            {
                isKey = true;
                continue;
            }
        }
    }
}