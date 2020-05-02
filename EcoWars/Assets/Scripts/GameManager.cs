using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***

http://www.plantuml.com/ state diagram:

@startuml
UnitSelection:

ObjectSelection:

SpeciesSelection:

FreeSelection:

[*] --> FreeSelection
FreeSelection --> UnitSelection : click on unit
FreeSelection --> ObjectSelection : click on object
FreeSelection --> SpeciesSelection : toggle species button

ObjectSelection --> FreeSelection : click on planet
ObjectSelection --> FreeSelection : dragging
ObjectSelection --> UnitSelection : click on unit
ObjectSelection --> SpeciesSelection : toggle species button

UnitSelection --> FreeSelection : override
UnitSelection --> SpeciesSelection : toggle species button

SpeciesSelection --> FreeSelection : untoggle species button
SpeciesSelection --> FreeSelection : click on planet
@enduml

- new file: gameManagerStateMachine
- new file: inputManager: responsible for setting isDragging, selectedGameObjectInThisFrame, selectedPointInThisFrame
    inputManager vars are set to null after every statemachine update by statemachine
- new file: gameManagerActions: functions called by the stateMachine to modify the game

***/



public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private GameObject target;
    public float bioFuel;
    public CameraController cameraController;
    public GameObject dummyTargets;
    public GameObject units;
    public GameObject unitPrefab;
    public GameObject areaGraphic;
    public GameObject planet;
    
    public GameState gameState;
    public List<Species> speciesList = new List<Species>();

    // to update a species first change the attributes in the species class using GetSpecies("Tall").speed = 100 etc.
    // then set this attribute (recall species) to the species name (i.e. "Tall") to recall all pets
    public string recallSpecies = null;

    private float lastMouseX;
    private float lastMouseY;
    public float shortClickDuration=.3f;
    private bool isDragging;
    private bool wasDragging;
    public bool wasButtonDown;
    public string selectedSpecies;
    public string previousSelectedSpecies;

    private void Awake()
    {
        if (gameManager != null && gameManager != this) Destroy(this.gameObject);
        else gameManager = this;

        lastMouseX = Mathf.Infinity;
        lastMouseY = Mathf.Infinity;
        target = null;
        selectedSpecies = null;

        AddSpecies("Tall", 1.5f, 0.6f, 0.2f, 0.2f,new Vector3(-0.09f, 5.48f, -2.99f), 2f,"Pet");
        AddSpecies("Fast", 3f,   0.2f, 0.2f, 0.2f,new Vector3(0, -4f, 4f),          2f,"Pet");



        GetSpecies("Tall").Spawn(unitPrefab);
        GetSpecies("Tall").Spawn(unitPrefab);
        GetSpecies("Tall").Spawn(unitPrefab);

        GetSpecies("Fast").Spawn(unitPrefab);
        GetSpecies("Fast").Spawn(unitPrefab);
        GetSpecies("Fast").Spawn(unitPrefab);

    }

    public void AddSpecies(string name, float speed, float legsLength, float bodySize, float headSize,Vector3 areaCenter, float areaRadius,string tag) {
        speciesList.Add(new Species(name, speed, legsLength, bodySize, headSize, areaCenter, areaRadius,tag));
    }

    public Species GetSpecies(string name) {
        foreach (Species species in speciesList) {
            if (species.speciesName == name) {
                return species;
            }
        }
        Debug.Log("species not found");
        return null;
    }

    void Update()
    {
        if (gameState==GameState.MovingToArea)
        {
            cameraController.StartMoveToLocation(GetSpecies(selectedSpecies).areaCenter);
           

        }
        if (selectedSpecies != null)
        {
            UnitActions.EnableAreaGraphics(GetSpecies(selectedSpecies).areaCenter);
        } else {
            UnitActions.DisableAreaGraphics();
        }

        if (recallSpecies != null) {
            GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
            GameObject[] filteredSpecied = UnitHelperFunctions.FilterSpecies(pets, recallSpecies);
            foreach (GameObject pet in filteredSpecied) {
                pet.GetComponent<Unit>().needsChange = true;
            }
            recallSpecies = null;
        }
        
        RaycastHit hitInfo;
        bool hit;

        SetDraggingState(); //sets isdragging
        if ((target == null && gameState == GameState.Following) ||
            (gameState == GameState.MovingToArea && isDragging))
        {
            gameState = GameState.Panning;
            cameraController.StartPanning();
            //UnitActions.DisableAreaGraphics();
        }

        //stop following unit if species selected
        if (selectedSpecies != null && target != null)
        {
            UnitActions.DisableSelectionGraphic(target.GetComponent<Unit>());
            target = null;
            //select all units of species

        }


        //SHORT CLICK
        bool shortClick = false;
        if (Input.touchCount == 1) shortClick = Input.GetTouch(0).phase == TouchPhase.Ended && !wasDragging;
        else shortClick = Input.GetMouseButtonUp(0) && !isDragging;

        if (shortClick)
        {

            //check first raycast collision
            hitInfo = new RaycastHit();
            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
            //todo: check if ui click and return.


            if (hit)//somehting hit
            {
                //unit hit
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Units"))
                {
                    gameState = GameState.Following;
                    cameraController.StartFollowing(hitInfo.transform);
                    UnitActions.DisableAreaGraphics();


                    //unit clicked is a new target
                    if (target != hitInfo.transform.gameObject)
                    {
                        selectedSpecies = null;
                        //disable last selection graphic
                        if (target != null) UnitActions.DisableSelectionGraphic(target.GetComponent<Unit>());
                        target = hitInfo.transform.gameObject;

                        UnitActions.EnableSelectionGraphic(target.GetComponent<Unit>());
                    }

                }
                else
                {
                    //only focus on planet
                    hitInfo = new RaycastHit();
                    hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                        Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));

                    //if a pet is being controlled and user hits map: override
                    if (target != null && hit)
                    {
                        if (target.tag == "Pet" || target.transform.parent.tag == "Pet")
                            UnitActions.OverrideTarget(target.GetComponent<Unit>(), hitInfo.point);

                    }
                    else if (selectedSpecies != null)
                    {
                        Species species = GetSpecies(selectedSpecies);
                        species.areaCenter=hitInfo.point; //update species
                        
                        List<Unit> unitsOfSpecies = species.UpdateAllUnitsOfSpecies(); //change area

                        //override pets so they follow go to the new area
                        //TODO: INSTEAD OF GOING TO THE AREA CENTER, FORCE A NEW GENERATION OF TARGET AROUND THE AREA.

                        foreach (var unit in unitsOfSpecies)
                        {
                            UnitActions.OverrideTarget(unit, hitInfo.point);
                        }

                        //show area selection animation
                    }
                }
            }


        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;

        if (isDragging && gameState == GameState.Following)
        {
            gameState = GameState.Panning;
            cameraController.StartPanning();
        }

        previousSelectedSpecies = selectedSpecies;
        wasDragging = isDragging;
    }


    //check for dragging behaviour if mouse was pressed on the previous frame in a different position
    // and it's still pressed now
    public void SetDraggingState(){
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved) isDragging = true;
            if (Input.GetTouch(0).phase == TouchPhase.Ended) isDragging = false;
            return;

        }if (Input.GetMouseButton(0))
        {
            if (wasButtonDown)
            {
                isDragging = Mathf.Pow(Input.GetAxis("Mouse X") - lastMouseX, 2) +
                Mathf.Pow(Input.GetAxis("Mouse Y") - lastMouseY, 2) > 0.005f || isDragging;
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
    Following,//only able to exit when unit dies
    MovingToArea
}

