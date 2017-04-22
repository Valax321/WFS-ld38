using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Environment/Time of Day")]
public class TimeOfDay : MonoBehaviour
{
    public float sunSpeed = 10f;

    void Update()
    {
        transform.Rotate(Vector3.up * sunSpeed * Time.deltaTime);
    }
}
