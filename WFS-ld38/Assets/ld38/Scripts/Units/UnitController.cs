using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using Assets;
using cakeslice;

[AddComponentMenu("Gameplay/Unit")]
public class UnitController : MonoBehaviour
{
    static float UnitMoveSpeed = 3.0f;

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

    Vector3 forward;

    AudioSource unitSound;

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

            unitSound = gameObject.AddComponent<AudioSource>();
            unitSound.PlayOneShot(RandomSound(unitType.spawnSounds));
            outline = go.AddComponent<Outline>();
            outline.enabled = false;        
        }

        forward = Vector3.ProjectOnPlane(transform.forward, GetUpVector());
        ChildUnitInit();
        StartOfTurn(); 
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
        transform.position = Vector3.MoveTowards(transform.position, currentTile.altitude > 0 ? currentTile.centerPoint + GetUpVector() * currentTile.altitude : currentTile.centerPoint, UnitMoveSpeed * Time.deltaTime);

        //if (currentTile.altitude > 0)
        //{
        //    transform.position = currentTile.centerPoint + GetUpVector() * currentTile.altitude;
        //}

        transform.rotation = Quaternion.LookRotation(forward, GetUpVector());    

        outline.enabled = shouldOutline;
    }

    public virtual void StartOfTurn()
    {
        movesThisTurn = unitType.baseSpeed;
        if (unitType.hasAbilities)
        {
            aScript1.OnTurn();
            aScript2.OnTurn();
        }
    }

    public virtual void EndOfTurn()
    {
        if (unitType.hasAbilities)
        {
            aScript1.OnEndOfTurn();
            aScript2.OnEndOfTurn();
        }
    }

    public Vector3 GetUpVector()
    {
        return transform.position - Vector3.zero;
    }

    public Ability GetAbility(int num)
    {
        if (unitType.hasAbilities)
        {
            if (num == 1) return ability1;
            else if (num == 2) return ability2;
            else throw new System.Exception(string.Format("Invalid ability number: {0}", num));
        }
        else throw new System.Exception("The unit type does not have abilities!");
    }

    public void MoveToTile(VoronoiTile tile)
    {
        forward = currentTile.centerPoint - tile.centerPoint;
        if (CanMakeMove(1) && (tile.baseBiome != VoronoiTile.Biomes.Water || unitType.moveType != Unit.UnitType.Land))
        {            
            currentTile.occupyingUnit = null;
            var old = currentTile;
            currentTile = tile;            
            tile.occupyingUnit = this;            
            MakeMove(1);
            unitSound.PlayOneShot(RandomSound(unitType.moveSounds));

            if (unitType.hasAbilities)
            {
                aScript1.OnMove(old);
                aScript2.OnMove(old);
            }
        }
        else
        {
            //Complain
        }
    }

    public void UseAbility(bool isAbility1, VoronoiTile tile)
    {
        if (unitType.hasAbilities)
        {
            if (isAbility1)
            {
                if (CanMakeMove(ability1.movesCost))
                {
                    aScript1.Activate(tile);
                    MakeMove(ability1.movesCost);
                }
            }
            else
            {
                if (CanMakeMove(ability2.movesCost))
                {
                    aScript2.Activate(tile);
                    MakeMove(ability2.movesCost);
                }
            }
        }
    }

    void MakeMove(int cost)
    {
        movesThisTurn = Mathf.Clamp(movesThisTurn - cost, 0, unitType.baseSpeed);
    }

    public bool CanMakeMove(int cost)
    {
        return movesThisTurn - cost > 0;
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
        player.RemoveUnitsFromList(this); //We've been destroyed!
    }

    AudioClip RandomSound(AudioClip[] clips)
    {
        if (clips.Length < 1)
            return null;
        else
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}
