using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets;
using System;

public class BoundaryOutliner : MonoBehaviour
{
    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    public void MakeBoundary(List<VoronoiTile> tilesList)
    {
        List<VoronoiTile> tiles = SortList(tilesList);
        if (line != null)
        {
            List<Vector3> positions = new List<Vector3>();
            positions.AddRange(tiles.Select(x => x.altitude > 0 ? (x.centerPoint + (x.centerPoint - Vector3.zero) * x.altitude) : x.centerPoint));
            line.numPositions = tiles.Count+1;
            line.SetPositions(positions.ToArray());
            line.SetPosition(line.numPositions - 1, positions[0]);
        }
    }

    List<VoronoiTile> SortList(List<VoronoiTile> listToClone)
    {
        List<VoronoiTile> list = new List<VoronoiTile>();
        for (int i = 0; i < listToClone.Count; i++)
        {
            list.Add(listToClone[i]);
        }
        List<VoronoiTile> tempList = new List<VoronoiTile>() { list[0] };
        list.RemoveAt(0);
        int count = list.Count - 1;
        for (int i = 0; i < count; i++)
        {
            //for (int j = 0; j < count - i - 1; j++)
            //{
            //    if (list[i].neighbors.Contains(list[Mathf.FloorToInt(Mathf.Clamp(j+i+1, 0, count-i))]))
            //    {
            //        VoronoiTile item = list[j + i+1];
            //        list.RemoveAt(j + i+1);
            //        list.Insert(Mathf.FloorToInt(Mathf.Clamp(i + 1, 0, count-1)), item);
            //        break;
            //    }
            //}
            tempList.Add(list.Where(x => tempList[i].neighbors.Contains(x)).FirstOrDefault());
            if (i < count - 1)
            {
                list.Remove(tempList[i + 1]);
            }
        }
        return tempList;
    }

    public void RemoveBoundary()
    {
        line.numPositions = 0;
    }
}