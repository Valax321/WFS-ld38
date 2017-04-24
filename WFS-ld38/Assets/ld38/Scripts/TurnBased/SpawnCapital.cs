using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets;

public class SpawnCapital : MonoBehaviour {

    public Unit unit;
    public GameObject planet;
    public GameController scriptGameController;

    private GameObject plate;
    private VoronoiTile tile;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateCapital(Player p)
    {
        //
        // FILE IT UNDER DIFFERENT PLAYER
        //

        bool foundLandPlate = false;
        var possibleChild = Enumerable.Range(1, planet.transform.childCount-1).ToList();
        while (!foundLandPlate)
        {
            int num = Random.Range(0, possibleChild.Count);
            plate = planet.transform.GetChild(possibleChild[Mathf.FloorToInt(num)]).gameObject;
            TectonicPlate scriptPlate = plate.GetComponent<TectonicPlate>();
            if (scriptPlate.isLand && plate.transform.childCount > 7)
            {
                foundLandPlate = true;
            }
            else
            {
                possibleChild.RemoveAt(num);
            }
        }

        do
        {
            tile = plate.transform.GetChild(Mathf.FloorToInt(Random.Range(0, plate.transform.childCount))).gameObject.GetComponent<VoronoiTile>();
        } while (tile.occupyingUnit != null);

        var go = new GameObject(unit.unitName, typeof(Captial));
        Debug.Log(tile.altitude);
        go.transform.position = tile.centerPoint;
        var crtl = go.GetComponent<Captial>();
        crtl.unitType = unit;
        crtl.currentTile = tile;
        crtl.player = p;
        crtl.InitUnit();
        tile.occupyingUnit = crtl;
    }
}
