using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Assets;

public class ThreadedSearch
{ 
    private ThreadedSearch(VoronoiTile v1, VoronoiTile v2, int pdepth)
    {
        thread = new Thread(new ThreadStart(delegate ()
        {
            result = VoronoiTile.CanMoveTo(v1, v2, pdepth);
            done = true;
        }));

        thread.Start();        
    }
    Thread thread;

    bool done;
    public bool isDone { get { return done; } }
    bool result;
    public bool Result
    {
        get
        {
            if (isDone)
            {
                return result;
            }
            else
            {
                throw new System.Exception("Cannot access calculation result until the thread has finished.");
            }
        }
    }

    public static ThreadedSearch CanMoveTo(VoronoiTile start, VoronoiTile end, int depth)
    {
        var threader = new ThreadedSearch(start, end, depth);
        return threader;
    }

    ~ThreadedSearch()
    {
        thread.Abort();
    }
}
