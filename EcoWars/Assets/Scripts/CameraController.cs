using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planet;
    [System.NonSerialized] public Transform target;
    [SerializeField] private float speed = 5f;
    public float distance;
    private float rotXAxis;
    private float rotYAxis;
    [SerializeField] private float defaultSize= 7.5f;
    [SerializeField] private float zoomedSize=5.5f;
    public float cameraOffset;
    [SerializeField] private float cameraMoveSpeed =10f;
    [SerializeField] private float cameraRotateSpeed =10f;
    public float orbitStartTime;
    //float fov = 50f;
    //float sensitivity = 17f;

    // Update is called once per frame

    private void Awake()
    {
        target = null;
        GetComponent<Camera>().orthographicSize = defaultSize;
        orbitStartTime = -100;
    }
    void Update()
    {
        

        if (target == null)
        {   
                Orbit();
        }
        else
        {
            //rotation around planet with target focused on center
            Vector3 cameraPosition = (planet.transform.position + (target.position - planet.transform.position).normalized*cameraOffset);
            transform.position = Vector3.Lerp(transform.position,cameraPosition,Time.deltaTime* cameraMoveSpeed);

            //rotate to look at position
            Quaternion targetRotation = Quaternion.LookRotation(planet.transform.position-transform.position);
            targetRotation = Quaternion.Euler(new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, .0f));

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * cameraRotateSpeed);

            //zoom in
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize,zoomedSize,Time.deltaTime);

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

        //if pressing the mouse and didnt just get in orbit pan planet
        if (Input.GetMouseButton(0)&&Time.time - orbitStartTime > .2){
            rotYAxis = transform.eulerAngles.y + Input.GetAxis("Mouse X") * speed;
            rotXAxis = transform.eulerAngles.x - Input.GetAxis("Mouse Y") * speed;
        }
        else
        {
            rotYAxis = transform.eulerAngles.y;
            rotXAxis = transform.eulerAngles.x;
        }

        // convert it to quaternions
        Quaternion toRotation = Quaternion.Euler(rotXAxis, rotYAxis, 0);
        Quaternion rotation = toRotation;

        //figure out what your distance should be (so that it's rotating around not just rotating)
        Vector3 negDistance = new Vector3(0, 0, -distance);
        Vector3 position = rotation * negDistance + planet.transform.position;
        transform.rotation = rotation;

        //just entered orbit: smooth
        if (Time.time - orbitStartTime < .2) transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime*20f);
        else transform.position = position;


        //zoom out
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, defaultSize, Time.deltaTime*2f);


    }

}
