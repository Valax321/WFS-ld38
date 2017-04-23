using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets;
using cakeslice;

[AddComponentMenu("Gameplay/Unit")]
public class UnitController : MonoBehaviour
{
    protected AbilityBehaviour aScript1;
    protected AbilityBehaviour aScript2;
    public Unit unitType;
    public Ability ability1;
    public Ability ability2;
    protected Outline outline;
    public bool shouldOutline;
    public int movesThisTurn;
    bool invalid;

    public int health;

    public VoronoiTile currentTile { get; set; }

    public Player player;

    void Awake()
    {
        transform.up = GetUpVector();
    }

    public void InitUnit()
    {
        if (unitType.hasAbilities)
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
            }
        }

        if (!invalid)
        {            
            var go = Instantiate(unitType.unitPrefab);
            go.transform.SetParent(transform, false);
            health = Random.Range(unitType.healthMin, unitType.healthMax + 1);

            if (unitType.moveType == Unit.UnitType.Captial || unitType.moveType == Unit.UnitType.CurrencyGenerator)
            {
                go.transform.Rotate(new Vector3(0, 0, Random.Range(0f, 359f)), Space.Self); //Give us a random rotation for variety.
            }

            outline = go.AddComponent<Outline>();
            outline.enabled = false;        
        }

        ChildUnitInit();        
    }

    protected virtual void ChildUnitInit()
    {
        
    }

    protected void GetSurroundingTiles(int depth)
    {
        if (currentTile != null)
        {
            //MAKE THIS WORK
        }
    }

    void Update()
    {
        BaseUpdate();
    }

    protected virtual void BaseUpdate()
    {
        if (currentTile.altitude > 0)
        {
            transform.position = currentTile.centerPoint + GetUpVector() * currentTile.altitude;
        }

        transform.up = GetUpVector();

        outline.enabled = shouldOutline;
    }

    public Vector3 GetUpVector()
    {
        return transform.position - Vector3.zero;
    }

    public void UseAbility(bool isAbility1)
    {
        if (unitType.hasAbilities)
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

    public virtual void Damage(int damage)
    {
        if (health - damage >= 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;
        }

        if (health <= 0)
        {
            Killed();
        }
    }

    protected virtual void Killed()
    {

    }
}
