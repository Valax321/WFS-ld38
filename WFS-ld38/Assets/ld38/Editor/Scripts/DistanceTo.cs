using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets;

public class DistanceTo : EditorWindow
{
    [MenuItem("Debug/Tile Search")]
    public static void Make()
    {
        DistanceTo wnd = (DistanceTo)GetWindow(typeof(DistanceTo));
        wnd.Show();
    }

    VoronoiTile start;
    //VoronoiTile target;
    int depth;
    //bool success;
    List<List<VoronoiTile>> result = new List<List<VoronoiTile>>();
    bool working;
    ThreadedSearch search;
    BoundaryOutliner scriptBoundaryOutliner = new BoundaryOutliner();

    void OnGUI()
    {
        start = (VoronoiTile)EditorGUILayout.ObjectField("Start", start, typeof(VoronoiTile), true);
        //target = (VoronoiTile)EditorGUILayout.ObjectField("Target", target, typeof(VoronoiTile), true);
        depth = EditorGUILayout.IntSlider("Search Depth", depth, 1, 5);
        if (GUILayout.Button("Calculate!"))
        {
            if (search == null)
            {
                //search = ThreadedSearch.CanMoveTo(start, target, depth);
                search = ThreadedSearch.CanMoveTo(start, depth);
                working = true;
            }
        }

        if (!working && search != null)
        {
            if (!search.isDone) return;

            //success = search.Result;
            result = search.Result;
            scriptBoundaryOutliner.MakeBoundary(result[result.Count - 1]);
            //List<VoronoiTile> last = result[result.Count - 1];
            //foreach (VoronoiTile item in last[last.Count-1])
            //{

            //}
            search = null;
        }

        if (search != null)
        {
            working = !search.isDone;
        }

        EditorGUILayout.LabelField(string.Format("Successfully found: {0}", result.ToString()));
    }
}
