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

        //switch (unit.unitState) {
        //    case UnitState.Wander: {
        //        if (unit.IsThreatened) {
        //            if (unit.ShouldBeAggressive()) {
        //                return UnitState.EngageEnemy;
        //            } else {
        //                return UnitState.Flee;
        //            }
        //        }
        //        if ()
        //    }
        //}

        switch (unit.unitState) {
            case UnitState.Wander: {
                if (unit.isBeingOverride) {
                    ////destination too close
                    if ((unit.destination - unit.transform.position).magnitude <= unit.minDistance) { break; }

                    unit.Move(unit.destination);
                    break;
                }
                //get a new destination if appropriate
                if (unit.NeedsDestination()) {
                    unit.GetDestination();
                }

                unit.Move(unit.destination);

                //TODO: check for blocked path and recalculate destination if so? 

                Food targetToEat = unit.CheckForFood(); //enemy nearby?
                if (targetToEat != null) {
                    unit.target = targetToEat.GetComponent<Transform>();
                    return UnitState.Eat;

                }

                Unit targetToAttack = unit.CheckForEnemy(); //enemy nearby?
                if (targetToAttack != null) {
                    unit.target = targetToAttack.GetComponent<Transform>();
                    return UnitState.Attack;

                }

                break;

            }
            case UnitState.Attack: {
                //todo: maybe set destination to targets transform
                if (unit.target == null) //enemy killed
                {
                    unit.attackRange = unit.originalAttackRange;
                    return UnitState.Wander;
                }
                //attack enemy
                else if ((unit.target.transform.position - unit.transform.position).magnitude <= unit.attackRange) {
                    unit.attackRange = unit.originalAttackRange + 1;
                    unit.target.GetComponent<Unit>().TakeDamage(unit.attackDamagePerSecond * Time.fixedDeltaTime);
                    //TODO: Change it so target destination is set to targets position.
                    unit.AnimateEat(); //todo: attack animation
                } else { unit.Move(unit.target.transform.position); }//chase target source

                break;


            }
            case UnitState.Eat: {
                //todo: maybe set destination to targets transform
                if (unit.stomachFilledAmount >= unit.stomachSize) //unit is full
                {
                    unit.eatRange = unit.originalEatRange; //reset eatRange
                    return UnitState.Wander;
                } else if ((unit.target.transform.position - unit.transform.position).magnitude <= unit.eatRange) {
                    unit.eatRange = unit.originalEatRange + 1; //increase eatRange while eating
                                                               //start eating until full
                    unit.stomachFilledAmount += unit.target.GetComponent<Food>().StomachFillPerSecond * Time.fixedDeltaTime;
                    unit.AnimateEat();

                } else { unit.Move(unit.target.transform.position); }//chase food source
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
