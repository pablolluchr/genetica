using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitQueries {

    // unit behaviour query functions here (not allowed to modify state)

    //public static bool AreaGraphicsState()
    //{
    //    return GameManager.gameManager.areaGraphic.activeSelf;
    //}

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
        return ClosestFoodInArea(unit) != null;
    }

    // null if no food in viewing range
    public static GameObject ClosestFoodInArea(Unit unit) {
        List<GameObject> foods = GameManager.gameManager.foodList; //TODO: add sources to list
        List<GameObject> nonEmptyFoods = UnitHelperFunctions.FilterEmptyFoods(foods); 
        return UnitHelperFunctions.GetClosetInAreaRange(unit, nonEmptyFoods);
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


        Vector3 closestWaterSource = UnitQueries.ClosestWater(unit);
        return closestWaterSource != Vector3.zero;
    }

    public static Vector3 ClosestWater(Unit unit) {

        //where water proximity is calculated against
        Vector3 center = unit.transform.position;
        //TODO: consider if water should be taken from the closest point to the food source
        //if (unit.foodSource != null) center = unit.foodSource.transform.position;

        List<Vector3> vertices = GetWaterVertices();

        Vector3 closestVertex = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        //Vector3 unitPosition = unit.transform.position;
        foreach (var vertex in vertices) {
                float distance = Vector3.SqrMagnitude(vertex - center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVertex = vertex;
                }
        }
        unit.transform.Find("ClosestWaterGizmo").transform.position = closestVertex;
        return closestVertex;
    }

    public static List<Vector3> GetWaterVertices()
    {

        MeshFilter water = GameManager.gameManager.water;
        List<Vector3> vertices = new List<Vector3>(water.mesh.vertices);
        List<Vector3> globalVertices = new List<Vector3>();
        foreach (var v in vertices)
        {
            globalVertices.Add(water.transform.TransformPoint(v));
        }
        return globalVertices;
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
        return ClosestMateInArea(unit) != null;
    }

    // returns null if does not see a mate
    public static GameObject ClosestMateInArea(Unit unit) {
        List<Unit> pets = GameManager.gameManager.petList;
        List<Unit> hornyPets = UnitHelperFunctions.FilterUnmatable(unit, pets);
        List<GameObject> hornyPetsGameObject = new List<GameObject>();
        foreach (var enemy in hornyPets) hornyPetsGameObject.Add(enemy.gameObject);
        return UnitHelperFunctions.GetClosetInAreaRange(unit, hornyPetsGameObject);
    }

    #endregion

    #region genetium // ################################################################################

    public static bool SeesGenetium(Unit unit) {
        if (unit.gameObject.tag == "Hostile") { return false; }
        return ClosestGenetiumInArea(unit) != null;
    }

    public static GameObject ClosestGenetiumInArea(Unit unit) {
        List<GameObject> genetiums = GameManager.gameManager.genetiumList;
        List<GameObject> nonEmptyGenetiums = UnitHelperFunctions.FilterEmptyGenetium(genetiums);
        return UnitHelperFunctions.GetClosetInAreaRange(unit, nonEmptyGenetiums);
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
        return UnitHelperFunctions.GetClosetInAreaRange(unit, enemiesGameObject);
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