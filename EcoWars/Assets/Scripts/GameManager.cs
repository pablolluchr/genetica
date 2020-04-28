using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private GameObject target;
    public float bioFuel;
    public CameraController cameraController;
    private float lastMouseDown;
    public float shortClickDuration=.3f;
    


    private void Awake()
    {
        if (gameManager != null && gameManager != this) Destroy(this.gameObject);
        else gameManager = this;

        lastMouseDown = -100;

    }

    private void Start()
    {
        target = null;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { lastMouseDown = Time.time; }

        //short click
        if (Input.GetMouseButtonUp(0) && Time.time-lastMouseDown<shortClickDuration)
        {

            //check for clicks on units
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                Mathf.Infinity, 1 << LayerMask.NameToLayer("Units"));
            if (hit)
            {

                if (hitInfo.transform.gameObject.tag == "Pet") //pet clicked
                {
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

            if (target != null) //if a pet is being controlled
            {

                //check for clicks on map to override pet's destination. short click
                if (Input.GetMouseButtonUp(0) && Time.time - lastMouseDown < shortClickDuration)
                {
                    hitInfo = new RaycastHit();
                    hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                        Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
                    if (hit)
                    {
                        target.GetComponent<Unit>().OverrideDestination(hitInfo.point);
                    }
                }
            }
        }

        //reset to orbit control if dragging
        if (Input.GetMouseButton(0) && (Time.time - lastMouseDown) > shortClickDuration){
            cameraController.target = null;
            if (Input.GetMouseButtonDown(0)){
                cameraController.orbitStartTime = Time.time;
            }
        }
    }

}

public enum GameState
{
    RotateAround, //using the camera to rotate around the planet
    ControlPet //overriding the controls of the selected pet.
}
