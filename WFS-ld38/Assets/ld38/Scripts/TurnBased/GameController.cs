using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[AddComponentMenu("ld38/Game Controller")]
public class GameController : MonoBehaviour
{
    public List<Player> players = new List<Player>();

    public int seed;
    public float waterLevel;

    public Generate planet;

    bool isPlayerTurn; //Lower first
    int turnNumber;

    bool gameFinished;
    bool hasGeneratedPlanet;

    void Start()
    {
        StartGame(); //TEMP
    }

    void Update()
    {

    }

    public void StartGame()
    {
        Thread t = new Thread(new ThreadStart(delegate()
        {
            hasGeneratedPlanet = false;
            if (planet != null)
            {
                planet.Create();
            }
            hasGeneratedPlanet = true;
        }));
    }

    IEnumerator HandleTurn(Player p)
    {
        while (!p.hasFinishedTurn)
        {
            //Do turn stuff here.
            yield return new WaitForEndOfFrame();
        }
    }
}

public class Player
{
    public bool hasFinishedTurn;
}