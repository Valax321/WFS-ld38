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
    protected Outline outlineSpecial;
    public bool shouldOutline;
    public int movesThisTurn;
    bool invalid;
    public AbilityPool abilityPool;

    public GameObject visibleObject;
    public GameObject visibleSpecial;

    public int speed;
    public int currency;

    public int health;
    public int maxHealth;
    public bool isDead;

    public int moveSpeedMultiplier = 1;

    public VoronoiTile currentTile { get; set; }

    public Player player;

    Vector3 forward;

    public AudioSource unitSound;

    Outline activeOutline
    {
        get
        {
            if (outlineSpecial != null && unitType.moveType == Unit.UnitType.Sea && currentTile.baseBiome == VoronoiTile.Biomes.Water)
            {
                return outlineSpecial;
            }
            else
            {
                return outline;
            }
        }
    }

    public GameObject activeObject
    {
        get
        {
            if (visibleSpecial != null && unitType.moveType == Unit.UnitType.Sea && currentTile.baseBiome == VoronoiTile.Biomes.Water)
            {
                return visibleSpecial;
            }
            else
            {
                return visibleObject;
            }
        }
    }

    void Awake()
    {
        transform.up = GetUpVector();
    }

    public void InitUnit()
    {
        if (unitType.hasAbilities)
        {
            if (unitType.nameKey != "Resource Man")
            {
                abilityPool = unitType.abilityPool;
                ability1 = abilityPool.abilities[Random.Range(0, abilityPool.abilities.Count)];
                do
                {
                    ability2 = abilityPool.abilities[Random.Range(0, abilityPool.abilities.Count)];
                }
                while (ability2 == ability1);
            }
            else
            {
                ability1 = unitType.ability1;
                ability2 = unitType.ability2;
            }

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
            visibleObject = Instantiate(unitType.unitPrefab);
            visibleObject.transform.SetParent(transform, false);

            if (unitType.unitPrefabSpecial != null)
            {
                visibleSpecial = Instantiate(unitType.unitPrefabSpecial);
                visibleSpecial.transform.SetParent(transform, false);
            }

            if (unitType.hasAbilities)
            {
                GenerateStats();
            }
            else
            {
                speed = unitType.baseSpeed;
                health = Random.Range(unitType.healthMin, unitType.healthMax + 1);
                maxHealth = health;
            }

            if (unitType.moveType == Unit.UnitType.Captial || unitType.moveType == Unit.UnitType.CurrencyGenerator)
            {
                visibleObject.transform.Rotate(new Vector3(0, 0, Random.Range(0f, 359f)), Space.Self); //Give us a random rotation for variety.
                if (visibleSpecial != null)
                {
                    visibleSpecial.transform.rotation = visibleObject.transform.rotation;
                }
            }

            unitSound = gameObject.AddComponent<AudioSource>();
            unitSound.spatialBlend = 1;
            unitSound.PlayOneShot(RandomSound(unitType.spawnSounds));
            outline = visibleObject.AddComponent<Outline>();
            outline.enabled = false;

            if (visibleSpecial != null)
            {
                outlineSpecial = visibleSpecial.AddComponent<Outline>();
                outlineSpecial.enabled = false;
            }
        }

        forward = Vector3.ProjectOnPlane(transform.forward, GetUpVector());

        if (visibleSpecial != null)
        {
            visibleSpecial.SetActive(currentTile.baseBiome == VoronoiTile.Biomes.Water);
            visibleObject.SetActive(currentTile.baseBiome != VoronoiTile.Biomes.Water);
        }

        ChildUnitInit();
        StartOfTurn(); 
    }
    
    void GenerateStats()
    {
        bool fullHealth = health != maxHealth; // Is currently damaged rightnow OR something got more health than maxhealth, would be used by ability stealing, which would be done during the game
        speed = Mathf.Max(ability1.movesCost, ability2.movesCost);
        speed = speed < 0 ? 2 : speed + 1;
        currency = Mathf.Max(Mathf.FloorToInt(Random.Range(ability1.currencyMin, ability1.currencyMax)), Mathf.FloorToInt(Random.Range(ability2.currencyMin, ability2.currencyMax)));
        maxHealth = Mathf.Max(Mathf.FloorToInt(Random.Range(ability1.healthMin, ability1.healthMax)), Mathf.FloorToInt(Random.Range(ability2.healthMin, ability2.healthMax)));
        if (!fullHealth) health = Mathf.Clamp(health, 0, maxHealth);
        else health = maxHealth;
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

        SetOutline(shouldOutline);
    }

    public virtual void StartOfTurn()
    {
        //movesThisTurn = (unitType.baseSpeed + 1) * moveSpeedMultiplier;
        movesThisTurn = (speed + 1) * moveSpeedMultiplier;
        moveSpeedMultiplier = 1;
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

            if (visibleSpecial != null)
            {
                visibleSpecial.SetActive(tile.baseBiome == VoronoiTile.Biomes.Water);
                visibleObject.SetActive(tile.baseBiome != VoronoiTile.Biomes.Water);
            }

            if (unitType.hasAbilities)
            {
                aScript1.OnMove(old);
                aScript2.OnMove(old);
            }
        }
        else
        {
            //Complain
            UIController.instance.PushNotification("Cannot move unit onto water.");
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

    public void StealAbility(UnitController other)
    {
        if (other.unitType.hasAbilities)
        {
            int a1 = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : 2;

            var a = other.GetAbility(a1);
            if (a1 == 1)
            {
                Destroy(aScript1);
                ability1 = a;
                aScript1 = (AbilityBehaviour)gameObject.AddComponent(System.Type.GetType(ability1.abilityScript));
                aScript1.ourAbility = ability1;
            }
            else
            {
                Destroy(aScript2);
                ability2 = a;
                aScript2 = (AbilityBehaviour)gameObject.AddComponent(System.Type.GetType(ability2.abilityScript));
                aScript2.ourAbility = ability2;
            }
        }
    }

    void SetOutline(bool enabled)
    {
        outline.enabled = enabled;
        if (outlineSpecial != null)
        {
            outlineSpecial.enabled = enabled;
        }
    }

    void MakeMove(int cost)
    {
        if (cost < 0)
        {
            movesThisTurn = 0;
            return;
        }

        //movesThisTurn = Mathf.Clamp(movesThisTurn - cost, 0, unitType.baseSpeed);
        movesThisTurn = Mathf.Clamp(movesThisTurn - cost, 0, speed);
    }

    public bool CanMakeMove(int cost)
    {
        if (cost < 0)
        {
            //return movesThisTurn == unitType.baseSpeed + 1;
            return movesThisTurn == speed + 1;
        }

        return movesThisTurn - cost > 0;
    }

    public virtual bool Damage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (damage > 0)
        {
            UIController.instance.PushNotification(string.Format("{0} took {1} damage!", unitType.unitName, damage));
        }
        else if (damage < 0)
        {
            UIController.instance.PushNotification(string.Format("{0} healed {1} HP!", unitType.unitName, -damage));
        }

        if (health <= 0)
        {
            Killed();
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Killed()
    {
        if (isDead)
            return;

        isDead = true;

        if (unitType.hasAbilities)
        {
            aScript1.OnDeath();
            aScript2.OnDeath();
        }

        UIController.instance.PushNotification(string.Format("{0} destroyed!", unitType.unitName));

        Debug.LogFormat("{0} destroyed!", unitType.unitName);
        player.RemoveUnitsFromList(this); //We've been destroyed!
        currentTile.occupyingUnit = null;
        Destroy(gameObject);
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
