using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planet;
    [System.NonSerialized] public Transform target;
    [SerializeField] private float desktopSpeed = 5f;
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
    public float sceneWidth=10f;

    private Queue<float> oldXSpeed;
    private Queue<float> oldYSpeed;

    private float averageOldXSpeed=0f;
    private float averageOldYSpeed=0f;

    private void Start()
    {
        //float unitsPerPixel = sceneWidth / Screen.width;
        //defaultSize = defaultSize * unitsPerPixel;
        //    / Screen.width* screenScaler;
        //zoomedSize = zoomedSize / Screen.width* screenScaler;
        //movingToPositionSize = movingToPositionSize / Screen.width* screenScaler;
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
        oldXSpeed= new Queue<float>(3);
        oldYSpeed= new Queue<float>(3);

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

    void LateUpdate()
    {
        if (cameraState==CameraState.Panning)
        {
            Pan();
        }
        else if(cameraState == CameraState.MoveToLocation)
        {

            MoveToLocation();
        }
        
        //TODO: stop moving if target didnt change position (performance improvement for static object)

        if (cameraState == CameraState.Following && (FollowTargetPosition() - transform.position).magnitude > 3f)
            FollowTarget(Time.deltaTime);



    }
    private void FixedUpdate()
    {
        if (cameraState == CameraState.Following && (FollowTargetPosition() - transform.position).magnitude <= 3f)
            FollowTarget(Time.fixedDeltaTime);
    }


    public void ResetMovingToSpeed()
    {
        moveToLocationSpeed = 30;
    }

    private Vector3 FollowTargetPosition()
    {
        return planet.transform.position + (target.position - planet.transform.position).normalized * cameraOffset;
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

        //or a phone
        if (Input.touchCount ==1)
        {

            if (Input.touches[0].deltaPosition.magnitude < 10000)
            {
                //TODO: Make speeds relative to the number of pixels on screen
                //xSpeed = Input.touches[0].deltaPosition.x/Time.deltaTime*phoneSpeed;
                //ySpeed = Input.touches[0].deltaPosition.y/Time.deltaTime* phoneSpeed;
                Vector2 speed = GameManager.gameManager.GetComponent<InputManager>().touchSpeed;
                xSpeed = speed.x; ySpeed = speed.y;
            }

        }

        //if phone/touch down
        if (Input.GetMouseButton(0) || Input.touchCount==1)
        {
            //get amount of rotation from panning.
            rotYAxis = transform.eulerAngles.y + xSpeed; //new y rotation after input
            rotXAxis = transform.eulerAngles.x - ySpeed; //new x rotation after

            //handle queue of past speeds
            oldXSpeed.Enqueue(xSpeed);
            oldYSpeed.Enqueue(ySpeed);
            if (oldXSpeed.Count > 3)
            {
                oldXSpeed.Dequeue();
                oldYSpeed.Dequeue();
            }

            //update average speed over the last 3 frames
            if (oldXSpeed.Count > 0)
            {
                float[] oldXArray = oldXSpeed.ToArray();
                foreach (var old in oldXArray) averageOldXSpeed += old;
                averageOldXSpeed = averageOldXSpeed / oldXArray.Length;


                float[] oldYArray = oldYSpeed.ToArray();
                foreach (var old in oldYArray) averageOldYSpeed += old;
                averageOldYSpeed = averageOldYSpeed / oldYArray.Length;
            }

        }
        else
        {

            averageOldXSpeed = Mathf.Lerp(averageOldXSpeed, .0f, Time.deltaTime * freeRotationDamper);
            averageOldYSpeed = Mathf.Lerp(averageOldYSpeed, .0f, Time.deltaTime *freeRotationDamper);

            rotYAxis = transform.eulerAngles.y + averageOldXSpeed;
            rotXAxis = transform.eulerAngles.x - averageOldYSpeed;

        }
        

        rotXAxis = ClampRotationAxis(rotXAxis);

        //update rotation according to mouse input

        transform.rotation = Quaternion.Euler(rotXAxis, rotYAxis, 0f);

        transform.position = planet.transform.position - (transform.forward).normalized * distanceToPlanetCenter;
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, defaultSize, Time.deltaTime * 3f);
        

    }

    

    public void FollowTarget(float timeMultiplier)
    {
        //rotation around planet with target focused on center

        Vector3 cameraPosition = FollowTargetPosition();


        //if ((cameraPosition-transform.position).magnitude>1f)
        transform.position=Vector3.Lerp(transform.position, cameraPosition, timeMultiplier * cameraMoveSpeed);

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
