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

    public static void TargetWater(Unit unit) {

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
        // suffer thirst
    }

    public static void Die(Unit unit)
    {
        unit.unitState = UnitState.Dead;
    }


    public static void TakeDamage(Unit unit, float damage)
    {
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

    public static void TargetMate(Unit unit) {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hornyPets = UnitHelperFunctions.FilterNonHornyPetsAndSelf(unit, pets);
        GameObject closestMate = UnitHelperFunctions.GetClosest(unit, hornyPets);
        if (closestMate != null) {
            unit.GetComponent<Target>().Change(closestMate, closestMate.GetComponent<Unit>().matingDistance);
        }
    }

    public static void TargetEnemy(Unit unit) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(unit.enemyTag);
        GameObject closestEnemy = UnitHelperFunctions.GetClosest(unit, enemies);
        if (closestEnemy.GetComponent<Unit>() == null) { return; }
        unit.GetComponent<Target>().Change(closestEnemy, closestEnemy.GetComponent<Unit>().interactionRadius);
    }

    public static void TargetFuel(Unit unit) {

    }

    public static void TargetBase(Unit unit) {

    }

    public static void Drink(Unit unit) {

    }

    //Find a random point in planet's surface 
    public static void SetWanderingDestination(Unit unit)
    {
        //random position somewhere in a sphere
        Vector3 position = Random.onUnitSphere;

        //project on planet. raycast has to be projected from the sky
        RaycastHit hitInfo = new RaycastHit();
        
        bool hit = Physics.Raycast(position+(position - unit.planet.transform.position)*20f,
            (unit.planet.transform.position-position), out hitInfo,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (hit) unit.GetComponent<Target>().Change(hitInfo.point);
        else Debug.Log("not hit");

        unit.wanderTimeStamp = Time.time;
    }

    public static void Eat(Unit unit) {
        //eat from the source at most however much space they have on their stomach
        if(unit.GetComponent<Target>() == null) { return; }
        unit.amountFed += unit.GetComponent<Target>().targetGameObject.GetComponent<Food>().Eat(unit.maxFed - unit.amountFed);
    }

    public static void Mate(Unit unit) {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hostiles = GameObject.FindGameObjectsWithTag("Hostile");
        GameObject[] hornyPets = UnitHelperFunctions.FilterNonHornyPetsAndSelf(unit, pets);
        if (hornyPets.Length <= 0) { return; }
        Unit closestMate = UnitHelperFunctions.GetClosest(unit, hornyPets).GetComponent<Unit>();
        closestMate.horny = false;
        unit.horny = false;
        if (pets.Length +hostiles.Length< unit.maxUnits) {
            MonoBehaviour.Instantiate(unit.gameObject).transform.parent=GameManager.gameManager.units.transform;
        }

    }

    public static void DisableSelectionGraphic(Unit unit)
    {
        unit.selectionGraphic.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = unit.selectionColor;
    }

    public static void EnableSelectionGraphic(Unit unit)
    {
        unit.selectionGraphic.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 0f, .8f);
        
    }

    public static void Harvest(Unit unit) {

    }

    public static void Attack(Unit unit) {
        if (unit.GetComponent<Target>().targetGameObject == null) { return; }
        Unit enemy = unit.GetComponent<Target>().targetGameObject.GetComponent<Unit>();
        UnitActions.TakeDamage(enemy, unit.attackDamagePerSecond * Time.fixedDeltaTime);
    }

    public static void Flee(Unit unit) {

    }

    public static void TurnHungryChance(Unit unit) {
        float random = Random.Range(0f, 1f);
        float hungerRatio = (unit.amountFed / unit.maxFed);
        float chanceOfHungerPerSec = Mathf.Pow(1 - hungerRatio, unit.hungerChanceExponent) * Time.fixedDeltaTime;
        if (random < chanceOfHungerPerSec) {
            unit.hungry = true;
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
}
