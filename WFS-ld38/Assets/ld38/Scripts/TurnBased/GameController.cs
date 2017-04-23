using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Assets;

[AddComponentMenu("ld38/Game Controller")]
public class GameController : MonoBehaviour
{
    #region Declaration
    public List<Player> players = new List<Player>();

    public int PlayerCount
    {
        get
        {
            return players.Count;
        }
    }

    public int seed;
    public float waterLevel;
    
    public Generate planet;
    public GameObject cameraPrefab;

    int currentPlayer;
    int turnNumber;
    [Range(2, 8)]
    public int numPlayers;

    bool gameFinished;
    bool gameStarted;
    bool calculatingCanUseTargetAttack;

    bool isTryingToMoveUnit;

    string currencyName;
    
    public GameObject tileInfo;
    public GameObject abilityPanel;
    public Text biomeText;
    public Text currencyText;
    public Text debugInfo;

    public GameObject tileMarker;

    [HideInInspector]
    public Camera cam;
    [HideInInspector]
    public Unit unitToSpawn; //Spawning off a Unit SO
    //[HideInInspector]
    //public Ability abilityToUse;
    [HideInInspector]
    public int abilityToUseNum = -1;
    [HideInInspector]
    public UnitController selectedUnit; // Selecting a unit to use ability and such
    
    private GameObject hoveredTile;
    private GameObject lastHoveredTile = null;
    private VoronoiTile scriptVoronoiTile;
    private VoronoiTile abilityStartPosition;
    // Check the range of the ability against the start of end position selected
    #endregion

    void Start()
    {
        GeneratePlanet(); //TEMP
        CancelAllSelection(); // Just in case
    }

    void Update()
    {
        if (planet.generationDone && !gameStarted)
        {
            //Start the game
            StartGame();
        }
        else if (gameStarted)
        {
            GameplayUpdate();
        }

        #region DAVID        
        #endregion
    }

    void GameplayUpdate()
    {
        Player current = players[currentPlayer];

        if (!current.hasPlayedThisTurn)
        {
            current.hasPlayedThisTurn = true;
            HandleTurn(current);
        }
        else
        {
            PlayerUpdate(current);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        for (int i = 0; i < numPlayers; i++)
        {
            if (cameraPrefab != null)
            {
                var cam = Instantiate(cameraPrefab);
                cam.GetComponent<CameraControl>().target = planet.gameObject;
                cam.transform.position = planet.transform.position;
                players.Add(new Player(i, false, cam));
            }
            else
            {
                players.Add(new Player(i, false, null));
            }
            Debug.LogFormat("Added player {0}", i);
        }

        currencyName = NameGenerator.Instance.GetRandomName("Currency");
        Debug.LogFormat("Currency name: {0}", currencyName);
    }

    public void GeneratePlanet()
    {
        //planet.seed = seed;
        //planet.landAmount = waterLevel;
        StartCoroutine(planet.Create());
    }

    void PlayerUpdate(Player p)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            p.hasFinishedTurn = true;
        }

        if (p.hasFinishedTurn)
        {
            p.hasPlayedThisTurn = false;
            p.hasFinishedTurn = false;
            if (currentPlayer + 1 < players.Count)
            {
                currentPlayer++;
            }
            else
            {
                currentPlayer = 0; //Wrap back.
                turnNumber++; //We've completed a whole round.
            }
        }

