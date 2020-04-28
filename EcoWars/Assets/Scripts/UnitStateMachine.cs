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
    Dead
}

public static class UnitStateMachine {

    public static UnitState NextState(Unit unit) { //returns next state

        unit.HungerEffect();
        unit.ThirstEffect();

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
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesWater(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearWater(unit)) { return UnitState.Drink; }
                UnitActions.MoveToWater(unit);
                break;
            }
            case UnitState.MoveToFood: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesFood(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearFood(unit)) { return UnitState.Eat; }
                UnitActions.MoveToFood(unit);
                break;
            }
            case UnitState.MoveToMate: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesMate(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearMate(unit)) { return UnitState.Mate; }
                UnitActions.MoveToMate(unit);
                break;
            }
            case UnitState.MoveToFuel: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (!UnitQueries.SeesFuel(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearFuel(unit)) { return UnitState.Harvest; }
                UnitActions.MoveToFuel(unit);
                break;
            }
            case UnitState.MoveToBase: {
                if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsNearBase(unit)) { return UnitState.Wander; }
                UnitActions.MoveToBase(unit);
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
                if (!UnitQueries.IsNearFuel(unit)) { return UnitState.Wander; }
                if (UnitQueries.IsStorageFull(unit)) { return UnitState.Wander; }
                UnitActions.Harvest(unit);
                break;
            }
            case UnitState.EngageEnemy: {
                if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsVeryHungry(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsVeryThirsty(unit)) { return UnitState.Flee; }
                if (UnitQueries.IsNearEnemy(unit)) { return UnitState.Attack; }
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.EngageEnemy(unit);
                break;
            }
            case UnitState.Attack: {
                if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                if (!UnitQueries.IsNearEnemy(unit)) { return UnitState.EngageEnemy; }
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.Attack(unit);
                break;
            }
            case UnitState.Flee: {
                if (!UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                UnitActions.Flee(unit);
                break;
            }
            case UnitState.Dead: {
                Object.Destroy(unit.gameObject);
                break;
            }
        }
        return unit.unitState; //no state change

        //switch (unit.unitState) {
        //    case UnitState.Wander: {
        //        if (unit.isBeingOverride) {
        //            ////destination too close
        //            if ((unit.destination - unit.transform.position).magnitude <= unit.minDistance) { break; }

        //            unit.Move(unit.destination);
        //            break;
        //        }
        //        //get a new destination if appropriate
        //        if (unit.NeedsDestination()) {
        //            unit.GetDestination();
        //        }

        //        unit.Move(unit.destination);

        //        //TODO: check for blocked path and recalculate destination if so? 

        //        Food targetToEat = unit.CheckForFood(); //enemy nearby?
        //        if (targetToEat != null) {
        //            unit.target = targetToEat.GetComponent<Transform>();
        //            return UnitState.Eat;

        //        }

        //        Unit targetToAttack = unit.CheckForEnemy(); //enemy nearby?
        //        if (targetToAttack != null) {
        //            unit.target = targetToAttack.GetComponent<Transform>();
        //            return UnitState.Attack;

        //        }

        //        break;

        //    }
        //    case UnitState.Attack: {
        //        //todo: maybe set destination to targets transform
        //        if (unit.target == null) //enemy killed
        //        {
        //            unit.attackRange = unit.originalAttackRange;
        //            return UnitState.Wander;
        //        }
        //        //attack enemy
        //        else if ((unit.target.transform.position - unit.transform.position).magnitude <= unit.attackRange) {
        //            unit.attackRange = unit.originalAttackRange + 1;
        //            unit.target.GetComponent<Unit>().TakeDamage(unit.attackDamagePerSecond * Time.fixedDeltaTime);
        //            //TODO: Change it so target destination is set to targets position.
        //            unit.AnimateEat(); //todo: attack animation
        //        } else { unit.Move(unit.target.transform.position); }//chase target source

        //        break;


        //    }
        //    case UnitState.Eat: {
        //        //todo: maybe set destination to targets transform
        //        if (unit.stomachFilledAmount >= unit.stomachSize) //unit is full
        //        {
        //            unit.eatRange = unit.originalEatRange; //reset eatRange
        //            return UnitState.Wander;
        //        } else if ((unit.target.transform.position - unit.transform.position).magnitude <= unit.eatRange) {
        //            unit.eatRange = unit.originalEatRange + 1; //increase eatRange while eating
        //                                                       //start eating until full
        //            unit.stomachFilledAmount += unit.target.GetComponent<Food>().StomachFillPerSecond * Time.fixedDeltaTime;
        //            unit.AnimateEat();

        //        } else { unit.Move(unit.target.transform.position); }//chase food source
        //        break;
        //    }
        //    case UnitState.Dead: {
        //        Object.Destroy(unit.gameObject);
        //        break;
        //    }
        //}
    }

}
