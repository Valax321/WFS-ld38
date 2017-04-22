using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    #region Declarations
    public float zoomMin = 10.0f;
    public float zoomMax = 50.0f;
    public float zoomSpeed = 10.0f;
    public float moveSpeed = 60.0f;
    public bool zoomEnabled = true;
    public bool orbitingPlanet = true;
    public GameObject target = null;
    #endregion


    private void Start()
    {
        transform.localPosition = new Vector3(0, 0, -((zoomMin + zoomMax) / 2));
    }
 
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

            if (zoomEnabled)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                //float zoom = Mathf.Clamp(Input.GetAxis("Zoom") * zoomSpeed * Time.deltaTime * -1 + distance, zoomMin, zoomMax) - distance;
                ////Debug.LogWarning(zoom + ", " + distance);
                //transform.Translate(transform.forward * zoom, target.transform);

                //float zoom = Input.GetAxis("Zoom") * zoomSpeed * Time.deltaTime;
                //if (((zoom < 0 && distance > zoomMin) || (zoom > 0 && distance < zoomMax)))
                //{
                //    transform.Translate(transform.forward * zoom, target.transform);
                //}

                if (Input.GetAxis("Zoom") < 0 && distance > zoomMin)
                {
                    transform.position += transform.forward * zoomSpeed * Time.deltaTime * Mathf.Abs(Input.GetAxis("Zoom"));
                }
                else if (Input.GetAxis("Zoom") > 0 && distance < zoomMax)
                {
                    transform.position -= transform.forward * zoomSpeed * Time.deltaTime * Mathf.Abs(Input.GetAxis("Zoom"));
                }
            }
        }
	}
}
