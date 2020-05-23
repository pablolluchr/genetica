using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitActions {
    //Unit behaviour functions that change values of the unit

    #region hunger // ##################################################################################
    
    public static void HungerEffect(Unit unit) {
        if (unit.amountFed <= 0) {
            UnitActions.TakeDamage(unit, unit.hungerDamage * Time.deltaTime);
            unit.amountFed = 0;
        } else {
            unit.amountFed -= unit.hungerPerSecond * Time.deltaTime;
        }
    }

    public static void TargetFood(Unit unit) {
        GameObject closestFood = UnitQueries.ClosestFoodInView(unit);
        if (closestFood == null) return;
        unit.GetComponent<Target>().Change(closestFood, closestFood.GetComponent<Food>().radius);
    }

    public static void Eat(Unit unit) {
        //eat from the source at most however much space they have on their stomach
        if (unit.GetComponent<Target>() == null) return;
        unit.amountFed += unit.GetComponent<Target>().targetGameObject.GetComponent<Food>().Eat(unit.maxFed - unit.amountFed);
    }

    public static void TurnFed(Unit unit) {
        unit.hungry = false;
    }
    
    public static void TurnHungryChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        float hungerRatio = (unit.amountFed / unit.maxFed);
        float chanceOfHungerPerSec = Mathf.Pow(1 - hungerRatio, unit.hungerChanceExponent) * Time.deltaTime;
        if (random < chanceOfHungerPerSec) {
            unit.hungry = true;
        }
    }

    #endregion

    #region thirst // ################################################################################
   
    public static void ThirstEffect(Unit unit) {
        if (unit.amountQuenched <= 0) {
            UnitActions.TakeDamage(unit, unit.thirstDamage * Time.deltaTime);
            unit.amountQuenched = 0;
        } else {
            unit.amountQuenched -= unit.thirstPerSecond * Time.deltaTime;
        }
    }

    public static void TurnQuenched(Unit unit) {
        unit.thirsty = false;
    }

    public static void TargetWater(Unit unit) {
        Vector3 closestWaterPoint = UnitQueries.ClosestWaterSource(unit);
        unit.GetComponent<Target>().Change(closestWaterPoint);
    }

    public static void Drink(Unit unit) {
        unit.amountQuenched = Mathf.Min(unit.maxQuenched, unit.amountQuenched + unit.quenchRate * Time.deltaTime);
    }

    public static void TurnThirstyChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        float thirstRatio = (unit.amountQuenched / unit.maxQuenched);
        float chanceOfThirstPerSec = Mathf.Pow(1 - thirstRatio, unit.thirstChanceExponent) * Time.deltaTime;
        if (random < chanceOfThirstPerSec) {
            unit.thirsty = true;
        }
    }


    #endregion

    #region mating // ################################################################################
  
    public static void TargetMate(Unit unit) {
        GameObject closestMate = UnitQueries.ClosestMateInView(unit);
        if (closestMate == null) return;
        unit.GetComponent<Target>().Change(closestMate, closestMate.GetComponent<Unit>().matingDistance);
    }

    public static void Mate(Unit unit) {
        GameObject closestMateObj = UnitQueries.ClosestMateInView(unit);
        if (closestMateObj == null) return;
        Unit closestMate = closestMateObj.GetComponent<Unit>();
        closestMate.horny = false;
        unit.horny = false;

        GameObject[] allies = GameObject.FindGameObjectsWithTag(unit.gameObject.tag);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(unit.enemyTag);
        if (allies.Length + enemies.Length < unit.maxUnits) {
            MonoBehaviour.Instantiate(unit.gameObject).transform.parent = GameManager.gameManager.units.transform;
        }
    }

    public static void TurnHornyChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        float chanceMultiplier = (unit.health / unit.maxHealth) 
            * (unit.amountQuenched / unit.maxQuenched) * (unit.amountFed / unit.maxFed);
        chanceMultiplier = Mathf.Pow(chanceMultiplier, unit.hornyCurveExponent);
        if (random < unit.hornyChancePerSecond * chanceMultiplier * Time.deltaTime) {
            unit.horny = true;
        }
    }

    #endregion

    #region genetium // ################################################################################

    public static void TargetGenetium(Unit unit) {
        GameObject closestGenetium = UnitQueries.ClosestGenetiumInView(unit);
        if (closestGenetium == null) return;
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

    public static void Harvest(Unit unit) {
        if (unit.currentGenetiumAmount > unit.carryingCapacity) return;
        Genetium genetium = unit.GetComponent<Target>().targetGameObject.GetComponent<Genetium>();
        if (genetium == null) return;
        float amountHarvested = Mathf.Min(genetium.transferRate * Time.deltaTime, genetium.currentAmount);
        genetium.currentAmount -= amountHarvested;
        unit.currentGenetiumAmount += amountHarvested;
    }

    #endregion

    #region attacking // ################################################################################

    public static void TargetEnemy(Unit unit) {
        GameObject closestEnemy = UnitQueries.ClosestEnemyInThreatRange(unit);
        if (closestEnemy == null || closestEnemy.GetComponent<Unit>() == null) return;
        unit.GetComponent<Target>().Change(closestEnemy, closestEnemy.GetComponent<Unit>().interactionRadius);
    }

    public static void SetFleeingTarget(Unit unit, Unit enemy) {
        Vector3 position = enemy.transform.position;
        RaycastHit hitInfo = new RaycastHit();

        bool hit = Physics.Raycast(position - 20f * (position - GameManager.gameManager.planet.transform.position),
            (position - GameManager.gameManager.planet.transform.position), out hitInfo,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) unit.GetComponent<Target>().Change(hitInfo.point);
    }

    public static void Attack(Unit unit) {
        if (Time.time - unit.lastAttacked < unit.attackRate) return;
        unit.lastAttacked = Time.time;
        if (unit.GetComponent<Target>().targetGameObject == null) return;
        Unit enemy = unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>();
        UnitActions.TakeDamage(enemy, unit.attackDamage);
        enemy.GetComponent<Rigidbody>().AddForce(unit.transform.forward * unit.attackForce, ForceMode.Impulse);
    }

    public static void Flee(Unit unit) {
        GameObject closestEnemy = UnitQueries.ClosestEnemyInThreatRange(unit);
        if (closestEnemy == null || closestEnemy.GetComponent<Unit>() == null) return;
        SetFleeingTarget(unit, closestEnemy.GetComponent<Unit>());
    }

    #endregion

    #region health // ################################################################################

    public static void Dead(Unit unit) {
        if (Time.time - unit.deathTimeStamp > unit.deathPeriod) {
            Object.Destroy(unit.gameObject);
        }
    }

    public static void Die(Unit unit) {
        unit.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        unit.dead = true;
        unit.deathTimeStamp = Time.time;
    }

    public static void HealthRegenEffect(Unit unit) {
        if (unit.health < unit.maxHealth) {
            unit.health += unit.healthRegen * Time.deltaTime;
        }
    }

    public static void TakeDamage(Unit unit, float damage) {
        if (unit.dead) return;
        unit.health -= damage;
        unit.healthbar.size = new Vector2(unit.health / unit.maxHealth * 9f, 1);
        if (unit.health <= 0) UnitActions.Die(unit);
    }

    public static void SetHealthBar(Unit unit) {
        unit.healthbarPivot.transform.rotation = Camera.main.transform.rotation;
    }

    #endregion

    #region selection // ################################################################################

    public static void EnableAreaGraphics(Species species) {
        GameManager.gameManager.areaGraphic.SetActive(true);
        GameManager.gameManager.areaGraphic.GetComponent<AreaGraphic>()
            .SetPosition(species.areaCenter, GameManager.gameManager.planet.transform.position);
        Unit[] units = species.GetAllUnits();
        foreach (Unit unit in units) {
            UnitActions.EnableSelectionGraphic(unit);
        }
    }

    public static void DisableAreaGraphics() {
        GameManager.gameManager.areaGraphic.SetActive(false);
    }

    public static void ResetSelectionGraphicPosition(Unit unit) {
        //check first raycast collision
        RaycastHit hitInfo = new RaycastHit();
        Vector3 directionToCenter = (GameManager.gameManager.planet.transform.position - unit.transform.position).normalized;
        bool hit = Physics.Raycast(unit.transform.position - directionToCenter * 5,
            directionToCenter, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) unit.selectionGraphic.transform.position = hitInfo.point;
    }

    public static void EnableSelectionGraphic(Unit unit) {
        unit.selectionGraphic.SetActive(true);
        ResetSelectionGraphicPosition(unit);
    }

    public static void DisableSelectionGraphic(Unit unit) {
        unit.selectionGraphic.SetActive(false);
    }

    public static void DisableAllSelectionGraphics() {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hostiles = GameObject.FindGameObjectsWithTag("Hostile");
        foreach (GameObject pet in pets) {
            DisableSelectionGraphic(pet.GetComponent<Unit>());
        }
        foreach (GameObject hostile in hostiles) {
            DisableSelectionGraphic(hostile.GetComponent<Unit>());
        }
    }

    #endregion

    #region movement // ################################################################################

    public static void Wander(Unit unit) {
        if (UnitQueries.NeedsWanderingDestination(unit)) {
            UnitActions.SetWanderingDestination(unit);
        }
    }

    public static void Move(Unit unit) {
        if (!unit.GetComponent<Target>().IsNear(unit, false)) {
            Vector3 projectedDestination = Vector3.ProjectOnPlane(unit.GetComponent<Target>().targetVector3, unit.transform.up);
            Quaternion targetRotation = Quaternion.LookRotation(projectedDestination, Vector3.up);
            unit.transform.rotation = Quaternion.Lerp(unit.transform.rotation, targetRotation, Time.fixedDeltaTime * unit.rotationSpeed);

            float movementMultiplier;
            if (unit.swimming) movementMultiplier = unit.swimspeed;
            else movementMultiplier = unit.walkspeed;

            //move forward in the local axis
            unit.rb.MovePosition(unit.rb.position + unit.transform.forward * Time.fixedDeltaTime * unit.speed * movementMultiplier);

        }
    }

    public static void OverrideTarget(Unit unit, Vector3 target) {
        unit.GetComponent<Target>().Change(target);
        unit.unitState = UnitState.Override;

    }

    public static void ShowTargetGraphic(Unit unit)
    {
        MonoBehaviour.Instantiate(unit.targetGraphic).GetComponent<TargetGraphic>()
            .SetPosition(unit.GetComponent<Target>().targetVector3, GameManager.gameManager.planet.transform.position);
    }

    //Find a random point in planet's surface 
    public static void SetWanderingDestination(Unit unit) {
        //random position somewhere in a sphere around the unit target
        Vector3 position = (Random.onUnitSphere * unit.areaRadius + unit.areaCenter);

        //project on planet. raycast has to be projected from the sky
        RaycastHit hitInfo = new RaycastHit();

        bool hit = Physics.Raycast(position + 20f * (position - GameManager.gameManager.planet.transform.position),
            (GameManager.gameManager.planet.transform.position - position), out hitInfo,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) {
            unit.GetComponent<Target>().Change(hitInfo.point);
        }

        unit.wanderTimeStamp = Time.time;
    }

    public static void WanderIfDeadTarget(Unit unit) {
        if (
            unit.GetComponent<Target>().targetGameObject
            && unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>()
            && unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>().dead
        ) {
            unit.unitState = UnitState.Wander;
        }
    }

    #endregion

    // ###################################################################################################

    public static void GravityEffect(Unit unit) {
        GameManager.gameManager.planet.GetComponent<GravityAttractor>().Attract(unit.transform);
    }


    public static void SetThought(Unit unit) {
        unit.thoughtPivot.transform.rotation = Camera.main.transform.rotation;
        if (unit.unitState == UnitState.TargetEnemy || unit.unitState == UnitState.Attack) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.attackSprite;
        } else if (unit.unitState == UnitState.TargetGenetium || unit.unitState == UnitState.Harvest) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.genetiumSprite;
        } else if (unit.unitState == UnitState.TargetBase) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.baseSprite;
        } else if (unit.hungry) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.hungrySprite;
        } else if (unit.thirsty) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.thirstSprite;
        } else if (unit.horny) {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = unit.hornySprite;
        } else {
            unit.thoughtPivot.GetComponentInChildren<SpriteRenderer>().sprite = null;
        }
    }
}