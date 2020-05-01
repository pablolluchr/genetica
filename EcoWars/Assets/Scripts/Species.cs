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
    public Species(string name, float speed, float legsLength, float bodySize, float headSize,Vector3 areaCenter, float areaRadius,string tag) {
        this.speciesName = name;
        this.speed = speed;
        this.legsLength = legsLength;
        this.bodySize = bodySize;
        this.headSize = headSize;
        this.areaCenter = areaCenter;
        this.tag = tag;
        this.areaRadius = areaRadius;

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
        unit.legsLength = legsLength;
        unit.interactionRadius = 0.5f + 0.8f * (legsLength - 0.2f);
        unit.bodySize = bodySize;
        unit.headSize = headSize;
        unit.areaCenter = areaCenter;
        unit.areaRadius = areaRadius;
        unit.gameObject.tag = tag;
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

    public List<Unit> UpdateAllUnitsOfSpecies()
    {
        List<Unit> unitsOfSpecies = new List<Unit>();
        Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
        foreach (var unit in allUnits)
        {
            if (updateUnit(unit)) unitsOfSpecies.Add(unit);

            

        }
        return unitsOfSpecies;
    }
}