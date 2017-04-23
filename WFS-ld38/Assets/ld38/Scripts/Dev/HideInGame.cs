using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInGame : MonoBehaviour
{
#if UNITY_EDITOR
    void Awake()
    {
        gameObject.SetActive(false);
    }
#endif
}
