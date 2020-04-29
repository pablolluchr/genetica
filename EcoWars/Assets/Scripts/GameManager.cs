using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private GameObject target;
    public float bioFuel;
    public CameraController cameraController;
    public GameObject dummyTargets;
    public GameObject units;
    public GameState gameState;

    private float lastMouseX;
    private float lastMouseY;
    public float shortClickDuration=.3f;
    private bool isDragging;
    public bool wasButtonDown;


    private void Awake()
    {
        if (gameManager != null && gameManager != this) Destroy(this.gameObject);
        else gameManager = this;

        lastMouseX = Mathf.Infinity;
        lastMouseY = Mathf.Infinity;

        gameState = GameState.RotateAround;


    }

    private void Start()
    {
        target = null;
    }
    void Update()
    {
        RaycastHit hitInfo;
        bool hit;

        //mouse being pressed
        if (Input.GetMouseButton(0))
        {
            //check for dragging behaviour if mouse was pressed on the previous frame in a different position
            // and it's still pressed now

            if (wasButtonDown)
            {
                isDragging = Mathf.Pow(Input.GetAxis("Mouse X") - lastMouseX, 2) +
                Mathf.Pow(Input.GetAxis("Mouse Y") - lastMouseY, 2) > 0f || isDragging;
            }
            
            lastMouseX = Input.GetAxis("Mouse X");
            lastMouseY = Input.GetAxis("Mouse Y");
            wasButtonDown = true;
        }
        else
        {
            wasButtonDown = false;

        }


        //short click
        if (Input.GetMouseButtonUp(0) && !isDragging)
        {

            hitInfo = new RaycastHit();
            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
            if (target != null && hit) //if a pet is being controlled and user hits map override
            {

                UnitActions.OverrideTarget(target.GetComponent<Unit>(), hitInfo.point);
            }

            //check for clicks on other units
            hitInfo = new RaycastHit();
            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,Mathf.Infinity);
            if (hit)
            {

                if (hitInfo.transform.gameObject.tag == "Pet") //pet clicked
                {
                    gameState = GameState.ControlPet;
                    //do stuff with pet
                    target = hitInfo.transform.gameObject;
                    cameraController.target = target.GetComponent<Transform>();
                    Debug.Log("Pet clicked");

                }
                else if (hitInfo.transform.gameObject.tag == "Hostile") //hostile clicked
                {
                    Debug.Log("Hostile clicked");

                }
            }

            
        }

        if (gameState==GameState.ControlPet && Input.GetMouseButtonDown(0))
        {
            cameraController.orbitStartTime = Time.time;
        }

        //reset to orbit control if dragging
        if (isDragging){
            gameState = GameState.RotateAround;
            cameraController.target = null;
            
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

}

public enum GameState
{
    RotateAround, //using the camera to rotate around the planet
    ControlPet //overriding the controls of the selected pet.
}
