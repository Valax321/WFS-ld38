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
    public GameController scriptGameController;
    #endregion


    private void Start()
    {
        transform.localPosition = new Vector3(0, 0, -((zoomMin + zoomMax) / 2));
        scriptGameController = (GameController)FindObjectOfType<GameController>();
    }
 
	void Update ()
    {
		if (target != null && !scriptGameController.isPaused)
        {
            //transform.LookAt(target.transform);

            if (orbitingPlanet)
            {
                float moveHorizontal;
                float moveVertical;
                if (Input.GetMouseButton(2))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    moveHorizontal = Input.GetAxis("Mouse X");
                    moveVertical = Input.GetAxis("Mouse Y") * -1;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    moveHorizontal = Input.GetAxis("Horizontal") * -1;
                    moveVertical = Input.GetAxis("Vertical");
                }
                
                transform.RotateAround(target.transform.position, transform.up, Time.deltaTime * moveHorizontal * moveSpeed);
                transform.RotateAround(target.transform.position, transform.right, Time.deltaTime * moveVertical * moveSpeed);
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
                float movement = 0;

                if (Input.GetAxis("Zoom") < 0 && distance> zoomMin)
                {
                    movement = Mathf.Clamp(zoomSpeed * Time.deltaTime * Mathf.Abs(Input.GetAxis("Zoom")), 0, distance - zoomMin);
                    transform.position += transform.forward * movement;
                }
                else if (Input.GetAxis("Zoom") > 0 && distance + movement < zoomMax)
                {
                    movement = Mathf.Clamp(zoomSpeed * Time.deltaTime * Mathf.Abs(Input.GetAxis("Zoom")), 0, zoomMax - distance);
                    transform.position -= transform.forward * movement;
                }
            }
        }
	}
}
