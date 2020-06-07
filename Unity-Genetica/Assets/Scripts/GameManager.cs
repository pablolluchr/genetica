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
    public GameObject sun;

    public Unit selectedUnit;
    
    public GMState gameState;
    public GameObject attributePanel;
    public GameObject bottomControls;
    public Transform objectPreview;

    public List<Species> speciesList = new List<Species>();
    public string recallSpecies = null;

    public float lastMouseX;
    public float lastMouseY;
    public float shortClickDuration=.3f;
    public bool isDragging;
    public bool isShortClick;
    public bool wasDraggingInPrevFrame;
    public bool wasButtonDown;
    public Species selectedSpecies;
    public Species previousSelectedSpecies;
    public int countsBetweenUpdates = 15;
    public Material unitFur;
    public float planetRadius;
    public int maxUnits;
    public bool newSpeciesSelected;

    //lists of units and objects
    public List<Unit> petList;
    public List<Unit> enemyList;
    public List<Unit> previewUnitList;
    public MeshFilter water;
    public List<GameObject> foodList;
    public List<GameObject> genetiumList;
    public List<Material> furMaterials;

    //UI stuff
    public UnitInfoPanel unitInfoPanel;
    public SpeciesInfoPanel speciesInfoPanel;
    public SpeciesSelectionPanel speciesSelectionPanel;

    public bool forceUnitSelectionExit;

    private void Awake()
    {
        Application.targetFrameRate = 60;

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
        forceUnitSelectionExit = false;
        AddSpecies("Reds", "Pet", "red", 1, 0, 0, 0, 0, 0, 0);
        AddSpecies("Blues", "Pet", "blue", 2, 0, 0, 0, 0, 0, 0);
        //AddSpecies("Enemy", "blue", 1.5f, 0.2f, 0.2f, 0.2f, new Vector3(0, 4f, -4f), 3f, "Hostile", 0.7f);
        //AddSpecies("FastEnemy", 1.5f, 0.2f, 0.2f, 0.2f, new Vector3(0, -4f, 4f), 2f, "Hostile", 0.7f);

        for (int i = 0; i < 5; i++) GetSpeciesFromName("Reds").Spawn(unitPrefab);
        for (int i = 0; i < 5; i++) GetSpeciesFromName("Blues").Spawn(unitPrefab);
        //for (int i = 0; i < 5; i++) GetSpeciesFromName("Enemy").Spawn(unitPrefab);



    }

    public void AddSpecies(string name,
        string tag,
        string color,
        float speed,
        float headSize,
        float legSize,
        float bellySize,
        float tailSize,
        float earSize,
        float armSize
    ) {
        Species newSpecies = new Species(name,tag, color, speed,
            headSize,legSize,bellySize,tailSize,earSize,armSize);
        speciesList.Add(newSpecies);
        //instantiate species graphic
        //GameObject areaGraphicInstance = MonoBehaviour.Instantiate(areaGraphic);
        //PositionAreaGraphic(areaGraphicInstance, newSpecies);

    }

    

    public void ChangeLayersRecursively(Transform parent, string newName)
    {
        parent.gameObject.layer = LayerMask.NameToLayer(newName);
        foreach (Transform child in parent)
        {
            child.gameObject.layer = LayerMask.NameToLayer(newName);
            ChangeLayersRecursively(child, newName);
        }
    }

    public Species GetSpeciesFromName(string name) {
        foreach (Species species in speciesList) {
            if (species.speciesName == name) {
                return species;
            }
        }

        return null;
    }
    
    void Update() {
        gameState = GameManagerStateMachine.NextState();
    }

    #region ACTIONS ##############################################################################

    public bool NewUnitSelected()
    {
        return selectedObject && selectedObject.GetComponent<Unit>()!=null &&
            selectedUnit != selectedObject.GetComponent<Unit>();
    }

    public void SetHabitatTargets()
    {
        if (selectedObject.CompareTag("Genetium")) selectedSpecies.genetiumSource = selectedObject;
        if (selectedObject.CompareTag("Food")) selectedSpecies.foodSource = selectedObject;
        selectedSpecies.UpdateAllUnits();
    }

    public Material GetFurMaterial(string color)
    {
        switch (color)
        {
            case "blue":
                return furMaterials[0];
            case "red":
                return furMaterials[1];
            default:
                throw new System.Exception("Color not available");
        }

       
    }
    public void SetTargetsToNull() {
        selectedObject = null;
        selectedPoint = Vector3.zero;
    }

    public void ShowSpeciesSelectionPanel()
    {
        speciesSelectionPanel.Show();
    }

    public void SelectUnit() {
        forceUnitSelectionExit = false;
        cameraController.StartFollowing(selectedObject.transform,"in");
        selectedUnit = selectedObject.GetComponent<Unit>();

        UnitActions.DisableAllSelectionGraphics();
        UnitActions.EnableSelectionGraphic(selectedUnit);
        speciesSelectionPanel.Hide();
        speciesInfoPanel.Hide();
        unitInfoPanel.Show(selectedUnit);
    }

    public void DeselectUnit()
    {
        unitInfoPanel.Hide();
        UnitActions.DisableAllSelectionGraphics();
    }

    public void OverrideUnit() {
        if (selectedUnit.CompareTag("Pet")) {
            UnitActions.OverrideTarget(selectedUnit, selectedPoint);
            UnitActions.ShowTargetGraphic(selectedUnit);
            UnitActions.DisableAllSelectionGraphics();
        }
        FreePan();
    }

    public void TargetObject() {
        cameraController.StartFollowing(selectedObject.transform,"in");
    }

    public void FreePan() {
        cameraController.StartPanning();
        SetTargetsToNull();
    }

    public void UpdateSelectedSpecies(Species species)
    {
        newSpeciesSelected = true;
        selectedSpecies = species;
    }
    public void SelectSpecies()
    {
        speciesInfoPanel.Show(selectedSpecies);
        UnitActions.SelectAllUnitsOfSpecies(selectedSpecies);

        //todo: find average point of species units and go there instead?
        cameraController.StartFollowing(selectedSpecies.foodSource.transform, "out");
        newSpeciesSelected = false;
    }

    public void DeselectSpecies() {
        selectedSpecies = null;
        UnitActions.DisableAllSelectionGraphics();
        SetTargetsToNull();
        FreePan();

        cameraController.zoomType = "in";
    }


    #endregion

    #region QUERIES ##############################################################################

    public bool IsUnitSelected() {
        return selectedObject && selectedObject.GetComponent<Unit>() != null;
    }

    public bool IsSpeciesSelected()
    {
        return selectedSpecies != null;
    }

    public bool IsObjectSelected() {

        return selectedObject &&  (
            selectedObject.GetComponent<Food>() != null ||
            
            selectedObject.gameObject.CompareTag("Base")||
            selectedObject.gameObject.CompareTag("Genetium")
        );
    }

    public bool PointSelected() {
        return selectedPoint != Vector3.zero;
    }

    #endregion

}

