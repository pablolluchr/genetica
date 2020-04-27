using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitStateMachine
{
    

    public static UnitState NextState(Unit unit) //returns next state
    {

        //get a bit hungry
        unit.stomachFilledAmount -= unit.stomachDecreasePerSecond * Time.fixedDeltaTime;

        if (unit.stomachFilledAmount < 0)
        {
            return UnitState.Dead;
        }

        switch (unit.unitState)
        {
            case UnitState.Wander:
                {
                    if (unit.isBeingOverride)
                    {
                        ////destination too close TODO: create idle state and transition to that state
                        if ((unit.destination - unit.transform.position).magnitude <= unit.minDistance) { break; }

                        unit.Move(unit.destination);
                        break;
                    }
                    //get a new destination if appropriate
                    if (unit.NeedsDestination())
                    {
                        unit.GetDestination();
                    }

                    unit.Move(unit.destination);

                    //TODO: check for blocked path and recalculate destination if so? 

                    Food targetToEat = unit.CheckForFood(); //enemy nearby?
                    if (targetToEat != null)
                    {
                        unit.target = targetToEat.GetComponent<Transform>();
                        return UnitState.Eat;

                    }

                    Unit targetToAttack = unit.CheckForEnemy(); //enemy nearby?
                    if (targetToAttack != null)
                    {
                        unit.target = targetToAttack.GetComponent<Transform>();
                        return UnitState.Attack;

                    }

                    break;

                }
            case UnitState.Attack:
                {
                    //todo: maybe set destination to targets transform
                    if (unit.target == null) //enemy killed
                    {
                        unit.attackRange = unit.originalAttackRange;
                        return UnitState.Wander;
                    }
                    //attack enemy
                    else if ((unit.target.transform.position - unit.transform.position).magnitude <= unit.attackRange)
                    {
                        unit.attackRange = unit.originalAttackRange + 1;
                        unit.target.GetComponent<Unit>().TakeDamage(unit.attackDamagePerSecond * Time.fixedDeltaTime);
                        //destination = target.transform.position //TODO: CHange it welll
                        unit.AnimateEat(); //todo: attack animation
                    }
                    else { unit.Move(unit.target.transform.position); }//chase target source

                    break;


                }
            case UnitState.Eat:
                {
                    //todo: maybe set destination to targets transform
                    if (unit.stomachFilledAmount >= unit.stomachSize) //unit is full
                    {
                        unit.eatRange = unit.originalEatRange; //reset eatRange
                        return UnitState.Wander;
                    }
                    else if ((unit.target.transform.position - unit.transform.position).magnitude <= unit.eatRange)
                    {
                        unit.eatRange = unit.originalEatRange + 1; //increase eatRange while eating
                        //start eating until full
                        unit.stomachFilledAmount += unit.target.GetComponent<Food>().StomachFillPerSecond * Time.fixedDeltaTime;
                        unit.AnimateEat();

                    }
                    else { unit.Move(unit.target.transform.position); }//chase food source
                    break;
                }
            case UnitState.Dead:
                {
                    Object.Destroy(unit.gameObject);
                    break;
                }
                
        }
        return unit.unitState; //no state change
    }

}
