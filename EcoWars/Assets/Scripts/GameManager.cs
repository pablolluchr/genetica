using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private Unit controlledPet;
    public float bioFuel;
    public CameraController cameraController;

    private void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManager = this;
        }
    }

    private void Start()
    {
        controlledPet = null;
    }
    void Update()
    {
        //check for clicks on units
        if (Input.GetMouseButtonDown(0))
        {

            //check for clicks on units
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                Mathf.Infinity, 1 << LayerMask.NameToLayer("Units"));
            if (hit)
            {
            
                if (hitInfo.transform.gameObject.tag == "Pet") //pet clicked
                {
                    controlledPet = hitInfo.transform.GetComponent<Unit>();
                    //do stuff with pet
                    Debug.Log("Pet clicked");
                }
                else if (hitInfo.transform.gameObject.tag == "Hostile") //hostile clicked
                {
                    Debug.Log("Hostile clicked");

                }
            }

            if (controlledPet != null) //if a pet is being controlled
            {

                //check for clicks on map to override pet's destination
                //TODO: avoid moving pet if mouse is being held (rotating around planet). Only fast clicks update position
                if (Input.GetMouseButtonDown(0))
                {
                hitInfo = new RaycastHit();
                hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                    Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
                if (hit)
                {
                        controlledPet.OverrideDestination(hitInfo.point);
                }

                }

            }
        }
    }

}

public enum GameState
{
    RotateAround, //using the camera to rotate around the planet
    ControlPet //overriding the controls of the selected pet.
}
