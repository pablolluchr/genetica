using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planet;
    public Transform focusUnit;
    [SerializeField] private float speed = 5f;
    public float zoomedCameraOffset = 2f;
    //float fov = 50f;
    //float sensitivity = 17f;

    // Update is called once per frame

    private void Awake()
    {
        //focusUnit = null;
    }
    void Update()
    {
        

        if (focusUnit == null)
        {
            //free around planet
            //TODO: enable for touch devices.
            //TODO: enable leftover movement after mouse up (maybe apply force instead of change position?)
            if (Input.GetMouseButton(0))
            {
                transform.RotateAround(planet.transform.position, transform.up, Input.GetAxis("Mouse X") * speed);
                transform.RotateAround(planet.transform.position, transform.right, -Input.GetAxis("Mouse Y") * speed);

            }
        }
        else
        {
            //rotation around planet with target focused on center
            Vector3 cameraPosition = (focusUnit.position + (focusUnit.position - planet.transform.position))*zoomedCameraOffset;
            transform.position = cameraPosition;
            transform.LookAt(planet.transform);

        }

    }
}
