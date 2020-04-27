using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    [SerializeField] private float speed = 5f;
    //float fov = 50f;
    //float sensitivity = 17f;

    // Update is called once per frame
    void Update()
    {
        //TODO: enable for touch devices.
        //TODO: enable leftover movement after mouse up (maybe apply force instead of change position?)
        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(target.transform.position, transform.up, Input.GetAxis("Mouse X") * speed);
            transform.RotateAround(target.transform.position, transform.right, -Input.GetAxis("Mouse Y") * speed);

        }

    }
}
