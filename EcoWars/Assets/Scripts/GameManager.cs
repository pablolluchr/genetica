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

    private void Start()
    {
        if (gameManager != null && gameManager != this) Destroy(this.gameObject);
        else gameManager = this;

        lastMouseX = Mathf.Infinity;
        lastMouseY = Mathf.Infinity;
        target = null;

    }
    void Update()
    {
        RaycastHit hitInfo;
        bool hit;
        SetDraggingState();
        if (target == null && gameState==GameState.Following) {
            gameState = GameState.Panning;
            cameraController.StartPanning();
        } 

        //SHORT CLICK
        if (Input.GetMouseButtonUp(0) && !isDragging)
        {
            hitInfo = new RaycastHit();
            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,Mathf.Infinity,
                1 << LayerMask.NameToLayer("Units"));

            if (hit)//unit clicked
            {

                if (hitInfo.transform.parent.tag == "Pet" || hitInfo.transform.tag == "Pet")
                {
                    Debug.Log("Pet clicked");

                    cameraController.StartFollowing(hitInfo.transform);

                    //unit clicked is a new target
                    if (target != hitInfo.transform.gameObject)
                    {
                        //disable last selection graphic
                        if (target != null) UnitActions.DisableSelectionGraphic(target.GetComponent<Unit>());

                        target = hitInfo.transform.gameObject;

                        UnitActions.EnableSelectionGraphic(target.GetComponent<Unit>());
                        gameState = GameState.Following;
                    }

                }
                else if (hitInfo.transform.parent.tag == "Hostile")  Debug.Log("Hostile clicked");
            }
            else //check for clicks on planet
            {
                hitInfo = new RaycastHit();
                hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                    Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));

                //if a pet is being controlled and user hits map: override
                if (gameState == GameState.Following && hit)
                    UnitActions.OverrideTarget(target.GetComponent<Unit>(), hitInfo.point);
            }

            
        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;

        if (isDragging && gameState==GameState.Following)
        {
            gameState = GameState.Panning;
            cameraController.StartPanning();
        }
    }


    //check for dragging behaviour if mouse was pressed on the previous frame in a different position
    // and it's still pressed now
    public void SetDraggingState()
    {
        if (Input.GetMouseButton(0))
        {
            

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
    }

}

public enum GameState
{
 
    Panning, //not following anything. 
    Following//only able to exit when unit dies
}

