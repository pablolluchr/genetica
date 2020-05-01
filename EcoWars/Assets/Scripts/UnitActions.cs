using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitActions {
    //Unit behaviour functions that change values of the unit

    public static void Wander(Unit unit) {
        if (UnitQueries.NeedsWanderingDestination(unit)) {
            UnitActions.SetWanderingDestination(unit);
        }
    }

    public static void Move(Unit unit)
    {
        

        if (!unit.GetComponent<Target>().IsNear(unit)){
            Vector3 projectedDestination = Vector3.ProjectOnPlane(unit.GetComponent<Target>().targetVector3, unit.transform.up);
            Quaternion targetRotation = Quaternion.LookRotation(projectedDestination, Vector3.up);
            unit.transform.rotation = Quaternion.Lerp(unit.transform.rotation, targetRotation, Time.fixedDeltaTime * unit.rotationSpeed);

            //move forward in the local axis
            unit.rb.MovePosition(unit.rb.position + unit.transform.forward * Time.fixedDeltaTime * unit.speed);

        }

    }


    
    public static void  OverrideTarget(Unit unit,Vector3 target)
    {
        unit.GetComponent<Target>().Change(target);
        unit.unitState = UnitState.Override;



        //display target selection animation.
        MonoBehaviour.Instantiate(unit.targetGraphic).GetComponent<TargetGraphic>()
            .SetPosition(target,unit.planet.transform.position);



    }

    public static void EnableAreaGraphics(Vector3 areaCenter)
    {
        GameManager.gameManager.areaGraphic.SetActive(true);
        GameManager.gameManager.areaGraphic.GetComponent<AreaGraphic>().SetPosition(areaCenter, GameManager.gameManager.planet.transform.position);
    }
    public static void DisableAreaGraphics()
    {
        GameManager.gameManager.areaGraphic.SetActive(false);

    }

    public static void HealthRegenEffect(Unit unit) {
        if (unit.health < unit.maxHealth) {
            unit.health += unit.healthRegen * Time.fixedDeltaTime;
        }
    }

    public static void HungerEffect(Unit unit)
    {
        if (unit.amountFed <= 0) {
            UnitActions.TakeDamage(unit, unit.hungerDamage * Time.fixedDeltaTime);
            unit.amountFed = 0;
        } else {
            unit.amountFed -= unit.hungerPerSecond * Time.fixedDeltaTime;
        }
    }

    public static void ThirstEffect(Unit unit)
    {
        if (unit.amountQuenched <= 0) {
            UnitActions.TakeDamage(unit, unit.thirstDamage * Time.fixedDeltaTime);
            unit.amountQuenched = 0;
        } else {
            unit.amountQuenched -= unit.thirstPerSecond * Time.fixedDeltaTime;
        }
    }


    public static void TakeDamage(Unit unit, float damage)
    {
        if (unit.dead) { return; }
        unit.health -= damage; 
        if (unit.health <= 0) { UnitActions.Die(unit); }
    }

    public static void GravityEffect(Unit unit)
    {
        unit.planet.Attract(unit.transform);
    }

    public static void TargetFood(Unit unit) {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        GameObject closestFood = UnitHelperFunctions.GetClosest(unit, foods);
        unit.GetComponent<Target>().Change(closestFood, closestFood.GetComponent<Food>().radius);
    }

    public static void TargetWater(Unit unit) {
        GameObject[] waterPoints = GameObject.FindGameObjectsWithTag("Water");
        GameObject closestWaterPoint = UnitHelperFunctions.GetClosest(unit, waterPoints);
        unit.GetComponent<Target>().Change(closestWaterPoint, closestWaterPoint.GetComponent<WaterPoint>().radius);
    }

    public static void TargetMate(Unit unit) {
        GameObject[] pets = GameObject.FindGameObjectsWithTag(unit.gameObject.tag);
        GameObject[] hornyPets = UnitHelperFunctions.FilterUnmatable(unit, pets);
        GameObject closestMate = UnitHelperFunctions.GetClosest(unit, hornyPets);
        if (closestMate != null) {
            unit.GetComponent<Target>().Change(closestMate, closestMate.GetComponent<Unit>().matingDistance);
        }
    }

    public static void TargetEnemy(Unit unit) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(unit.enemyTag);
        GameObject closestEnemy = UnitHelperFunctions.GetClosest(unit, enemies);
        if (closestEnemy== null || closestEnemy.GetComponent<Unit>() == null) { return; }
        unit.GetComponent<Target>().Change(closestEnemy, closestEnemy.GetComponent<Unit>().interactionRadius);
    }

    public static void TargetGenetium(Unit unit) {
        GameObject[] genetiums = GameObject.FindGameObjectsWithTag("Genetium");
        GameObject closestGenetium = UnitHelperFunctions.GetClosest(unit, genetiums);
        unit.GetComponent<Target>().Change(closestGenetium, closestGenetium.GetComponent<Genetium>().radius);
    }

    public static void TargetBase(Unit unit) {
        GameObject home = GameObject.FindGameObjectWithTag("Base");
        unit.GetComponent<Target>().Change(home, 3f);
    }

    public static void ReachBase(Unit unit) {
        unit.currentGenetiumAmount = 0;
        GameManager.gameManager.GetSpecies(unit.species).updateUnit(unit);
        unit.needsChange = false;
    }

    //Find a random point in planet's surface 
    public static void SetWanderingDestination(Unit unit)
    {
        //random position somewhere in a sphere around the unit target
        Vector3 position = (Random.onUnitSphere * unit.areaRadius + unit.areaCenter);

        //project on planet. raycast has to be projected from the sky
        RaycastHit hitInfo = new RaycastHit();
        
        bool hit = Physics.Raycast(position + 20f * (position - unit.planet.transform.position),
            (unit.planet.transform.position - position), out hitInfo,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) unit.GetComponent<Target>().Change(hitInfo.point);
        else Debug.Log("not hit");

        unit.wanderTimeStamp = Time.time;
    }

    public static void SetFleeingTarget(Unit unit, Unit enemy) {
        Vector3 position = enemy.transform.position;
        RaycastHit hitInfo = new RaycastHit();
        
        bool hit = Physics.Raycast(position - 20f * (position - enemy.planet.transform.position),
            (position - enemy.planet.transform.position), out hitInfo,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) unit.GetComponent<Target>().Change(hitInfo.point);
        else Debug.Log("not hit");
    }

    public static void Eat(Unit unit) {
        //eat from the source at most however much space they have on their stomach
        if(unit.GetComponent<Target>() == null) { return; }
        unit.amountFed += unit.GetComponent<Target>().targetGameObject.GetComponent<Food>().Eat(unit.maxFed - unit.amountFed);
    }

    public static void Drink(Unit unit) {
        WaterPoint waterPoint = unit.GetComponent<Target>().targetGameObject.GetComponent<WaterPoint>();
        unit.amountQuenched = Mathf.Min(unit.maxQuenched, unit.amountQuenched + waterPoint.quenchRate * Time.fixedDeltaTime);
    }

    public static void SetSwimming(Unit unit) {
        GameObject[] waterPoints = GameObject.FindGameObjectsWithTag("Water");
        foreach (GameObject waterPoint in waterPoints) {
            float distance = (unit.transform.position - waterPoint.transform.position).magnitude;
            if (distance < waterPoint.GetComponent<WaterPoint>().radius) {
                unit.swimming = true;
                return;
            }
        }
        unit.swimming = false;
    }

    public static void Mate(Unit unit) {
        GameObject[] allies = GameObject.FindGameObjectsWithTag(unit.gameObject.tag);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(unit.enemyTag);
        GameObject[] hornyAllies = UnitHelperFunctions.FilterUnmatable(unit, allies);
        if (hornyAllies.Length <= 0) { return; }
        Unit closestMate = UnitHelperFunctions.GetClosest(unit, hornyAllies).GetComponent<Unit>();
        closestMate.horny = false;
        unit.horny = false;
        if (allies.Length + enemies.Length < unit.maxUnits) {
            MonoBehaviour.Instantiate(unit.gameObject).transform.parent = GameManager.gameManager.units.transform;
        }

    }
    public static void EnableSelectionGraphic(Unit unit)
    {
        unit.selectionGraphic.SetActive(true);
        ResetSelectionGraphicPosition(unit);

    }

    public static void ResetSelectionGraphicPosition(Unit unit)
    {
        //check first raycast collision
        RaycastHit hitInfo = new RaycastHit();
        Vector3 directionToCenter = (GameManager.gameManager.planet.transform.position- unit.transform.position).normalized;
        bool hit = Physics.Raycast(unit.transform.position - directionToCenter* 5,
            directionToCenter, out hitInfo,Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) unit.selectionGraphic.transform.position = hitInfo.point;
    }

    public static void DisableSelectionGraphic(Unit unit)
    {
        unit.selectionGraphic.SetActive(false);
    }

    

    public static void Harvest(Unit unit) {
        if (unit.currentGenetiumAmount > unit.carryingCapacity) { return; }
        Genetium genetium = unit.GetComponent<Target>().targetGameObject.GetComponent<Genetium>();
        if (genetium == null) { return; }
        genetium.currentAmount -= genetium.transferRate * Time.fixedDeltaTime;
        unit.currentGenetiumAmount += genetium.transferRate * Time.fixedDeltaTime;
    }

    public static void Attack(Unit unit) {
        if (Time.time - unit.lastAttacked < unit.attackRate) { return; }
        unit.lastAttacked = Time.time;
        if (unit.GetComponent<Target>().targetGameObject == null) { return; }
        Unit enemy = unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>();
        UnitActions.TakeDamage(enemy, unit.attackDamage);
        enemy.GetComponent<Rigidbody>().AddForce(unit.transform.forward * 20, ForceMode.Impulse);
    }

    public static void Flee(Unit unit) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(unit.enemyTag);
        GameObject closestEnemy = UnitHelperFunctions.GetClosest(unit, enemies);
        if (closestEnemy== null || closestEnemy.GetComponent<Unit>() == null) { return; }
        SetFleeingTarget(unit, closestEnemy.GetComponent<Unit>());
    }

    public static void TurnHungryChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        float hungerRatio = (unit.amountFed / unit.maxFed);
        float chanceOfHungerPerSec = Mathf.Pow(1 - hungerRatio, unit.hungerChanceExponent) * Time.fixedDeltaTime;
        if (random < chanceOfHungerPerSec) {
            unit.hungry = true;
        }
    }

    public static void TurnThirstyChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        float thirstRatio = (unit.amountQuenched / unit.maxQuenched);
        float chanceOfThirstPerSec = Mathf.Pow(1 - thirstRatio, unit.thirstChanceExponent) * Time.fixedDeltaTime;
        if (random < chanceOfThirstPerSec) {
            unit.thirsty = true;
        }
    }

    public static void TurnHornyChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        if (random < unit.hornyChancePerSecond * Time.fixedDeltaTime) {
            unit.horny = true;
        }
    }

    public static void TurnFed(Unit unit) {
        unit.hungry = false;
    }

    public static void TurnQuenched(Unit unit) {
        unit.thirsty = false;
    }

    public static void WanderIfDeadTarget(Unit unit) {
        if (
            unit.GetComponent<Target>().targetGameObject &&
            unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>() &&
            unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>().dead
        ) {
            unit.unitState = UnitState.Wander;
        }
    }

    public static void Dead(Unit unit) {
        if (Time.time - unit.deathTimeStamp > unit.deathPeriod) {
            Object.Destroy(unit.gameObject);
        }
    }

    public static void Die(Unit unit)
    {
        unit.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        unit.dead = true;
        unit.deathTimeStamp = Time.time;
    }

    public static void SetThought(Unit unit) {
        unit.thoughtPivot.transform.rotation = Camera.main.transform.rotation;
        if (unit.hungry) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.hungrySprite;
        } else if (unit.thirsty) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.thirstSprite;
        } else if (unit.unitState == UnitState.TargetMate) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.hornySprite;
        } else if (unit.unitState == UnitState.TargetGenetium) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.genetiumSprite;
        } else if (unit.unitState == UnitState.TargetBase || unit.unitState == UnitState.Harvest) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.baseSprite;
        } else {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = null;
        }
    }

    public static void SetHealthBar(Unit unit) {
        unit.healthbarPivot.transform.rotation = Camera.main.transform.rotation;
        unit.healthbar.size = new Vector2(unit.health / unit.maxHealth * 9f, 1);
        // unit.healthbar.fillAmount = unit.health / unit.maxHealth;
    }
}
