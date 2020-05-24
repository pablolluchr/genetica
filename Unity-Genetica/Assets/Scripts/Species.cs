using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species 
{
    public string speciesName;
    public float speed;
    public float legsLength;
    public float bodySize;
    public float headSize;
    public Vector3 areaCenter;
    public float areaRadius;
    public string tag;
    public float swimspeed;
    public float walkspeed;

    public Species(string name,
        float speed,
        float legsLength,
        float bodySize,
        float headSize,
        Vector3 areaCenter,
        float areaRadius,
        string tag,
        float swimVsWalk
    ) {
        this.speciesName = name;
        this.speed = speed;
        this.legsLength = legsLength;
        this.bodySize = bodySize;
        this.headSize = headSize;
        this.areaCenter = areaCenter;
        this.tag = tag;
        this.areaRadius = areaRadius;
        this.swimspeed = (1 - swimVsWalk) / 0.5f;
        this.walkspeed = swimVsWalk / 0.5f;
    }

    public void Spawn(GameObject unitPrefab) {
        Vector3 spawnPosition = areaCenter + Random.onUnitSphere;
        GameObject unit = MonoBehaviour.Instantiate(unitPrefab, spawnPosition,unitPrefab.transform.rotation);
        unit.transform.parent = GameManager.gameManager.units.transform;
        Unit unitComponent = unit.GetComponent<Unit>();
        unitComponent.species = speciesName;
        updateUnit(unitComponent);
    }

    public bool updateUnit(Unit unit) { //returns true if succeeds, false otherwise
        if (unit.species != speciesName) { return false; } //cant modify another species
        unit.speed = speed;
        //unit.legsSize = legsSize;
        unit.interactionRadius = UnitHelperFunctions.Interpolate(legsLength, new float[,]{{0.2f, 0.5f}, {0.6f, 0.6f}});
        //unit.bodySize = bodySize;
        //unit.headSize = headSize;
        unit.areaCenter = areaCenter;
        unit.areaRadius = areaRadius;
        unit.gameObject.tag = tag;
        unit.swimspeed = swimspeed;
        unit.walkspeed = walkspeed;

        //TODO:update models

        if (tag == "Pet")
        {
            unit.enemyTag = "Hostile";
            unit.selectionColor = new Color(0, 1, 0, 0.4f);
        }
        else if (tag == "Hostile")
        {
            unit.enemyTag = "Pet";
            unit.selectionColor = new Color(0, 0, 0, 0f);

        }
        else
        {
            throw new System.Exception("Wrong tag for unit");
        }
        UnitActions.ResetSelectionGraphicPosition(unit);
        return true;

    }

    public void UpdateAllUnits() {
        GameObject[] unitObjects = GameObject.FindGameObjectsWithTag("Pet");
        foreach (GameObject unitObject in unitObjects) {
            Unit unit = unitObject.GetComponent<Unit>();
            if (unit.species != speciesName) continue;
            updateUnit(unit);
        }
    }

    public Unit[] GetAllUnits() {
        List<Unit> units = new List<Unit>();
        GameObject[] unitObjects = GameObject.FindGameObjectsWithTag("Pet");
        foreach (GameObject unitObject in unitObjects) {
            Unit unit = unitObject.GetComponent<Unit>();
            if (unit.species != speciesName) continue;
            units.Add(unit);
        }
        unitObjects = GameObject.FindGameObjectsWithTag("Hostile");
        foreach (GameObject unitObject in unitObjects) {
            Unit unit = unitObject.GetComponent<Unit>();
            if (unit.species != speciesName) continue;
            units.Add(unit);
        }
        return units.ToArray();
    }
}