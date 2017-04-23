using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInGame : MonoBehaviour
{
#if DEBUG
    void Awake()
    {
        gameObject.SetActive(false);
    }
#endif
}
