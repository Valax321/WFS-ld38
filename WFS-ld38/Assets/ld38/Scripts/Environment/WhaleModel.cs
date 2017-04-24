using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleModel : MonoBehaviour
{
    public GameObject landModel;
    public GameObject seaModel;

    void Update()
    {
        if (GetComponentInParent<UnitController>() != null)
        {
            var unit = GetComponentInParent<UnitController>();
            if (unit.currentTile != null)
            {
                var biome = unit.currentTile.baseBiome;
                switch (biome)
                {
                    case Assets.VoronoiTile.Biomes.Water:
                        seaModel.SetActive(true);
                        landModel.SetActive(false);
                        break;
                    default:
                        seaModel.SetActive(false);
                        landModel.SetActive(true);
                        break;
                }
            }
        }
    }
}
