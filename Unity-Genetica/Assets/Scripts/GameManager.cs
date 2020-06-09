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
    [Header("Game state")]
    public int countsBetweenUpdates = 15;
    public float planetRadius;
    public float wanderingRadius;
    public int maxUnits;
    [HideInInspector] public GMState gameState;
    [HideInInspector] public GameObject selectedObject;
    [HideInInspector] public Vector3 selectedPoint;
    [HideInInspector] public Unit selectedUnit;
    [HideInInspector] public string recallSpecies = null;
    [HideInInspector] public bool isDragging;
    [HideInInspector] public bool isShortClick;
    [HideInInspector] public Species selectedSpecies;
    [HideInInspector] public Species previousSelectedSpecies;
    [HideInInspector] public bool newSpeciesSelected;
    [HideInInspector] public bool forcePanelExit;
    [HideInInspector] public bool openSpeciesAttributes;


    [Header("Object references")]
    public static GameManager gameManager;
    public Transform objectPreview;
    public CameraController cameraController;
    public GameObject units;
    public Material unitFur;
    public GameObject unitPrefab;
    public GameObject sun;
    public List<Species> speciesList = new List<Species>();
    public List<Unit> petList;
    public List<Unit> enemyList;
    public List<Unit> previewUnitList;
    public MeshFilter water;
    public List<GameObject> foodList;
    public List<GameObject> genetiumList;
    public List<Material> furMaterials;

    [Header("UI stuff")]
    public UnitInfoPanel unitInfoPanel;
    public SpeciesInfoPanel speciesInfoPanel;
    public FoodInfoPanel foodInfoPanel;
    public GenetiumInfoPanel genetiumInfoPanel;
    public BaseInfoPanel baseInfoPanel;
    public SpeciesSelectionPanel speciesSelectionPanel;
    public AttributePanel speciesAttributePanel;



    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (gameManager != null && gameManager != this) Destroy(this.gameObject);
        else gameManager = this;


        gameState = GMState.FreeSelection;

        selectedSpecies = null;
        previousSelectedSpecies = null;
        forcePanelExit = false;
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

    public void SetSpeciesAttributes(bool newValue) {
        openSpeciesAttributes = newValue;
    }

    public void ShowSpeciesAttributes() {
        forcePanelExit = false;
        openSpeciesAttributes = false;
        speciesAttributePanel.Show(selectedUnit.GetComponent<Unit>().speciesName);
    }

    public void HideSpeciesSelectionPanel() {
        speciesSelectionPanel.Hide();
    }


    public void SelectUnit() {
        forcePanelExit = false;
        cameraController.StartFollowing(selectedObject.transform,"in");
        selectedUnit = selectedObject.GetComponent<Unit>();
        UnitActions.EnableSelectionGraphic(selectedUnit);
        unitInfoPanel.Show(selectedUnit);
    }

    public void DeselectUnit()
    {
        unitInfoPanel.Hide();
        UnitActions.DisableAllSelectionGraphics();
    }
    public void HideObjectSelection() {
        foodInfoPanel.Hide();
        genetiumInfoPanel.Hide();
        baseInfoPanel.Hide();
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
        forcePanelExit = false;
        cameraController.StartFollowing(selectedObject.transform,"in");

        if (selectedObject.CompareTag("Food")) foodInfoPanel.Show();
        if (selectedObject.CompareTag("Genetium")) genetiumInfoPanel.Show();
        if (selectedObject.CompareTag("Base")) baseInfoPanel.Show();

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
        //SetTargetsToNull();
        FreePan();

        cameraController.zoomType = "in";
        speciesInfoPanel.Hide();
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
            selectedObject.CompareTag("Food") ||
            selectedObject.CompareTag("Base")||
            selectedObject.CompareTag("Genetium")
        );
    }

    public bool PointSelected() {
        return selectedPoint != Vector3.zero;
    }

    #endregion

}

