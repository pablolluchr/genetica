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
    public float panningStartTime;
    public CameraState cameraState;

    float oldMoveX;
    float oldMoveY;

    private void Start()
    {
        target = null;
        GetComponent<Camera>().orthographicSize = defaultSize;
        panningStartTime = -Mathf.Infinity;
        cameraState = CameraState.Panning;

        //vector from where I am to what I'm rotating around
        Vector3 initialVector = transform.position - planet.transform.position;

        //current angles of camera (make sure it's looking at what you want
        //to rotate around)
        float rotYAxis = transform.eulerAngles.y;
        float rotXAxis = transform.eulerAngles.x;

        //distance between me and what I'm rotating around
        distance = Vector3.Magnitude(initialVector);
    }


    public void StartPanning()
    {
        cameraState = CameraState.Panning;
        panningStartTime = Time.time;


    }

    public void StartFollowing(Transform _target) {

        cameraState = CameraState.Following;
        target = _target;
    }


    
    void Update()
    {
        if (cameraState==CameraState.Panning)
        {
            Pan();
        }
        else if (cameraState == CameraState.Following)
        {
            FollowTarget();
        }

    }
    


    //You can probably call this in update I just call it from a dif script
    public void Pan()
    {
        if (Input.GetMouseButton(0))
        {
            oldMoveX = Input.GetAxis("Mouse X");
            oldMoveY = Input.GetAxis("Mouse Y");

        }
        //if pressing the mouse and didnt just get in orbit pan planet
        if (Input.GetMouseButton(0)&&Time.time - panningStartTime > .1f){
            rotYAxis = transform.eulerAngles.y + Input.GetAxis("Mouse X") * speed;
            rotXAxis = transform.eulerAngles.x - Input.GetAxis("Mouse Y") * speed;
        }else if (!Input.GetMouseButton(0))
        {
            oldMoveX = Mathf.Lerp(oldMoveX,.0f,Time.deltaTime);
            oldMoveY =Mathf.Lerp(oldMoveY, .0f, Time.deltaTime);
            rotYAxis = transform.eulerAngles.y + oldMoveX * speed;
            rotXAxis = transform.eulerAngles.x - oldMoveY * speed;
        }
        else
        {
            rotYAxis = transform.eulerAngles.y;
            rotXAxis = transform.eulerAngles.x;
        }

        // Clamp rotation to avoid jiterry on the poles
        if (rotXAxis < 180)
        {
            rotXAxis = Mathf.Min(rotXAxis, 90f);
        }
        else
        {
            rotXAxis = Mathf.Max(rotXAxis, 270f);

        }
        //rotXAxis = Mathf.Min(Mathf.Max(rotXAxis, 80f);
        //if (rotXAxis < 0) rotXAxis = 360 + rotXAxis; // e.g. 360 + -40 = 320 which is the same rotation
        Quaternion toRotation = Quaternion.Euler(rotXAxis, rotYAxis, 0);
        Quaternion rotation = toRotation;

        //figure out what your distance should be (so that it's rotating around not just rotating)
        Vector3 negDistance = new Vector3(0, 0, -distance);
        Vector3 position = rotation * negDistance + planet.transform.position;
        transform.rotation = rotation;


        //just entered orbit: smooth
        if (Time.time - panningStartTime < .1) transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime*20f);
        else transform.position = position;


        //zoom out
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, defaultSize, Time.deltaTime*2f);


    }

    public void FollowTarget()
    {
        //rotation around planet with target focused on center
        Vector3 cameraPosition = (planet.transform.position + (target.position - planet.transform.position).normalized * cameraOffset);
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraMoveSpeed);

        //rotate to look at position
        Quaternion targetRotation = Quaternion.LookRotation(planet.transform.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, .0f));

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * cameraRotateSpeed);

        //zoom in
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomedSize, Time.deltaTime);
    }


 

}

public enum CameraState
{
    Panning,
    Following
}
