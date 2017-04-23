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
    VoronoiTile target;
    int depth;
    bool success;

    void OnGUI()
    {
        start = (VoronoiTile)EditorGUILayout.ObjectField("Start", start, typeof(VoronoiTile), true);
        target = (VoronoiTile)EditorGUILayout.ObjectField("Target", target, typeof(VoronoiTile), true);
        depth = EditorGUILayout.IntSlider("Search Depth", depth, 1, 5);
        if (GUILayout.Button("Calculate!"))
        {
            success = VoronoiTile.CanMoveTo(start, target, depth);
        }
        EditorGUILayout.LabelField(string.Format("Successfully found: {0}", success));
    }
}
