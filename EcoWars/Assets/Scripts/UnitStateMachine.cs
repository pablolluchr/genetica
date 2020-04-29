using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState {
    Wander,
    MoveToWater,
    Drink,
    MoveToFood,
    Eat,
    MoveToMate,
    Mate,
    MoveToFuel,
    Harvest,
    MoveToBase,
    EngageEnemy,
    Attack,
    Flee,
    Override,
    Dead
}

public static class UnitStateMachine {

    public static UnitState NextState(Unit unit) { //returns next state

        

        switch (unit.unitState) {
            case UnitState.Wander: {
                if (UnitQueries.IsThreatened(unit)) {
                    if (UnitQueries.ShouldBeAggressive(unit)) {
                        return UnitState.EngageEnemy;
                    } else {
                        return UnitState.Flee;
                    }
                }
                if (UnitQueries.IsHungry(unit) && UnitQueries.SeesFood(unit)) { return UnitState.MoveToFood; }
                if (UnitQueries.IsThirsty(unit) && UnitQueries.SeesWater(unit)) { return UnitState.MoveToWater; }
                if (UnitQueries.NeedsChange(unit)) { return UnitState.MoveToBase; }
                if (UnitQueries.IsHorny(unit) && UnitQueries.SeesMate(unit)) { return UnitState.MoveToMate; }
                if (UnitQueries.SeesFuel(unit)) { return UnitState.MoveToFuel; }
                if (UnitQueries.IsCarryingFuel(unit)) { return UnitState.MoveToBase; }
                UnitActions.Wander(unit);
                break;
            }
            case UnitState.MoveToWater: {
                UnitActions.MoveToWater(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesWater(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit)) { return UnitState.Drink; }
                
                break;
            }
            case UnitState.MoveToFood: {
                UnitActions.MoveToFood(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesFood(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit)) { return UnitState.Eat; }
                break;
            }
            case UnitState.MoveToMate: {
                UnitActions.MoveToMate(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesMate(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit)) { return UnitState.Mate; }
                break;
            }
            case UnitState.MoveToFuel: {
                UnitActions.MoveToFuel(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesFuel(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit)) { return UnitState.Harvest; }
                break;
            }
            case UnitState.MoveToBase: {
                UnitActions.MoveToBase(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit)) { return UnitState.Wander; }
                break;
            }
            case UnitState.Drink: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsQuenched(unit)) { return UnitState.Wander; }
                UnitActions.Drink(unit);
                break;
            }
            case UnitState.Eat: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsFed(unit)) { return UnitState.Wander; }
                UnitActions.Eat(unit);
                break;
            }
            case UnitState.Mate: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.IsHorny(unit)) { return UnitState.Wander; }
                UnitActions.Mate(unit);
                break;
            }
            case UnitState.Harvest: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.IsNearTarget(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsStorageFull(unit)) { return UnitState.Wander; }
                UnitActions.Harvest(unit);
                break;
            }
            case UnitState.EngageEnemy: {
                if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsVeryHungry(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsVeryThirsty(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsNearTarget(unit)) { return UnitState.Attack; }
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.EngageEnemy(unit);
                break;
            }
            case UnitState.Attack: {
                if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                if (!UnitQueries.IsNearTarget(unit)) { return UnitState.EngageEnemy; }
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.Attack(unit);
                break;
            }
            case UnitState.Flee: {
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.Flee(unit);
                break;
            }
            case UnitState.Override:{
                    if (UnitQueries.IsNearTarget(unit)) { return UnitState.Wander; }
                break;
            }
            case UnitState.Dead: {
                Object.Destroy(unit.gameObject);
                break;
            }
        }
        return unit.unitState; //no state change

    }

}
