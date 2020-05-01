using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planet;
    [System.NonSerialized] public Transform target;
    [SerializeField] private float desktopSpeed = 5f;
    [SerializeField] private float phoneSpeed = 0.3f;
    [SerializeField] private float freeRotationDamper = 0.1f;
    public float distanceToPlanetCenter;
    private float rotXAxis;
    private float rotYAxis;
    [SerializeField] private float defaultSize= 7.5f;
    [SerializeField] private float zoomedSize=5.5f;
    [SerializeField] private float movingToPositionSize=9.5f;
    public float cameraOffset;
    [SerializeField] private float cameraMoveSpeed =10f;
    public float panningStartTime;
    public CameraState cameraState;
    private Vector3 targetPosition;
    private float moveToLocationSpeed;
    private float speed;

    float oldXSpeed;
    float oldYSpeed;

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
        distanceToPlanetCenter = Vector3.Magnitude(initialVector);
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

    public void StartMoveToLocation(Vector3 position)
    {

        cameraState = CameraState.MoveToLocation;
        targetPosition = position;
    }

    private void FixedUpdate()
    {
        if (cameraState == CameraState.Following)
        {
            FollowTarget();
        }
    }

    void Update()
    {
        if (cameraState==CameraState.Panning)
        {
            Pan();
        }
        else if(cameraState == CameraState.MoveToLocation)
        {

            MoveToLocation();
        }
        

    }
    public void ResetMovingToSpeed()
    {
        moveToLocationSpeed = 30;
    }



    public void MoveToLocation() {

        //initial speed to avoid crossing through poles
        moveToLocationSpeed = Mathf.Max(moveToLocationSpeed - Time.deltaTime*50, 0);

        //if target is very far start spinning to avoid crossing through ppoles
        if ((new Vector2(targetPosition.x, targetPosition.z).normalized +
            new Vector2(transform.position.x, transform.position.z).normalized).magnitude < 0.8f){
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.right, Time.deltaTime * moveToLocationSpeed);
        }
        
        //rotation around planet with target focused on center
        Vector3 cameraPosition = (planet.transform.position + (targetPosition - planet.transform.position).normalized * cameraOffset);

        //linear interpolation between positions
        Vector3 newLinearPosition = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraMoveSpeed);
        newLinearPosition = newLinearPosition.normalized * distanceToPlanetCenter;

        //transform.position = new Vector3(transform.position.x, newLinearPosition.y, 0);
        //avoid crossing through the planet

        transform.position = newLinearPosition;

        //rotate to look at center of planet
        Quaternion targetRotation = Quaternion.LookRotation(planet.transform.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, .0f));

        transform.rotation = targetRotation;


        //zoom out
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, movingToPositionSize, Time.deltaTime * 3f);
    }

    //clamp axis to avoid being close to poles
    public float ClampRotationAxis(float axis)
    {
        if (axis < 180)
        {
            axis = Mathf.Min(axis, 70f);
        }
        else
        {
            axis = Mathf.Max(axis, 290f);

        }
        return axis;
    }




    public void Pan()
    {
        //Check if the device running this is a desktop
        float xSpeed = Input.GetAxis("Mouse X")/desktopSpeed;
        float ySpeed = Input.GetAxis("Mouse Y")/ desktopSpeed;
        if (Input.touchCount ==1)
        {

            if (Input.touches[0].deltaPosition.magnitude < 10000)
            {
                xSpeed = Input.touches[0].deltaPosition.x/phoneSpeed;
                ySpeed = Input.touches[0].deltaPosition.y/phoneSpeed;
            }

        }
        else
        {

        }


        if (Input.GetMouseButton(0) || Input.touchCount>0)
        {
            //get amount of rotation from panning.
            rotYAxis = transform.eulerAngles.y + xSpeed; //new y rotation after input
            rotXAxis = transform.eulerAngles.x - ySpeed; //new x rotation after
            oldXSpeed = xSpeed;
            oldYSpeed = ySpeed;

        }
        else
        {

            oldXSpeed = Mathf.Lerp(oldXSpeed, .0f, Time.deltaTime * freeRotationDamper);
            oldYSpeed = Mathf.Lerp(oldYSpeed, .0f, Time.deltaTime *freeRotationDamper);

            rotYAxis = transform.eulerAngles.y + oldXSpeed;
            rotXAxis = transform.eulerAngles.x - oldYSpeed;

        }


        

        rotXAxis = ClampRotationAxis(rotXAxis);

        //update rotation according to mouse input

        transform.rotation = Quaternion.Euler(rotXAxis, rotYAxis, 0f);

        transform.position = planet.transform.position - (transform.forward).normalized * distanceToPlanetCenter;
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, defaultSize, Time.deltaTime * 3f);
        


        //Vector3 cameraToPlanet = planet.transform.position - transform.position;
        //float distanceToPlanet = cameraToPlanet.magnitude;
        //Vector3 cameraPointing = transform.rotation.eulerAngles;

        ////move camera so that it looks to planet

        //Vector3 negDistance = new Vector3(0, 0, -distance);
        //Vector3 position = rotation * negDistance + planet.transform.position;
        //transform.rotation = rotation;


        //if (Input.GetMouseButton(0))
        //{
        //    oldMoveX = Input.GetAxis("Mouse X");
        //    oldMoveY = Input.GetAxis("Mouse Y");

        //}
        ////if pressing the mouse and didnt just get in orbit pan planet
        //if (Input.GetMouseButton(0)&&Time.time - panningStartTime > .1f){
        //    rotYAxis = transform.eulerAngles.y + Input.GetAxis("Mouse X") * speed;
        //    rotXAxis = transform.eulerAngles.x - Input.GetAxis("Mouse Y") * speed;
        //}else if (!Input.GetMouseButton(0))
        //{
        //    oldMoveX = Mathf.Lerp(oldMoveX,.0f,Time.deltaTime);
        //    oldMoveY =Mathf.Lerp(oldMoveY, .0f, Time.deltaTime);
        //    rotYAxis = transform.eulerAngles.y + oldMoveX * speed;
        //    rotXAxis = transform.eulerAngles.x - oldMoveY * speed;
        //}
        //else
        //{
        //    rotYAxis = transform.eulerAngles.y;
        //    rotXAxis = transform.eulerAngles.x;
        //}

        //// Clamp rotation to avoid jiterry on the poles
        //if (rotXAxis < 180)
        //{
        //    rotXAxis = Mathf.Min(rotXAxis, 90f);
        //}
        //else
        //{
        //    rotXAxis = Mathf.Max(rotXAxis, 270f);

        //}
        ////rotXAxis = Mathf.Min(Mathf.Max(rotXAxis, 80f);
        ////if (rotXAxis < 0) rotXAxis = 360 + rotXAxis; // e.g. 360 + -40 = 320 which is the same rotation
        //Quaternion toRotation = Quaternion.Euler(rotXAxis, rotYAxis, 0);
        //Quaternion rotation = toRotation;

        ////figure out what your distance should be (so that it's rotating around not just rotating)
        //Vector3 negDistance = new Vector3(0, 0, -distance);
        //Vector3 position = rotation * negDistance + planet.transform.position;
        //transform.rotation = rotation;


        ////just entered orbit: smooth
        //if (Time.time - panningStartTime < .1) transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime*20f);
        //transform.position = position;


        ////zoom out
        //GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, defaultSize, Time.deltaTime*2f);


    }

    

    public void FollowTarget()
    {
        //rotation around planet with target focused on center

        Vector3 cameraPosition = (planet.transform.position + (target.position - planet.transform.position).normalized * cameraOffset);

        //transform.position = cameraPosition;

        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraMoveSpeed);

        //rotate to look at position
        Quaternion targetRotation = Quaternion.LookRotation(planet.transform.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, .0f));

        transform.rotation = targetRotation;


        //zoom in
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomedSize, Time.deltaTime*3f);
    }


 

}

public enum CameraState
{
    Panning,
    Following,
    MoveToLocation
}
