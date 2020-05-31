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
    public Transform objectPreview;
    public GameObject speciesSelectionPanel;

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

    //lists of units and objects
    public List<Unit> petList;
    public List<Unit> enemyList;
    public List<Unit> previewUnitList;
    public List<GameObject> waterList;
    public List<GameObject> foodList;
    public List<GameObject> genetiumList;

    public List<Material> furMaterials;




    //UI stuff
    public GameObject unitInfoPanel;
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
        AddSpecies("Fast", "red",2f,   0.2f, 0.2f, 0f,new Vector3(0, -4f, 4f),          5f,"Pet", 0.7f);
        AddSpecies("Tall", "blue", 1.5f, 0.2f, 0.2f, 0.2f, new Vector3(0, 4f, -4f), 3f, "Pet", 0.7f);
        //AddSpecies("Enemy", "blue", 1.5f, 0.2f, 0.2f, 0.2f, new Vector3(0, 4f, -4f), 3f, "Hostile", 0.7f);
        //AddSpecies("FastEnemy", 1.5f, 0.2f, 0.2f, 0.2f, new Vector3(0, -4f, 4f), 2f, "Hostile", 0.7f);

        for (int i = 0; i < 10; i++) GetSpeciesFromName("Fast").Spawn(unitPrefab);
        for (int i = 0; i < 10; i++) GetSpeciesFromName("Tall").Spawn(unitPrefab);
        //for (int i = 0; i < 5; i++) GetSpeciesFromName("Enemy").Spawn(unitPrefab);



    }

    public void AddSpecies(string name,
        string color,
        float speed,
        float legsLength,
        float bodySize,
        float headSize,
        Vector3 areaCenter,
        float areaRadius,
        string tag,
        float swimVsWalk
    ) {
        Species newSpecies = new Species(name,color, speed, legsLength, bodySize, headSize, areaCenter, areaRadius, tag, swimVsWalk);
        speciesList.Add(newSpecies);
        //instantiate species graphic
        GameObject areaGraphicInstance = MonoBehaviour.Instantiate(areaGraphic);
        PositionAreaGraphic(areaGraphicInstance, newSpecies);

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
        return selectedUnit != selectedObject.GetComponent<Unit>();
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

    public void TargetUnit() {
        cameraController.StartFollowing(selectedObject.transform,"in");
        selectedUnit = selectedObject.GetComponent<Unit>();
    }

    public void SetTargetUnitGraphics()
    {
        //UnitActions.DisableAreaGraphics();
        UnitActions.DisableAllSelectionGraphics();
        UnitActions.EnableSelectionGraphic(selectedObject.GetComponent<Unit>());
        unitInfoPanel.GetComponent<UnitInfo>().Show(selectedObject.GetComponent<Unit>());
    }
    public void HideInfoPanel()
    {
        unitInfoPanel.GetComponent<UnitInfo>().Hide();
        forceUnitSelectionExit = false;
    }

    public void PositionAreaGraphic(GameObject areaGraphic,Species species)
    {
        areaGraphic.SetActive(true);
        areaGraphic.GetComponent<AreaGraphic>().SetSpecies(species);
    }


    public void OverrideUnit() {
        if (selectedUnit.CompareTag("Pet")) {
            UnitActions.OverrideTarget(selectedUnit, selectedPoint);
            UnitActions.ShowTargetGraphic(selectedUnit);
            UnitActions.DisableAllSelectionGraphics();
        }
        FreePan();
    }

    public void ForceSelectionExit()
    {
        FreePan();
        HideInfoPanel();
        UnitActions.DisableAllSelectionGraphics();

        SetTargetsToNull();
    }

    public void TargetObject() {
        cameraController.StartFollowing(selectedObject.transform,"in");
    }

    public void FreePan() {
        cameraController.StartPanning();
        if (!IsAreaSelected())
            SetTargetsToNull();
    }

    public void SelectSpecies() {

        AreaGraphic areaGraphic = selectedObject.GetComponent<AreaGraphic>();
        //selectedObject.transform.Find("Image").GetComponent<Animator>().SetBool("isSelected", true);

        selectedSpecies = areaGraphic.species;
        areaGraphic.SelectArea();
        cameraController.StartFollowing(areaGraphic.gameObject.transform, "out");
        UnitActions.SelectAllUnitsOfSpecies(selectedSpecies);

        //SetTargetsToNull();
        //previousSelectedSpecies = selectedSpecies;

    }

    public bool NewSpeciesSelected()
    {
        AreaGraphic areaGraphic = selectedObject.GetComponent<AreaGraphic>();
        return selectedSpecies != areaGraphic.species;
    }

    public void DeselectSpecies() {

        //selectedObject.transform.Find("Image").GetComponent<Animator>().SetBool("isSelected", false);

        selectedSpecies = null;
        previousSelectedSpecies = null;
        AreaGraphic areaGraphic = selectedObject.GetComponent<AreaGraphic>();

        areaGraphic.DeselectArea();

        UnitActions.DisableAllSelectionGraphics();
        SetTargetsToNull();
        FreePan();

        cameraController.zoomType = "in";
    }

    public void SetHabitat() {
        if (selectedSpecies.tag == "Hostile"){
            DeselectSpecies();
            return;
        }
        selectedSpecies.areaCenter = selectedPoint;
        selectedObject.GetComponent<AreaGraphic>().SetSpecies(selectedSpecies);
        selectedSpecies.UpdateAllUnits();
        List<Unit> units = selectedSpecies.GetAllUnitsOfSpecies();
        foreach (Unit unit in units){
            UnitActions.OverrideTarget(unit, selectedSpecies.areaCenter);
        }
        SelectSpecies();
        //SetTargetsToNull();
    }

    #endregion

    #region QUERIES ##############################################################################

    public bool IsUnitSelected() {
        return selectedObject && selectedObject.GetComponent<Unit>() != null;
    }

    public bool IsAreaSelected()
    {
        if (selectedObject != null) return selectedObject.gameObject.tag == "AreaGraphic";
        else return false;

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

