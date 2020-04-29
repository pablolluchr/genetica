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

    public static void MoveToWater(Unit unit) {

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
    }

    public static void MoveToFood(Unit unit) {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        GameObject closestFood = UnitHelperFunctions.GetClosest(unit, foods);
        unit.GetComponent<Target>().Change(closestFood, closestFood.GetComponent<Food>().radius);



    }

    public static void HungerEffect(Unit unit)
    {
        unit.amountFed -= unit.hungerPerSecond * Time.deltaTime;
        if (unit.amountFed <= 0) { UnitActions.TakeDamage(unit, unit.hungerDamage); }
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

    public static void MoveToMate(Unit unit) {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hornyPets = UnitHelperFunctions.GetOtherHornyPets(unit, pets);
        GameObject closestMate = UnitHelperFunctions.GetClosest(unit, hornyPets);
        if (closestMate != null) {

            unit.GetComponent<Target>().Change(closestMate, closestMate.GetComponent<Unit>().matingDistance);


        }
    }

    public static void MoveToFuel(Unit unit) {

    }

    public static void MoveToBase(Unit unit) {

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
        GameObject[] hornyPets = UnitHelperFunctions.GetOtherHornyPets(unit, pets);
        if (hornyPets.Length <= 0) { return; }
        Unit closestMate = UnitHelperFunctions.GetClosest(unit, hornyPets).GetComponent<Unit>();
        closestMate.horny = false;
        unit.horny = false;
        if (pets.Length < unit.maxUnits) {
            MonoBehaviour.Instantiate(unit.gameObject).transform.parent=GameManager.gameManager.units.transform;
        }
    }

    public static void Harvest(Unit unit) {

    }

    public static void EngageEnemy(Unit unit) {

    }

    public static void Attack(Unit unit) {

    }

    public static void Flee(Unit unit) {

    }

    public static void TurnHorny(Unit unit) {
        unit.horny = true;
    }
}
