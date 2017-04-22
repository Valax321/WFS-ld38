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

    string currencyName;

    [HideInInspector]
    public Camera cam;
    public GameObject unitSelected;
    public Unit unitToSpawn; //Spawning off a Unit SO
    public GameObject tileInfo;
    public Text biomeText;
    public Text currencyText;

    private GameObject hoveredTile;
    private GameObject lastHoveredTile = null;
    private VoronoiTile scriptVoronoiTile;
    #endregion

    void Start()
    {
        GeneratePlanet(); //TEMP
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
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHover();
            if (Input.GetButtonDown("Fire1"))
            {
                //if (unitSelected != null)
                //{
                //    PlaceUnits();
                //}

                if (unitToSpawn != null)
                {
                    var unit = SpawnUnitFromScriptableObject(unitToSpawn);
                }
            }
            //if (Input.GetButtonDown("Fire2"))
            //{
            //    CancelSelection();
            //}
            if (lastHoveredTile != hoveredTile)
            {
                UpdateTileInfo();
                lastHoveredTile = hoveredTile;
            }
        }

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
    }

    #region DAVID
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

    }

    public void SetUnitSelected(GameObject unit)
    {
        unitSelected = unit;
    }

    [Obsolete("Use SpawnUnitFromScriptableObject() instead.")]
    void PlaceUnits()
    {
        Instantiate(unitSelected, scriptVoronoiTile.centerPoint, new Quaternion(0,0,0,0));
        // THE CENTRE THAT THE SCRIPT RETURNS IN AT ALTITUDE 0

        // DO MORE CALCULATION HERE
    }

    GameObject SpawnUnitFromScriptableObject(Unit u)
    {
        var go = new GameObject(u.unitName, typeof(UnitController));
        Debug.Log(scriptVoronoiTile.altitude);
        go.transform.position = scriptVoronoiTile.centerPoint;
        var crtl = go.GetComponent<UnitController>();
        crtl.unitType = u;
        crtl.currentTile = scriptVoronoiTile;
        crtl.InitUnit();
        return go;
    }

    void CancelSelection()
    {
        unitSelected = null;
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
            biomeText.text = string.Format("Biome: {0}", scriptVoronoiTile.biome);
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
    #endregion
}