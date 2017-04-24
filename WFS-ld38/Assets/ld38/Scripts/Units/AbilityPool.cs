using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/AbilityPool")]
public class AbilityPool : ScriptableObject {

    public List<Ability> abilities;
}
