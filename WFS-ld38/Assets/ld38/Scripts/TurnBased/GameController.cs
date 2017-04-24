using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Assets;
using cakeslice;

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
    public int curPlayerCount;

    bool gameFinished;
    bool gameStarted;
    bool calculatingCanUseTargetAttack;

    bool isTryingToMoveUnit;

    string currencyName;

    public bool isPaused;

    public Text debugInfo;
    public GameObject tileMarker;
    public BoundaryOutliner boundary;

    public AudioClip clickSound;
    public AudioClip deselectSound;
    public AudioClip noSound;
    AudioSource audio;

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
    private List<List<VoronoiTile>> playerRange = new List<List<VoronoiTile>>();


    #endregion

    void Start()
    {
        audio = GetComponent<AudioSource>();
        //GeneratePlanet(); //TEMP
        CancelAllSelection(); // Just in case
        isPaused = false;
    }

    void Update()
    {
        if (planet.generationDone && !gameStarted)
        {
            //Start the game
            StartGame();
        }
        else if (gameFinished)
        {
            Debug.Log("Game over.");
        }

        else if (gameStarted)
        {
            GameplayUpdate();
        }
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
        curPlayerCount = numPlayers;
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

            if (GetComponent<SpawnCapital>() != null)
            {
                GetComponent<SpawnCapital>().GenerateCapital(players[i]);
            }
        }

        currencyName = NameGenerator.Instance.GetRandomName("Currency");
        Debug.LogFormat("Currency name: {0}", currencyName);
        if (tileMarker != null)
        {
            tileMarker.SetActive(true);
        }
    }

    public void GeneratePlanet()
    {
        //planet.seed = seed;
        //planet.landAmount = waterLevel;
        StartCoroutine(planet.Create());
    }

    void EndOfTurn(Player p)
    {
        CancelAllSelection();
        p.hasPlayedThisTurn = false;
        p.hasFinishedTurn = false;

        foreach (var unit in p.units)
        {
            unit.EndOfTurn();
        }

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

    void PlayerUpdate(Player p)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            p.hasFinishedTurn = true;
        }

        if (p.hasFinishedTurn)
        {
            EndOfTurn(p);
        }

        else if (!isPaused) // PAUSE FUNCTION HERE
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
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    CancelAllSelection(true);
                }
            }

        }
        UpdateDebugInfo();

        UIController.instance.UpdateCurrency(currencyName, p.currency, p.currencyPerTurn);
    }

    void HandleMouse1Down(Player p)
    {
        //if (calculatingCanUseTargetAttack)
        //    return; //TEMP

        if (abilityToUseNum != -1)
        {
            isTryingToMoveUnit = false;
            UseAbilityAtPosition(abilityToUseNum, scriptVoronoiTile);
            //StartCoroutine(UseAbilityAtPosition(abilityToUseNum, scriptVoronoiTile));
            //return;
        }
        else if (unitToSpawn != null)
        {
            if (scriptVoronoiTile.occupyingUnit == null && scriptVoronoiTile.baseBiome != VoronoiTile.Biomes.Water)
            {
                // TODO currency checks here!

                var unit = SpawnUnitFromScriptableObject(unitToSpawn, p);
                scriptVoronoiTile.occupyingUnit = unit.GetComponent<UnitController>();
                p.AddUnitToList(unit.GetComponent<UnitController>());
                UIController.instance.UpdateCurrency(currencyName, p.currency, p.currencyPerTurn);
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
            PlaySelectSound();
        }
        else
        {
            if (scriptVoronoiTile != null && scriptVoronoiTile.occupyingUnit != null && scriptVoronoiTile.occupyingUnit.player != p) //Is enemy unit
            {
                CancelAllSelection();
                PlayNoSound();
            }
            else
            {
                CancelAllSelection();
                UpdateUnitSelected(scriptVoronoiTile);
            }
        }

        if (selectedUnit != null)
        {
            UIController.instance.UpdateAbilityPanelName(selectedUnit.unitType.unitName, selectedUnit.movesThisTurn);
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

        UIController.instance.UpdateDescriptionEnabled(false);
        foreach (var unit in p.units)
        {
            unit.StartOfTurn();
        }

        UIController.instance.UpdateCurrency(currencyName, p.currency, p.currencyPerTurn);
        UIController.instance.UpdateHP(p.health);
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
            }
            else
            {
                hoveredTile = null;
            }

            HighlightObject();
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
            UIController.instance.UpdateInfoPanelEnabled(true);
            tileMarker.SetActive(true);
            //tileMarker.transform.position = hoveredTile.GetComponent<VoronoiTile>().centerPoint;
            var tile = hoveredTile.GetComponent<VoronoiTile>();
            var center = tile.centerPoint;
            tileMarker.transform.position = Vector3.MoveTowards(tileMarker.transform.position, tile.altitude > 0 ? (center + (center - planet.transform.position) * tile.altitude) : center, 10 * Time.deltaTime);
            tileMarker.transform.up = tileMarker.transform.position - planet.transform.position;

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                UIController.instance.UpdateDescriptionEnabled(tile.occupyingUnit != null);
                if (tile.occupyingUnit != null)
                {
                    UIController.instance.UpdateDescription(tile.occupyingUnit.unitType.unitName, tile.occupyingUnit.unitType.description);
                }
            }
        }
        else if (hoveredTile == null && tileMarker != null)
        {
            UIController.instance.UpdateInfoPanelEnabled(false);
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

                PlaySelectSound();

                //
                // SHOW INFORMATION OF UNIT SELECTED?
                //
                UIController.instance.UpdateAbilityPanelEnabled(true);
                UIController.instance.UpdateAbilityPanelName(script.occupyingUnit.unitType.unitName, script.occupyingUnit.movesThisTurn);
                bool ab = selectedUnit.unitType.hasAbilities;
                UIController.instance.UpdateAbility1Enabled(ab);
                UIController.instance.UpdateAbility2Enabled(ab);

                if (ab)
                {
                    UIController.instance.UpdateAbility1Name(selectedUnit.ability1.abilityName);
                    UIController.instance.UpdateAbility2Name(selectedUnit.ability2.abilityName);
                }

                UIController.instance.UpdateMoveButtonEnabled((selectedUnit.unitType.moveType != Unit.UnitType.CurrencyGenerator) && (selectedUnit.unitType.moveType != Unit.UnitType.Captial));
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
            else
            {
                PlayNoSound();
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
                //ThreadedSearch t = ThreadedSearch.GetPlayerRange(selectedUnit.currentTile, selectedUnit.GetAbility(abilityNum).range);
                playerRange = VoronoiTile.FindTilesInRange(selectedUnit.currentTile, selectedUnit.GetAbility(abilityNum).range);
                ShowPlayerRange();
            }
        }
        //else
        //{
        //    //
        //    // THE ABILITY PANEL SHOULDN'T SHOW UP WHEN A UNIT IS NOT SELECTED
        //    //
        //}
    }

    void ShowPlayerRange()
    {
        //
        // TEMP, CHANGE THIS
        //
        //Debug.LogError(playerRange.Count);

        List<VoronoiTile> last = playerRange[playerRange.Count - 1];
        try
        {
            boundary.MakeBoundary(last);
        }
        catch
        { }
        //Debug.LogWarning("LAST LIST COUNT: " + last.Count.ToString());
        //for (int i = 0; i < last.Count; i++)
        //{
        //    var center = last[i].centerPoint;
        //    center = last[i].altitude > 0 ? (center + (center - planet.transform.position) * last[i].altitude) : center;
        //    Instantiate(spawnThings, center, new Quaternion(0, 0, 0, 0));
        //}
    }

    public void SetAbilityDescription(int abilityNum)
    {
        if (abilityNum == -1)
        {
            UIController.instance.UpdateDescriptionEnabled(false);
            return;
        }

        if (selectedUnit != null)
        {
            Ability ability = abilityNum == 1 ? selectedUnit.ability1 : selectedUnit.ability2;
            UIController.instance.UpdateDescriptionEnabled(true);
            UIController.instance.UpdateDescription(ability.abilityName, ability.description);
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

    void UseAbilityAtPosition(int abilityNum, VoronoiTile targetedPosition = null)
    {
        if (selectedUnit.unitType.moveType == Unit.UnitType.Sea)
        {
            if (selectedUnit.currentTile.baseBiome != VoronoiTile.Biomes.Water)
            {
                abilityToUseNum = -1;
                PlayNoSound();
                return;
            }
        }

        if (!selectedUnit.CanMakeMove(selectedUnit.GetAbility(abilityNum).movesCost))
        {
            //Complain about lack of points.
            abilityToUseNum = -1;
            PlayNoSound();
            return;
        }

        if (targetedPosition == null) // Not targeted
        {
            selectedUnit.UseAbility(abilityNum == 1, abilityStartPosition);
            abilityToUseNum = -1;

            //
            // NOT WORKING
            //

        }
        else // Targeted
        {
            if (playerRange.SelectMany(list => list).Contains(targetedPosition))
            {
                selectedUnit.UseAbility(abilityNum == 1, targetedPosition);
                abilityToUseNum = -1;
                CancelAllSelection();
            }
            else
            {
                //ERROR: attack out of range!
                abilityToUseNum = abilityNum;
                PlayNoSound();
            }
            //ThreadedSearch t = ThreadedSearch.CanMoveTo(selectedUnit.currentTile, targetedPosition, selectedUnit.GetAbility(abilityNum).range);

            //calculatingCanUseTargetAttack = true;

            //while (!t.isDone)
            //{
            //    yield return null;
            //}

            //if (t.Result)
            //{
            //    selectedUnit.UseAbility(abilityNum == 1, targetedPosition);
            //    abilityToUseNum = -1;
            //}
            //else
            //{
            //    //ERROR: attack out of range!
            //    abilityToUseNum = abilityNum;
            //    PlayNoSound();
            //}

            //calculatingCanUseTargetAttack = false;
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

    public void Pause(bool paused)
    {
        isPaused = paused;
        UIController.instance.UpdatePause(paused);
    }

    public void SceneRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void PlaySelectSound()
    {
        audio.PlayOneShot(clickSound);
    }

    public void PlayNoSound()
    {
        audio.PlayOneShot(noSound);
    }

    public void CancelAllSelection(bool playSound = false)
    {
        if (playSound)
        {
            audio.PlayOneShot(deselectSound);
        }

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
        playerRange.Clear();
        UIController.instance.UpdateAbilityPanelEnabled(false);
        boundary.RemoveBoundary();
    }

    void UpdateTileInfo()
    {
        if (hoveredTile == null)
        {
            // NOT WORKING
            scriptVoronoiTile = null;
            UIController.instance.UpdateInfoPanelEnabled(false);
            //CloseUI(tileInfo);
        }
        else
        {
            if (lastHoveredTile == null)
            {
                UIController.instance.UpdateInfoPanelEnabled(true);
                //OpenUI(tileInfo);
            }
            scriptVoronoiTile = hoveredTile.GetComponent<VoronoiTile>();
            UIController.instance.UpdateInfoBiome(scriptVoronoiTile.baseBiome);
            UIController.instance.UpdateInfoCurrencyValue(currencyName, scriptVoronoiTile.currency);
            //currencyText.text = string.Format("{0}: {1}", currencyName, scriptVoronoiTile.currency); // PLACEHOLDER
        }
    }

    public void PlayerHurt(Player p)
    {
        if (players.IndexOf(p) == currentPlayer)
        {
            UIController.instance.UpdateHP(p.health);
        }
    }

    public void PlayerKilled(Player p)
    {
        CancelAllSelection();
        foreach (var unit in p.units.ToArray())
        {
            unit.Killed();
        }

        p.isDead = true;
        curPlayerCount--;

        if (curPlayerCount == 1)
        {
            //Someone won. End game.
            gameFinished = true;
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
        playerColor = colors[Mathf.Clamp(index, 0, 8)];

        playerCamera.GetComponent<OutlineEffect>().lineColor0 = playerColor;
    }    

    public bool hasFinishedTurn;
    public bool hasPlayedThisTurn;
    public bool isAI;
    public int number;
    public GameObject playerCamera;

    public bool isDead;

    #region Gameplay Properties
    public long currency;
    public long currencyPerTurn;
    public int health;

    public List<UnitController> units;

    public Color playerColor;
    #endregion

    static Color[] colors = new Color[]
    {
        Color.red,
        Color.cyan,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.blue,
        new Color32(255, 132, 0, 1), //Orange
        new Color32(81, 0, 255, 1) //Purpleish
    };

    public void AddCurrency(long add)
    {
        currency += add;        
    }

    public void AddCurrencyPerTurn(long add)
    {
        if (long.MaxValue - currencyPerTurn == add)
        {
            currencyPerTurn = long.MaxValue;
        }
        else
        {
            currencyPerTurn += add;
        }        
    }

    public void RemoveCurrencyPerTurn(long rm)
    {
        if (currencyPerTurn - rm < 0)
        {
            currencyPerTurn = 0;
        }
        else
        {
            currencyPerTurn -= rm;
        }
    }

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