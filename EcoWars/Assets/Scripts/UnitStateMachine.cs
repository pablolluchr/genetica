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
                if (unit.IsThreatened()) {
                    if (unit.ShouldBeAggressive()) {
                        return UnitState.EngageEnemy;
                    } else {
                        return UnitState.Flee;
                    }
                }
                if (unit.IsThreatened()   && unit.CanSeeWater()) { return UnitState.MoveToWater; }
                if (unit.IsHungry()       && unit.CanSeeFood())  { return UnitState.MoveToFood; }
                if (unit.IsCarryingFuel() || unit.NeedsChange()) { return UnitState.MoveToBase; }
                if (unit.IsHorny()        && unit.SeesMate())    { return UnitState.MoveToMate; }
                if (unit.SeesFuel())                             { return UnitState.MoveToFuel; }
                unit.Wander();
                break;
            }
            case UnitState.MoveToWater: {
                if (unit.IsThreatened()) { return UnitState.Wander; }
                if (unit.IsNearWater())    { return UnitState.Drink; }
                break;
            }
            case UnitState.Drink: {
                if (unit.IsThreatened() || unit.IsQuenched()) { return UnitState.Wander; }
                break;
            }
            case UnitState.MoveToFood: {
                if (unit.IsThreatened()) { return UnitState.Wander; }
                if (unit.IsNearFood())     { return UnitState.Eat; }
                break;
            }
            case UnitState.Eat: {
                if (unit.IsThreatened() || unit.IsFed()) { return UnitState.Wander; }
                break;
            }
            case UnitState.MoveToMate: {
                if (unit.IsThreatened()) { return UnitState.Wander; }
                if (unit.IsNearMate())     { return UnitState.Mate; }
                break;
            }
            case UnitState.Mate: {
                if (unit.IsThreatened() || unit.HasMadeBaby()) { return UnitState.Wander; }
                break;
            }
            case UnitState.MoveToFuel: {
                if (unit.IsThreatened()) { return UnitState.Wander; }
                if (unit.IsNearFuel())     { return UnitState.Harvest; }
                break;
            }
            case UnitState.Harvest: {
                if (unit.IsThreatened() || unit.IsFuelSourceEmpty() || unit.IsStorageFull()) { return UnitState.Wander; }
                break;
            }
            case UnitState.MoveToBase: {
                if (unit.IsThreatened() || unit.IsNearBase()) { return UnitState.Wander; }
                break;
            }
            case UnitState.EngageEnemy: {
                if (unit.HasLowHealth() || unit.IsVeryHungry() || unit.IsVeryThirsty()) { return UnitState.Flee; }
                if (unit.IsNearEnemy())                                                 { return UnitState.Attack; }
                break;
            }
            case UnitState.Attack: {
                if (unit.HasLowHealth())  { return UnitState.Flee; }
                if (!unit.IsNearEnemy())  { return UnitState.EngageEnemy; }
                if (!unit.IsThreatened()) { return UnitState.Wander; }
                break;
            }
            case UnitState.Flee: {
                if (!unit.IsThreatened()) { return UnitState.Wander; }
                break;
            }
            case UnitState.Dead: {
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
