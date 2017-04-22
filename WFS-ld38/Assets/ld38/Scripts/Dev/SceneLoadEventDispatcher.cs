using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("LD38/On Scene Load")]
public class SceneLoadEventDispatcher : MonoBehaviour
{
    public UnityEvent OnLoad;

    // Use this for initialization
    void Start()
    {
        if (OnLoad != null)
        {
            OnLoad.Invoke();
        }
    }
}
