using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitQueries {

    // query functions here (not allowed to modify state) ##################################################

    public static bool IsQuenched(Unit unit) {
        return false;
    }

    public static bool IsThirsty(Unit unit) {
        // random based on thirst
        return false;
    }

    public static bool IsVeryThirsty(Unit unit) {
        // threshold
        return false;
    }

    public static bool IsFed(Unit unit) {
        if (unit.amountFed >= unit.maxFed) {
            return true;
        }
        return false;
    }

    public static bool IsHungry(Unit unit) {
        float random = Random.Range(0f, 1f);
        float hungerRatio = (unit.amountFed / unit.maxFed);
        float chanceOfHungerPerSec = Mathf.Pow(1 - hungerRatio, unit.hungerChanceExponent) * Time.deltaTime;
        if (random < chanceOfHungerPerSec) {
            return true;
        }
        // random based on hunger
        return false;
    }

    public static bool IsVeryHungry(Unit unit) {
        // threshold
        return false;
    }

    public static bool IsHorny(Unit unit) {
        if (unit.horny) { return true; }
        float random = Random.Range(0f, 1f);
        if (random < unit.hornyChancePerSecond * Time.deltaTime) {
            UnitActions.TurnHorny(unit);
            return true;
        }
        return false;
    }

    public static bool SeesMate(Unit unit) {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hornyPets = UnitHelperFunctions.GetOtherHornyPets(unit, pets);
        return UnitHelperFunctions.InRangeOf(unit, hornyPets, unit.viewDistance);
    }

    public static bool SeesWater(Unit unit) {
        return false;
    }

    public static bool SeesFood(Unit unit) {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        return UnitHelperFunctions.InRangeOf(unit, foods, unit.viewDistance);
    }

    public static bool SeesFuel(Unit unit) {
        return false;
    }

    public static bool IsNearWater(Unit unit) {
        return false;
    }

    public static bool IsNearFood(Unit unit) {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        return UnitHelperFunctions.InRangeOf(unit, foods, unit.eatingDistance);
    }

    public static bool IsNearMate(Unit unit) {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] hornyPets = UnitHelperFunctions.GetOtherHornyPets(unit, pets);
        return UnitHelperFunctions.InRangeOf(unit, hornyPets, unit.matingDistance);
    }

    public static bool IsNearBase(Unit unit) {
        return false;
    }

    public static bool IsNearEnemy(Unit unit) {
        return false;
    }

    public static bool IsNearFuel(Unit unit) {
        return false;
    }

    public static bool IsThreatened(Unit unit) {
        // check for near enemies
        return false;
    }

    public static bool ShouldBeAggressive(Unit unit) {
        // randomly return true or false based on aggression
        return true;
    }

    public static bool IsCarryingFuel(Unit unit) {
        return false;
    }

    public static bool NeedsChange(Unit unit) {
        return false;
    }

    public static bool IsStorageFull(Unit unit) {
        return false;
    }

    public static bool HasLowHealth(Unit unit) {
        return false;
    }
}
