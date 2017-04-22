using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    #region Declarations
    [Range(0, 10)]
    public float zoomLevel;
    public float zoomSpeed = 50.0f;
    public float moveSpeed = 50.0f;
    public bool zoomEnabled = true;
    public bool orbitingPlanet = true;
    public GameObject target = null;
    #endregion

	void Update ()
    {
		if (target != null)
        {
            //transform.LookAt(target.transform);

            if (orbitingPlanet)
            {
                float moveHorizontal = Input.GetAxis("Horizontal") * -1 * moveSpeed;
                float moveVertical = Input.GetAxis("Vertical") * moveSpeed;
                
                transform.RotateAround(target.transform.position, transform.up, Time.deltaTime * moveHorizontal);
                transform.RotateAround(target.transform.position, transform.right, Time.deltaTime * moveVertical);
            }

            //if (zoomEnabled)
            //{
            //    float zoom = Input.GetAxis("Zoom") * zoomSpeed;
            //    transform.Translate(new Vector3(0, Time.deltaTime * zoom, 0), Space.World);
            //}
        }
	}
}