        else
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHover();
                if (lastHoveredTile != hoveredTile)
                {
                    UpdateTileInfo();
                    lastHoveredTile = hoveredTile;
                }
                if (Input.GetButtonDown("Fire1"))
                {
                    HandleMouse1Down(p);                    
                }
            }
            if (Input.GetButtonDown("Fire2"))
            {
                CancelAllSelection();
            }

            UpdateDebugInfo();
        }
    }

    void HandleMouse1Down(Player p)
    {
        if (calculatingCanUseTargetAttack)
            return; //TEMP

        if (abilityToUseNum != -1)
        {
            isTryingToMoveUnit = false;
            StartCoroutine(UseAbilityAtPosition(abilityToUseNum, scriptVoronoiTile));
            return;
        }
        else if (unitToSpawn != null)
        {
            if (scriptVoronoiTile.occupyingUnit == null && scriptVoronoiTile.baseBiome != VoronoiTile.Biomes.Water)
            {
                var unit = SpawnUnitFromScriptableObject(unitToSpawn, p);
                scriptVoronoiTile.occupyingUnit = unit.GetComponent<UnitController>();
                p.AddUnitToList(unit.GetComponent<UnitController>());
            }
            else
            {
                //
                // TELL THE PLAYER YOU CAN'T PLACE THINGS WHERE THERE ARE ALREADY THINGS THERE
                //
            }
        }
        else if (selectedUnit != null && isTryingToMoveUnit)
        {
            MoveSelectedUnit();
        }
        else
        {
            if (scriptVoronoiTile != null && scriptVoronoiTile.occupyingUnit != null && scriptVoronoiTile.occupyingUnit.player != p)
            {
                CancelAllSelection();
            }
            else
            {
                CancelAllSelection();
                UpdateUnitSelected(scriptVoronoiTile);
            }
        }
    }

    void HandleTurn(Player p)
    {
        Debug.LogFormat("Player {0}'s turn", p.number);
        if (p.playerCamera != null)
        {
            foreach (Player p2 in players)
            {
                if (p2.playerCamera != null) p2.playerCamera.SetActive(false);
            }

            p.playerCamera.SetActive(true);
        }
        #region DAVID
        cam = FindObjectOfType<CameraControl>().gameObject.GetComponent<Camera>();
        #endregion

        foreach (var unit in p.units)
        {
            unit.StartOfTurn();
        }
    }

    #region DAVID
    void UpdateDebugInfo()
    {
        debugInfo.text = string.Format("unitToSpawn: {0}\nabilityToUse: {1}\nabilityStartPosition: {2}\nhoveredTile: {3}\nhoveredTilePosition: {4}\nunitSelected: {5}", unitToSpawn, abilityToUseNum, abilityStartPosition == null ? "" : abilityStartPosition.centerPoint.ToString(), hoveredTile, scriptVoronoiTile == null ? "" : scriptVoronoiTile.centerPoint.ToString(), selectedUnit);
    }

    void RaycastHover()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.transform != null)
            {
                hoveredTile = hit.transform.gameObject;
                HighlightObject();
                // ^^^^
                // NOT DONE
                //
            }
            else
            {
                hoveredTile = null;
            }
        }
    }

    public void CloseUI(GameObject UI)
    {
        // PLAY CLOSING ANIMATION

        UI.SetActive(false);
    }

    public void OpenUI(GameObject UI)
    {
        // PLAY OPENING ANIMATION

        UI.SetActive(true);
    }

    void HighlightObject()
    {
        if (hoveredTile != null && tileMarker != null)
        {
            tileMarker.SetActive(true);
            //tileMarker.transform.position = hoveredTile.GetComponent<VoronoiTile>().centerPoint;
            var tile = hoveredTile.GetComponent<VoronoiTile>();
            var center = tile.centerPoint;
            tileMarker.transform.position = Vector3.MoveTowards(tileMarker.transform.position, tile.altitude > 0 ? (center + (center - planet.transform.position) * tile.altitude) : center, 10 * Time.deltaTime);
            tileMarker.transform.up = tileMarker.transform.position - planet.transform.position;
        }
        else if (hoveredTile == null && tileMarker != null)
        {
            tileMarker.SetActive(false);
        }
    }

    void UpdateUnitSelected(VoronoiTile script = null)
    {
        if (script != null)
        {
            if (script.occupyingUnit != null)
            {
                selectedUnit = script.occupyingUnit;
                selectedUnit.shouldOutline = true;
                abilityStartPosition = script;
                //
                // SHOW INFORMATION OF UNIT SELECTED?
                //
                abilityPanel.SetActive(true);
                abilityPanel.transform.FindChild("Unit Name Label").gameObject.GetComponent<Text>().text = script.occupyingUnit.unitType.unitName;
                abilityPanel.transform.FindChild("Move Button").gameObject.SetActive((selectedUnit.unitType.moveType != Unit.UnitType.CurrencyGenerator) && (selectedUnit.unitType.moveType != Unit.UnitType.Captial));                
            }
            else
            {
                CancelAllSelection();
                //selectedUnit.shouldOutline = false;
                //selectedUnit = null;
                //abilityStartPosition = null;
            }
        }
        else
        {
            CancelAllSelection();
        }
    }

    public void SetUnitToSpawn(Unit unit)
    {
        CancelAllSelection(); // Cancel other selections like ability and such
        unitToSpawn = unit;
    }

    public void SetUnitMoveMode()
    {
        if (selectedUnit != null)
        {
            if (selectedUnit.unitType.moveType == Unit.UnitType.CurrencyGenerator || selectedUnit.unitType.moveType == Unit.UnitType.Captial)
                return;

            isTryingToMoveUnit = true;
        }
    }

    void MoveSelectedUnit()
    {
        if (selectedUnit != null)
        {
            var surrounds = selectedUnit.currentTile.neighbors;
            bool valid = false;
            foreach (var tile in surrounds)
            {
                if (scriptVoronoiTile == tile)
                {
                    valid = true;
                }
            }

            if (valid && scriptVoronoiTile.occupyingUnit == null)
            {
                selectedUnit.MoveToTile(scriptVoronoiTile);
            }
        }
    }

    public void SetAbilitiesToUse(int abilityNum)
    {
        if (selectedUnit != null)
        {
            Ability ability = abilityNum == 1 ? selectedUnit.ability1 : selectedUnit.ability2;
            // Temp storage to check if the ability is targeted

            if (ability.range <= 0)
            {
                UseAbilityAtPosition(abilityNum);
                //UseAbilityFromScriptableObject(ability);
            }
            else
            {
                abilityToUseNum = abilityNum;
            }
        }
        else
        {
            //
            // THE ABILITY PANEL SHOULDN'T SHOW UP WHEN A UNIT IS NOT SELECTED
            //
        }
    }

    GameObject SpawnUnitFromScriptableObject(Unit u, Player p)
    {
        var go = new GameObject(u.unitName, typeof(UnitController));
        Debug.Log(scriptVoronoiTile.altitude);
        go.transform.position = scriptVoronoiTile.centerPoint;
        var crtl = go.GetComponent<UnitController>();
        crtl.unitType = u;
        crtl.currentTile = scriptVoronoiTile;
        crtl.player = p;
        crtl.InitUnit();
        return go;
    }

    IEnumerator UseAbilityAtPosition(int abilityNum, VoronoiTile targetedPosition = null)
    {        
        if (targetedPosition == null) // Not targeted
        {
            selectedUnit.UseAbility(abilityNum == 1, abilityStartPosition);
            abilityToUseNum = -1;
        }
        else // Targeted
        {
            ThreadedSearch t = ThreadedSearch.CanMoveTo(selectedUnit.currentTile, targetedPosition, selectedUnit.GetAbility(abilityNum).range);
            calculatingCanUseTargetAttack = true;

            while (!t.isDone)
            {
                yield return null;
            }

            if (t.Result)
            {
                selectedUnit.UseAbility(abilityNum == 1, targetedPosition);
                abilityToUseNum = -1;
            }
            else
            {
                //ERROR: attack out of range!
                abilityToUseNum = 1;
            }

            calculatingCanUseTargetAttack = false;
        }
    }

    //GameObject UseAbilityFromScriptableObject(Ability a)
    //{
    //    if (a.usesTarget)
    //    {
    //        a.
    //    }
    //    //
    //    // NOT IMPLEMENTED
    //    //
    //    throw new NotImplementedException();
    //}

    void CancelAllSelection()
    {
        if (selectedUnit != null)
        {
            selectedUnit.shouldOutline = false;
            selectedUnit = null;            
        }
        unitToSpawn = null;
        //abilityToUse = null;
        abilityToUseNum = -1;
        abilityStartPosition = null;
        isTryingToMoveUnit = false;
        abilityPanel.SetActive(false);
    }

    void UpdateTileInfo()
    {
        if (hoveredTile == null)
        {
            // NOT WORKING
            scriptVoronoiTile = null;
            CloseUI(tileInfo);
        }
        else
        {
            if (lastHoveredTile == null)
            {
                OpenUI(tileInfo);
            }
            scriptVoronoiTile = hoveredTile.GetComponent<VoronoiTile>();
            biomeText.text = string.Format("Biome: {0}", scriptVoronoiTile.baseBiome);
            currencyText.text = string.Format("{0}: {1}", currencyName, 10); // PLACEHOLDER
        }
    }


    #endregion    
}

public class Player
{
    public Player(int index, bool ai, GameObject cam)
    {
        number = index;
        isAI = ai;
        playerCamera = cam;
        units = new List<UnitController>();
    }

    public bool hasFinishedTurn;
    public bool hasPlayedThisTurn;
    public bool isAI;
    public int number;
    public GameObject playerCamera;

    #region Gameplay Properties
    public ulong currencyPoints;
    public ulong currencyPerTurn;
    public int health;

    public List<UnitController> units;
    #endregion

    public void AddUnitToList(UnitController c)
    {
        if (!units.Contains(c))
        {
            units.Add(c);
        }
    }

    public void RemoveUnitsFromList(UnitController c)
    {
        if (units.Contains(c))
        {
            units.Remove(c);
        }
    }
}