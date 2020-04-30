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
    public Species(string name, float speed, float legsLength, float bodySize, float headSize) {
        this.speciesName = name;
        this.speed = speed;
        this.legsLength = legsLength;
        this.bodySize = bodySize;
        this.headSize = headSize;
    }

    public void Spawn(GameObject unitPrefab) {
        GameObject home = GameObject.FindGameObjectWithTag("Base");
        GameObject unit = MonoBehaviour.Instantiate(unitPrefab, home.transform.position * 2, home.transform.rotation);
        unit.transform.parent = GameManager.gameManager.units.transform;
        Unit unitComponent = unit.GetComponent<Unit>();
        unitComponent.species = speciesName;
        updateUnit(unitComponent, speed, legsLength, bodySize, headSize);
    }

    public void updateUnit(Unit unit, float speed, float legsLength, float bodySize, float headSize) {
        if (unit.species != speciesName) { Debug.Log("Cannot modify another species"); return; }
        unit.speed = speed;
        unit.legsLength = legsLength;
        unit.interactionRadius = 0.5f + 0.8f * (legsLength - 0.2f);
        unit.bodySize = bodySize;
        unit.headSize = headSize;
    }
}