using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState {
    Wander,
    TargetWater,
    Drink,
    TargetFood,
    Eat,
    TargetMate,
    Mate,
    TargetGenetium,
    Harvest,
    TargetBase,
    TargetEnemy,
    Attack,
    Flee,
    Override,
    Dead
}

public static class UnitStateMachine {

    public static UnitState NextState(Unit unit) { //returns next state

        if (unit.dead) { UnitActions.Dead(unit); }

        switch (unit.unitState) {
            case UnitState.Wander: {
                if (UnitQueries.IsThreatened(unit)) {
                    if (UnitQueries.ShouldBeAggressive(unit)) {
                        return UnitState.TargetEnemy;
                    } else {
                        return UnitState.Flee;
                    }
                }
                if (UnitQueries.IsHungry(unit) && UnitQueries.SeesFood(unit)) { return UnitState.TargetFood; }
                if (UnitQueries.IsThirsty(unit) && UnitQueries.SeesWater(unit)) { return UnitState.TargetWater; }
                if (UnitQueries.NeedsChange(unit)) { return UnitState.TargetBase; }
                if (UnitQueries.IsHorny(unit) && UnitQueries.SeesMate(unit)) { return UnitState.TargetMate; }
                if (UnitQueries.SeesGenetium(unit) && !UnitQueries.IsStorageFull(unit)) { return UnitState.TargetGenetium; }
                if (UnitQueries.IsCarryingGenetium(unit)) { return UnitState.TargetBase; }
                UnitActions.Wander(unit);
                break;
            }
            case UnitState.TargetWater: {
                UnitActions.TargetWater(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesWater(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit, false)) { return UnitState.Drink; }
                
                break;
            }
            case UnitState.TargetFood: {
                UnitActions.TargetFood(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesFood(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit, false)) { return UnitState.Eat; }
                break;
            }
            case UnitState.TargetMate: {
                UnitActions.TargetMate(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesMate(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit, false)) { return UnitState.Mate; }
                break;
            }
            case UnitState.TargetGenetium: {
                UnitActions.TargetGenetium(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsHungry(unit) && UnitQueries.SeesFood(unit)) { return UnitState.TargetFood; }
                if (UnitQueries.IsThirsty(unit) && UnitQueries.SeesWater(unit)) { return UnitState.TargetWater; }
                if (!UnitQueries.SeesGenetium(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit, false)) { return UnitState.Harvest; }
                break;
            }
            case UnitState.TargetBase: {
                UnitActions.TargetBase(unit);
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsHungry(unit) && UnitQueries.SeesFood(unit)) { return UnitState.TargetFood; }
                if (UnitQueries.IsThirsty(unit) && UnitQueries.SeesWater(unit)) { return UnitState.TargetWater; }
                if (UnitQueries.IsNearTarget(unit, false)) { UnitActions.ReachBase(unit); return UnitState.Wander; }
                break;
            }
            case UnitState.TargetEnemy: {
                UnitActions.TargetEnemy(unit);
                if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsVeryHungry(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsVeryThirsty(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsNearTarget(unit, true)) { return UnitState.Attack; }
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                break;
            }
            case UnitState.Drink: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsQuenched(unit)) { UnitActions.TurnQuenched(unit); return UnitState.Wander; }
                UnitActions.Drink(unit);
                break;
            }
            case UnitState.Eat: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsFed(unit)) { UnitActions.TurnFed(unit); return UnitState.Wander; }
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
                    if (!UnitQueries.IsNearTarget(unit,true)) { return UnitState.Wander; }
                    if (UnitQueries.IsStorageFull(unit)) { return UnitState.Wander; }
                UnitActions.Harvest(unit);
                break;
            }
            case UnitState.Attack: {
                if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                if (!UnitQueries.IsNearTarget(unit,true)) { return UnitState.TargetEnemy; }
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.Attack(unit);
                break;
            }
            case UnitState.Flee: {
                UnitActions.Flee(unit);
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                break;
            }
            case UnitState.Override:{
                if (UnitQueries.IsNearTarget(unit,false)) { return UnitState.Wander; }
                break;
            }
        }

        return unit.unitState; //no state change

    }

}
