using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planet;
    public Transform focusUnit;
    [SerializeField] private float speed = 5f;
    public float zoomedCameraOffset = 2f;
    public float distance;
    private float rotXAxis;
    private float rotYAxis;
    //float fov = 50f;
    //float sensitivity = 17f;

    // Update is called once per frame

    private void Awake()
    {
        //focusUnit = null;
    }
    void Update()
    {
        

        if (focusUnit != null)
        {
            //free around planet
            //TODO: enable for touch devices.
            //TODO: enable leftover movement after mouse up (maybe apply force instead of change position?)
            if (Input.GetMouseButton(0))
            {

                Orbit();

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
    

    void Start()
    {
        //vector from where I am to what I'm rotating around
        Vector3 initialVector = transform.position - planet.transform.position;

        //current angles of camera (make sure it's looking at what you want
        //to rotate around)
        float rotYAxis = transform.eulerAngles.y;
        float rotXAxis = transform.eulerAngles.x;
        //distance between me and what I'm rotating around
        distance = Vector3.Magnitude(initialVector);
    }

    //You can probably call this in update I just call it from a dif script
    public void Orbit()
    {

        //get your inputs
        rotYAxis += Input.GetAxis("Mouse X") * speed;
        rotXAxis -= Input.GetAxis("Mouse Y") * speed;

        //clamp the angle
        //rotXAxis = ClampAngle(rotXAxis, thirdMin, thirdMax);

        // convert it to quaternions
        Quaternion toRotation = Quaternion.Euler(rotXAxis, rotYAxis, 0);
        Quaternion rotation = toRotation;

        //figure out what your distance should be (so that it's rotating around 
        //not just rotating)
        Vector3 negDistance = new Vector3(0, 0, -distance);
        Vector3 position = rotation * negDistance + planet.transform.position;

        //and apply!
        transform.rotation = rotation;
        transform.position = position;
    }

    //clamp angle from before
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
