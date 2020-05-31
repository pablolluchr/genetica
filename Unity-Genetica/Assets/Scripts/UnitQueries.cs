using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitQueries {

    // unit behaviour query functions here (not allowed to modify state)

    public static bool AreaGraphicsState()
    {
        return GameManager.gameManager.areaGraphic.activeSelf;
    }

    #region hunger // ################################################################################

    public static bool IsFed(Unit unit) {
        return unit.amountFed / unit.maxFed >= 0.99;
    }

    public static bool IsHungry(Unit unit) {
        return unit.hungry;
    }

    public static bool IsVeryHungry(Unit unit) {
        return unit.amountFed / unit.maxFed <= unit.criticalHunger;
    }

    public static bool SeesFood(Unit unit) {
        return ClosestFoodInView(unit) != null;
    }

    // null if no food in viewing range
    public static GameObject ClosestFoodInView(Unit unit) {
        List<GameObject> foods = GameManager.gameManager.foodList; //TODO: add sources to list
        List<GameObject> nonEmptyFoods = UnitHelperFunctions.FilterEmptyFoods(foods); 
        return UnitHelperFunctions.GetClosestInView(unit, nonEmptyFoods);
    }

    #endregion

    #region thirst // ################################################################################

    public static bool IsQuenched(Unit unit) {
        return unit.amountQuenched / unit.maxQuenched >= 0.99;
    }

    public static bool IsThirsty(Unit unit) {
        return unit.thirsty;
    }

    public static bool IsVeryThirsty(Unit unit) {
        return unit.amountQuenched / unit.maxQuenched <= unit.criticalThirst;
    }

    public static bool SeesWater(Unit unit) {

        Vector3 closestWaterSource = UnitQueries.ClosestWaterSource(unit);
        return (closestWaterSource - unit.transform.position).magnitude <= unit.viewDistance;
    }

    public static Vector3 ClosestWaterSource(Unit unit) {
        List<GameObject> waterSources = GameManager.gameManager.waterList;

        Vector3 closestSource = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        foreach (var waterSource in waterSources) {
            Vector3 waterSourcePosition = NearestVertexTo(waterSource, unit.transform.position);

            //Vector3 waterSourcePosition = waterSource.GetComponent<Collider>().ClosestPoint(unit.transform.position);
            if ((waterSourcePosition - unit.transform.position).magnitude < closestDistance) {
                closestDistance = (waterSourcePosition - unit.transform.position).magnitude;
                closestSource = waterSourcePosition;
            }
        }
        if (closestSource == Vector3.zero) throw new System.Exception("No water found");
        unit.transform.Find("ClosestWaterGizmo").transform.position = closestSource;

        return closestSource;
    }

    public static Vector3 NearestVertexTo(GameObject source, Vector3 point)
    {
        // convert point to local space
        point = source.transform.InverseTransformPoint(point);


        Mesh mesh = source.GetComponent<MeshFilter>().mesh;
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        // convert nearest vertex back to world space
        return source.transform.TransformPoint(nearestVertex);

    }

    #endregion

    #region mating // ################################################################################

    public static bool IsHorny(Unit unit) {
        return unit.horny;
    }

    public static bool SeesMate(Unit unit) {
        return ClosestMateInView(unit) != null;
    }

    // returns null if does not see a mate
    public static GameObject ClosestMateInView(Unit unit) {
        List<Unit> pets = GameManager.gameManager.petList;
        List<Unit> hornyPets = UnitHelperFunctions.FilterUnmatable(unit, pets);
        List<GameObject> hornyPetsGameObject = new List<GameObject>();
        foreach (var enemy in hornyPets) hornyPetsGameObject.Add(enemy.gameObject);
        return UnitHelperFunctions.GetClosestInView(unit, hornyPetsGameObject);
    }

    #endregion

    #region genetium // ################################################################################

    public static bool SeesGenetium(Unit unit) {
        if (unit.gameObject.tag == "Hostile") { return false; }
        return ClosestGenetiumInView(unit) != null;
    }

    public static GameObject ClosestGenetiumInView(Unit unit) {
        List<GameObject> genetiums = GameManager.gameManager.genetiumList;
        List<GameObject> nonEmptyGenetiums = UnitHelperFunctions.FilterEmptyGenetium(genetiums);
        return UnitHelperFunctions.GetClosestInView(unit, nonEmptyGenetiums);
    }

    public static bool IsCarryingGenetium(Unit unit) {
        return unit.currentGenetiumAmount / unit.carryingCapacity >= 0.01;
    }

    public static bool IsStorageFull(Unit unit) {
        return unit.currentGenetiumAmount / unit.carryingCapacity >= 0.99;
    }

    public static bool IsNearTarget(Unit unit, bool increasedRadius) {
        return unit.GetComponent<Target>().IsNear(unit, increasedRadius);
    }

    #endregion

    #region attacking // ################################################################################

    public static bool IsThreatened(Unit unit) {
        return ClosestEnemyInThreatRange(unit) != null;
    }

    // null if not in range
    public static GameObject ClosestEnemyInThreatRange(Unit unit) {
        List<Unit> enemies;
        if (unit.enemyTag == "Hostile")
            enemies = GameManager.gameManager.enemyList;
        else enemies = GameManager.gameManager.petList;

        List<GameObject> enemiesGameObject = new List<GameObject>();
        foreach (var enemy in enemies) enemiesGameObject.Add(enemy.gameObject);

        //List<Unit> aliveEnemies = UnitHelperFunctions.FilterDeadEnemies(enemies);
        //TODO: units should be taken out of the gameobject array juust when they die
        return UnitHelperFunctions.GetClosestInRange(unit, enemiesGameObject, unit.enemyDetectionRange);
    }

   

    public static bool ShouldBeAggressive(Unit unit) {
        float random = Random.Range(0f, 1f);
        return random < unit.aggression;
    }

    public static bool HasLowHealth(Unit unit) {
        return unit.health / unit.maxHealth <= unit.criticalHealth;
    }

    #endregion

    //does the unit require to be given a new destination
    public static bool NeedsWanderingDestination(Unit unit) {
        //no destination
        if (unit.GetComponent<Target>().targetVector3 == Vector3.zero) { return true; }

        ////destination already reached
        if (UnitQueries.IsNearTarget(unit, false)) { return true; }
        //if its wandering and couldn't reach the destination in 10 sec reset 
        if (Time.time - unit.wanderTimeStamp > 3f && unit.unitState == UnitState.Wander) { return true; }

        //otherwise
        return false;
    }

    public static bool NeedsChange(Unit unit) {
        return unit.needsChange;
    }
}