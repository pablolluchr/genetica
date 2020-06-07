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
        //todo: if enemies approach them they start attacking? or only if the user puts them in attack mode
        //if they don't start attacking do they run away?

        //todo: only show mating thought when having a baby (make the fucking process longer)
        if (unit.dead) { UnitActions.Dead(unit); }

        switch (unit.unitState) {
            case UnitState.Wander: {

                    //rethink this
                    if (UnitQueries.IsThreatened(unit)){
                        if (UnitQueries.ShouldBeAggressive(unit)){
                            return UnitState.TargetEnemy;
                        }else{
                            return UnitState.Flee;
                        }
                    }
                    if (UnitQueries.NeedsChange(unit)) { return UnitState.TargetBase; } //TODO this should probably override cause it seems like a bug otherwise

                    //todo; enemies should also grow in population if given unlimited food
                    //The species starts with a base food source allocated so they always have a target food source
                    if (UnitQueries.IsHungry(unit) && unit.foodSource!=null) { UnitActions.TargetFood(unit); return UnitState.TargetFood; }
                    if (UnitQueries.IsThirsty(unit)) { UnitActions.TargetWater(unit); return UnitState.TargetWater; }
                    if (UnitQueries.IsHorny(unit) && UnitQueries.SeesMate(unit)) { return UnitState.TargetMate; }
                    if (unit.genetiumSource!=null && !UnitQueries.IsStorageFull(unit)) {
                        UnitActions.TargetGenetium(unit);return UnitState.TargetGenetium; }
                    if (UnitQueries.IsCarryingGenetium(unit)) { return UnitState.TargetBase; }
                    UnitActions.Wander(unit);
                break;
            }
            case UnitState.TargetWater: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                //if (!UnitQueries.SeesWater(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearTarget(unit, false)) { return UnitState.Drink; }
                
                break;
            }
            case UnitState.TargetFood: {
                    //todo handle change of food source as its targetting
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                //if (!UnitQueries.SeesFood(unit)) { return UnitState.Wander; }
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
                    //todo handle change of genetium source as its targetting

                    //if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                    if (UnitQueries.IsHungry(unit) && unit.foodSource != null) { return UnitState.TargetFood; }
                if (UnitQueries.IsThirsty(unit)) { return UnitState.TargetWater; } //todo what if the water is protected by enemies?
                //if (!UnitQueries.SeesGenetium(unit)) { return UnitState.Wander; }
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
                //todo handle change of food source as its eating
                //if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
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
                    //todo handle change of food source as its harvesting

                    if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                    if (!UnitQueries.IsNearTarget(unit,true)) { return UnitState.Wander; }
                    if (UnitQueries.IsStorageFull(unit)) { return UnitState.Wander; }
                    if (!UnitQueries.SeesGenetium(unit)) { return UnitState.Wander; }
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
