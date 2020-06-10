using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species 
{
    public string speciesName;
    public string color;
    public float headSize;
    public float legSize;
    public float bellySize;
    public float tailSize;
    public float earSize;
    public float armSize;

    public GameObject foodSource;
    public GameObject genetiumSource;
    public GameObject unitPrefab;
    public string tag;

    //todo: this ones are inferred from body attribuets
    public float speed;
    public float swimspeed;
    public float walkspeed;


    //enemy species
    public Species(string name, float speed) {
        this.speciesName = name;
        this.speed = speed;
        this.swimspeed = speed / 2f; //todo calculate!
        this.walkspeed = speed;
        this.tag = "Hostile";
        this.unitPrefab = GameManager.gameManager.enemyPrefab;
        this.foodSource = GameManager.gameManager.foodList[GameManager.gameManager.foodList.Count-1];


    }


    //pet species
    public Species(string name,
        string color,
        float speed, //todo remove
        float headSize,
        float legSize,
        float bellySize,
        float tailSize,
        float earSize,
        float armSize
    ) {
        this.speciesName = name;
        this.color = color;
        this.speed = speed;
        this.legSize = legSize;
        this.bellySize = bellySize;
        this.headSize = headSize;
        this.tailSize = tailSize;
        this.earSize = earSize;
        this.armSize = armSize;
        this.tag = "Pet";
        this.unitPrefab = GameManager.gameManager.unitPrefab;
        this.swimspeed = speed/2f; //todo calculate!
        this.walkspeed = speed;
        this.foodSource = GameManager.gameManager.foodList[0];
    }

    public GameObject Spawn() {
        Vector3 spawnPosition = foodSource.transform.position + Random.onUnitSphere*2f;
        GameObject unit = MonoBehaviour.Instantiate(unitPrefab, spawnPosition,unitPrefab.transform.rotation);
        unit.transform.parent = GameManager.gameManager.units.transform;
        Unit unitComponent = unit.GetComponent<Unit>();
        unitComponent.speciesName = speciesName;
        UpdateUnit(unitComponent);
        UnitActions.DisableSelectionGraphic(unitComponent);

        //if (unitComponent.CompareTag("Preview"))
        //    GameManager.gameManager.previewUnitList.Add(unitComponent);
        if (unitComponent.CompareTag("Pet"))
            GameManager.gameManager.petList.Add(unitComponent);
        else if (unitComponent.CompareTag("Hostile"))
            GameManager.gameManager.enemyList.Add(unitComponent);

        return unit;
    }

    public void UpdateUnit(Unit unit) { //returns true if succeeds, false otherwise
        //if (unit.speciesName != speciesName) { return; } //cant modify another species

        if (!unit.CompareTag("Hostile")) {
            unit.UpdateHeadSize(headSize);
            unit.UpdateLegSize(legSize);
            unit.UpdateBellySize(bellySize);
            unit.UpdateEarSize(earSize);
            unit.UpdateTailSize(tailSize);
            unit.UpdateArmSize(armSize);
            unit.UpdateFurColor(color);
        }

        if (unit.CompareTag("Preview")) return;

        unit.genetiumSource = genetiumSource;
        unit.foodSource = foodSource;
        unit.speed = speed;
        unit.interactionRadius = UnitHelperFunctions.Interpolate(legSize, new float[,]{{0.2f, 0.5f}, {0.6f, 0.6f}}); //todo: rethink this
        
        //unit.areaCenter = areaCenter;


        //unit.areaRadius = AreaRadiusFromSize();
        unit.gameObject.tag = tag;
        unit.swimspeed = swimspeed;

        if (tag == "Pet")
        {
            unit.enemyTag = "Hostile";
        }
        else if (tag == "Hostile")
        {
            unit.enemyTag = "Pet";
        }
        else
        {
            throw new System.Exception("Wrong tag for unit");
        }
        UnitActions.ResetSelectionGraphicPosition(unit);
        return;

    }

    public void UpdateAllUnits() {

        List<Unit> pets = GameManager.gameManager.petList;
        foreach (Unit pet in pets) {
            if (pet.speciesName != speciesName) continue;
            UpdateUnit(pet);
        }
    }

    //public float AreaRadiusFromSize()
    //{
    //    switch (areaSize)
    //    {
    //        case 0: return 0f;
    //        case 1: return 3f;
    //        case 2: return 6f;
    //        case 3: return 8.5f;
    //        case 4: return 10f;
    //        case 5: return 13f;
    //        default:
    //            throw new System.Exception("Size not defined");
    //    }
    //}

    public List<Unit> GetAllUnitsOfSpecies() {
        List<Unit> units = new List<Unit>();

        if (tag == "Pet")
        {
            List<Unit> pets = GameManager.gameManager.petList;
            foreach (Unit pet in pets) {
                if (pet.speciesName != speciesName) continue;
                units.Add(pet);
            }

        }
        else
        {
            List<Unit> enemies = GameManager.gameManager.enemyList;
            foreach (Unit enemy in enemies)
            {
                if (enemy.speciesName != speciesName) continue;
                units.Add(enemy);
            }
        }
        return units;
    }
}