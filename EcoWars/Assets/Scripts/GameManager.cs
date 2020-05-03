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
UnitSelection --> FreeSelection : dragging
UnitSelection --> UnitSelection : click on unit
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
    public GameObject selectedObject;
    public Vector3 selectedPoint;
    
    public static GameManager gameManager;
    public CameraController cameraController;
    public GameObject units;
    public GameObject unitPrefab;
    public GameObject areaGraphic;
    public GameObject planet;
    public GameObject sun;

    public Unit selectedUnit;
    
    public GMState gameState;
    public GameObject attributePanel;
    public GameObject bottomControls;
    public GameObject speciesSelectionPanel;

    public List<Species> speciesList = new List<Species>();

    // to update a species first change the attributes in the species class using GetSpecies("Tall").speed = 100 etc.
    // then set this attribute (recall species) to the species name (i.e. "Tall") to recall all pets
    public string recallSpecies = null;

    public float lastMouseX;
    public float lastMouseY;
    public float shortClickDuration=.3f;
    public bool isDragging;
    public bool isShortClick;
    public bool wasDraggingInPrevFrame;
    public bool wasButtonDown;
    public string selectedSpecies;
    public string previousSelectedSpecies;

    private void Awake()
    {
        if (gameManager != null && gameManager != this) {
            Destroy(this.gameObject);
        }
        else {
            gameManager = this;
        }

        gameState = GMState.FreeSelection;

        lastMouseX = Mathf.Infinity;
        lastMouseY = Mathf.Infinity;
        selectedSpecies = null;
        previousSelectedSpecies = null;

        AddSpecies("Tall", 1.5f, 0.6f, 0.2f, 0.2f,new Vector3(-0.09f, 5.48f, -2.99f), 2f,"Pet");
        AddSpecies("Fast", 3f,   0.2f, 0.2f, 0.2f,new Vector3(0, -4f, 4f),          2f,"Hostile");

        GetSpecies("Tall").Spawn(unitPrefab);
        GetSpecies("Tall").Spawn(unitPrefab);
        GetSpecies("Tall").Spawn(unitPrefab);
        GetSpecies("Tall").Spawn(unitPrefab);
        GetSpecies("Tall").Spawn(unitPrefab);

        GetSpecies("Fast").Spawn(unitPrefab);
        GetSpecies("Fast").Spawn(unitPrefab);
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
    
    void Update() {
        gameState = GameManagerStateMachine.NextState();
    }

    #region ACTIONS ##############################################################################

    public void SetTargetsToNull() {
        selectedObject = null;
        selectedPoint = Vector3.zero;
    }

    public void TargetUnit() {
        UnitActions.DisableAreaGraphics();
        UnitActions.DisableAllSelectionGraphics();
        UnitActions.EnableSelectionGraphic(selectedObject.GetComponent<Unit>());
        cameraController.StartFollowing(selectedObject.transform);
        selectedUnit = selectedObject.GetComponent<Unit>();
    }

    public void OverrideUnit() {
        if (selectedUnit.tag == "Pet") UnitActions.OverrideTarget(selectedUnit, selectedPoint);
        UnitActions.DisableAllSelectionGraphics();
        FreePan();
    }

    public void TargetObject() {
        cameraController.StartFollowing(selectedObject.transform);
    }

    public void FreePan() {
        cameraController.StartPanning();
        SetTargetsToNull();
    }

    public void SelectSpecies() {
        previousSelectedSpecies = selectedSpecies;
        Species species = GetSpecies(selectedSpecies);
        UnitActions.DisableAllSelectionGraphics();
        UnitActions.EnableAreaGraphics(GetSpecies(selectedSpecies));
        cameraController.StartMoveToLocation(GetSpecies(selectedSpecies).areaCenter);
        SetTargetsToNull();
    }

    public void DeselectSpecies() {
        selectedSpecies = null;
        previousSelectedSpecies = null;
        UnitActions.DisableAllSelectionGraphics();
        UnitActions.DisableAreaGraphics();
        FreePan();
    }

    public void SetHabitat() {
        Species species = GetSpecies(selectedSpecies);
        if (species.tag == "Hostile") {
            DeselectSpecies();
            return;
        }
        species.areaCenter = selectedPoint;
        species.UpdateAllUnits();
        Unit[] units = species.GetAllUnits();
        foreach (Unit unit in units) {
            UnitActions.OverrideTarget(unit, species.areaCenter);
        }
        SelectSpecies();
        SetTargetsToNull();
    }

    #endregion

    #region QUERIES ##############################################################################

    public bool IsUnitSelected() {
        return selectedObject && selectedObject.GetComponent<Unit>() != null;
    }

    public bool IsObjectSelected() {

        return selectedObject &&  (
            selectedObject.GetComponent<Food>() != null ||
            selectedObject.gameObject.tag == "Base" ||
            selectedObject.gameObject.tag == "Genetium"
        );
    }

    public bool PointSelected() {
        return selectedPoint != Vector3.zero;
    }

    #endregion

}

