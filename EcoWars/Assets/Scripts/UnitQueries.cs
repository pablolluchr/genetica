﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitQueries {

    // unit behaviour query functions here (not allowed to modify state) ##################################################

    public static bool IsQuenched(Unit unit) {
        return unit.amountQuenched / unit.maxQuenched >= 0.99;
    }

    public static bool IsThirsty(Unit unit) {
        return unit.thirsty;
    }

    public static bool IsVeryThirsty(Unit unit) {
        return unit.amountQuenched / unit.maxQuenched <= unit.criticalThirst;
    }

    public static bool IsFed(Unit unit) {
        return unit.amountFed / unit.maxFed >= 0.99;
    }

    public static bool IsHungry(Unit unit) {
        return unit.hungry;
    }

    public static bool IsVeryHungry(Unit unit) {
        return unit.amountFed / unit.maxFed <= unit.criticalHunger;
    }

    public static bool IsHorny(Unit unit) {
        return unit.horny;
    }

    public static bool SeesMate(Unit unit) {
        if (unit.gameObject.tag == "Hostile") { return false; }
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hornyPets = UnitHelperFunctions.FilterUnmatable(unit, pets);
        return UnitHelperFunctions.InRangeOf(unit, hornyPets, unit.viewDistance);
    }

    
    public static bool SeesWater(Unit unit) {

        Vector3 closestWaterSource = UnitActions.ClosestWaterSource(unit);
        return (closestWaterSource - unit.transform.position).magnitude <= unit.viewDistance;
    }

    public static bool SeesFood(Unit unit) {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        GameObject[] nonEmptyFoods = UnitHelperFunctions.FilterEmptyFoods(foods);
        return UnitHelperFunctions.InRangeOf(unit, nonEmptyFoods, unit.viewDistance);
    }

    public static bool SeesGenetium(Unit unit) {
        if (unit.gameObject.tag == "Hostile") { return false; }
        GameObject[] genetiums = GameObject.FindGameObjectsWithTag("Genetium");
        GameObject[] nonEmptyGenetiums = UnitHelperFunctions.FilterEmptyGenetium(genetiums);
        return UnitHelperFunctions.InRangeOf(unit, nonEmptyGenetiums, unit.genetiumDetectionRange);
    }

    public static bool IsNearTarget(Unit unit,bool increasedRadius)
    {
        return unit.GetComponent<Target>().IsNear(unit, increasedRadius);
    }

    public static bool IsThreatened(Unit unit) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(unit.enemyTag);
        GameObject[] aliveEnemies = UnitHelperFunctions.FilterDeadEnemies(enemies);
        return UnitHelperFunctions.InRangeOf(unit, aliveEnemies, unit.enemyDetectionRange);
    }

    public static bool ShouldBeAggressive(Unit unit) {
        float random = Random.Range(0f, 1f);
        return random < unit.aggression;
    }

    //does the unit require to be given a new destination
    public static bool NeedsWanderingDestination(Unit unit)
    {
        //no destination
        if (unit.GetComponent<Target>().targetVector3 == Vector3.zero) { return true; }

        ////destination already reached
        if (UnitQueries.IsNearTarget(unit,false)) { return true; }
        //if its wandering and couldn't reach the destination in 10 sec reset 
        if (Time.time - unit.wanderTimeStamp > 10f && unit.unitState == UnitState.Wander) { return true; }

        //otherwise
        return false;
    }

    public static bool IsCarryingGenetium(Unit unit) {
        return unit.currentGenetiumAmount / unit.carryingCapacity >= 0.01;
    }

    public static bool NeedsChange(Unit unit) {
        return unit.needsChange;
    }

    public static bool IsStorageFull(Unit unit) {
        return unit.currentGenetiumAmount / unit.carryingCapacity >= 0.99;
    }

    public static bool HasLowHealth(Unit unit) {
        return unit.health / unit.maxHealth <= unit.criticalHealth;
    }
}
