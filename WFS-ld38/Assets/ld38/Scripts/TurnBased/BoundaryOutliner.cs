using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets;

public class BoundaryOutliner : MonoBehaviour
{
    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();        
    }

    public void MakeBoundary(List<VoronoiTile> tiles)
    {        
        if (line != null)
        {
            List<Vector3> positions = new List<Vector3>();
            positions.AddRange(tiles.Select(x => x.centerPoint));
            line.numPositions = tiles.Count;
            line.SetPositions(positions.ToArray());
        }
    }

    public void RemoveBoundary()
    {
        
    }
}
