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
        //todo: add actions that happen from every state here!

        if (unit.dead) { UnitActions.Die(unit); return UnitState.Dead; }

        switch (unit.unitState) {
            case UnitState.Wander: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    //rethink this
                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    if (UnitQueries.NeedsChange(unit)) { return UnitState.TargetBase; } //TODO this should probably override everything cause it seems like a bug otherwise
                    if (UnitQueries.IsHungry(unit) && UnitQueries.FoodSourceAvailable(unit))
                        { UnitActions.TargetFood(unit); return UnitState.TargetFood; }
                    if (UnitQueries.IsThirsty(unit)) { UnitActions.TargetWater(unit); return UnitState.TargetWater; }
                    if (UnitQueries.IsHorny(unit) && UnitQueries.AvailableMate(unit)) { return UnitState.TargetMate; }
                    if (UnitQueries.GenetiumAvailable(unit) && !UnitQueries.IsStorageFull(unit)) { //todo: make funuction is genetium available? (unit storage not full, genetium enough genetium, genetium chosen in species)
                        UnitActions.TargetGenetium(unit); return UnitState.TargetGenetium;
                    }
                    if (UnitQueries.IsCarryingGenetium(unit)) { return UnitState.TargetBase; }
                    UnitActions.Wander(unit);
                    break;
                }
            case UnitState.TargetWater: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    //if (!UnitQueries.SeesWater(unit)) { return UnitState.Wander; }
                    if (UnitQueries.IsNearTarget(unit)) { return UnitState.Drink; }

                    break;
                }
            case UnitState.TargetFood: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    //todo handle change of food source as its targetting
                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    //if (!UnitQueries.SeesFood(unit)) { return UnitState.Wander; }
                    if (UnitQueries.IsNearTarget(unit)) { return UnitState.Eat; }
                    break;
                }
            case UnitState.TargetMate: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    UnitActions.TargetMate(unit);
                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    if (!UnitQueries.AvailableMate(unit)) { return UnitState.Wander; }
                    if (UnitQueries.IsNearTarget(unit)) { return UnitState.Mate; }
                    break;
                }
            case UnitState.TargetGenetium: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    //if (UnitQueries.IsThreatened(unit)) { return UnitState.Wander; }
                    if (UnitQueries.IsHungry(unit) && UnitQueries.FoodSourceAvailable(unit))
                        { UnitActions.TargetFood(unit); return UnitState.TargetFood; }
                    if (UnitQueries.IsThirsty(unit)) { UnitActions.TargetWater(unit); return UnitState.TargetWater; }
                    //todo handle change of genetium source as its targetting
                    if (UnitQueries.IsNearTarget(unit)) { return UnitState.Harvest; }

                    break;
                }
            case UnitState.TargetBase: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    UnitActions.TargetBase(unit);
                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    if (UnitQueries.IsHungry(unit) && UnitQueries.FoodSourceAvailable(unit)) { UnitActions.TargetFood(unit); return UnitState.TargetFood; }
                    if (UnitQueries.IsThirsty(unit)) { UnitActions.TargetWater(unit); return UnitState.TargetWater; }
                    if (UnitQueries.IsNearTarget(unit)) { UnitActions.ReachBase(unit); return UnitState.Wander; }
                    break;
                }
            case UnitState.TargetEnemy: {
                    UnitActions.TargetEnemy(unit);
                    //if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                    //if (UnitQueries.IsVeryHungry(unit)) { return UnitState.Flee; }
                    //if (UnitQueries.IsVeryThirsty(unit)) { return UnitState.Flee; }
                    if (UnitQueries.IsNearTarget(unit)) { return UnitState.Attack; }
                    if (!UnitQueries.EnemyInRange(unit)) { return UnitState.Wander; }
                    break;
                }
            //todo: change food and genetium to be near the known food target
            case UnitState.Drink: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    if (UnitQueries.IsQuenched(unit)) { UnitActions.TurnQuenched(unit); return UnitState.Wander; }
                    UnitActions.Drink(unit);
                    break;
                }
            case UnitState.Eat: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    //todo handle change of food source as its eating
                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    if (UnitQueries.IsFed(unit)) { UnitActions.TurnFed(unit); return UnitState.Wander; }
                    UnitActions.Eat(unit);
                    break;
                }
            case UnitState.Mate: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    if (!UnitQueries.IsHorny(unit)) { return UnitState.Wander; }
                    UnitActions.Mate(unit);
                    break;
                }
            case UnitState.Harvest: {
                    if (unit.isBeingOverride) return UnitState.Override;

                    //todo handle change of genetium source as its harvesting
                    if (UnitQueries.EnemyInRange(unit)) return UnitState.TargetEnemy;
                    if (!UnitQueries.IsNearTarget(unit)) { return UnitState.Wander; }
                    if (UnitQueries.IsStorageFull(unit)) { return UnitState.Wander; }
                    if (!UnitQueries.GenetiumAvailable(unit)) { return UnitState.Wander; }
                    UnitActions.Harvest(unit);
                    break;
                }
            case UnitState.Attack: {
                    //if (UnitQueries.HasLowHealth(unit)) { return UnitState.Flee; }
                    if (!UnitQueries.EnemyInRange(unit)) { return UnitState.Wander; }
                    if (!UnitQueries.IsNearTarget(unit)) { return UnitState.TargetEnemy; }
                    UnitActions.Attack(unit);
                    break;
                }
            case UnitState.Flee: {
                    UnitActions.Flee(unit);
                    if (!UnitQueries.EnemyInRange(unit)) { return UnitState.Wander; }
                    break;
                }
            case UnitState.Override: {
                    if (UnitQueries.IsNearTarget(unit)) {
                        UnitActions.StopOverride(unit); return UnitState.Wander; }
                    if (UnitQueries.EnemyInRange(unit)) { return UnitState.TargetEnemy;}

                    break;
                }
        }

        return unit.unitState; //no state change

    }

}
