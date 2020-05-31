using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [System.NonSerialized] public Transform target;
    [SerializeField] private float desktopSpeed = 5f;
    [SerializeField] private float freeRotationDamper = 0.1f;
    public float distanceToPlanetCenter;
    private float rotXAxis;
    private float rotYAxis;
    [SerializeField] private float defaultSize= 7.5f;
    [SerializeField] private float zoomedInSize=5.5f;
    [SerializeField] private float zoomedOutSize = 9.5f;
    public float cameraOffset;
    [SerializeField] private float cameraMoveSpeed =10f;
    public float panningStartTime;
    public CameraState cameraState;
    public float sceneWidth=10f;

    private Queue<float> oldXSpeed;
    private Queue<float> oldYSpeed;

    private float averageOldXSpeed=0f;
    private float averageOldYSpeed=0f;

    public string zoomType;

    private void Start()
    {
        target = null;
        GetComponent<Camera>().orthographicSize = defaultSize;
        panningStartTime = -Mathf.Infinity;
        cameraState = CameraState.Panning;
        oldXSpeed= new Queue<float>(3);
        oldYSpeed= new Queue<float>(3);
        distanceToPlanetCenter = Vector3.Magnitude(transform.position);
    }


    public void StartPanning()
    {
        cameraState = CameraState.Panning;
        panningStartTime = Time.time;
    }

    public void StartFollowing(Transform _target,string zoomType) {

        this.zoomType = zoomType;
        cameraState = CameraState.Following;
        target = _target;
    }

    void Update()
    {
        if (cameraState==CameraState.Panning)
        {
            Pan();
        }
        
        //TODO: stop moving if target didnt change position (performance improvement for static object)

        if (cameraState == CameraState.Following)
            FollowTarget(Time.deltaTime);



    }



    private Vector3 FollowTargetPosition()
    {
        return Vector3.Normalize(target.position) * cameraOffset;
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
        //TODO: make sure desktop-relevant code is only running on desktop.
        //TODO:Check if the device running this is a desktop
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

        transform.position = - (transform.forward).normalized * distanceToPlanetCenter;
        if( zoomType=="out")
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomedOutSize, Time.deltaTime * 3f);
        else
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, defaultSize, Time.deltaTime * 3f);


    }

    

    public void FollowTarget(float timeMultiplier)
    {
        //rotation around planet with target focused on center

        Vector3 cameraPosition = FollowTargetPosition();


        //if ((cameraPosition-transform.position).magnitude>1f)
        transform.position=Vector3.Lerp(transform.position, cameraPosition, timeMultiplier * cameraMoveSpeed);

        //rotate to look at position
        Quaternion targetRotation = Quaternion.LookRotation( - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, .0f));

        transform.rotation = targetRotation;


        //zoom in
        if (zoomType=="in")
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomedInSize, Time.deltaTime*3f);
        else
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomedOutSize, Time.deltaTime * 3f);
    }


 

}

public enum CameraState
{
    Panning,
    Following
}
