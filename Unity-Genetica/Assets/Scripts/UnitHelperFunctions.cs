using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitHelperFunctions {
    public static GameObject GetClosest(Unit unit, List<GameObject> objects) {
        float closestDistance = Mathf.Infinity;
        GameObject closestObj = null;
        foreach (GameObject obj in objects) {
            float distance = (unit.transform.position - obj.transform.position).magnitude;
            if (distance < closestDistance) {
                closestDistance = distance;
                closestObj = obj;
            }
        }
        return closestObj;
    }

    // null if not in view
    public static GameObject GetClosestInRange(Unit unit, List<GameObject> objects, float range) {
        GameObject closestObj = GetClosest(unit, objects);
        if (closestObj == null) return null;
        float distance = (unit.transform.position - closestObj.transform.position).magnitude;
        if (distance < range) {
            return closestObj;
        } else {
            return null;
        }
    }

    // null if not in view
    public static GameObject GetClosestInView(Unit unit, List<GameObject> objects) {
        return GetClosestInRange(unit, objects, unit.viewDistance);
    }

    public static List<Unit> FilterUnmatable(Unit unit, List<Unit> pets) {
        List<Unit> hornyPets = new List<Unit>();
        foreach (Unit pet in pets) {
            if (
                pet &&
                pet.horny &&
                pet != unit &&
                pet.speciesName == unit.speciesName
            ) {
                hornyPets.Add(pet);
            }
        }
        return hornyPets;
    }

    public static List<GameObject> FilterEmptyFoods(List<GameObject> foods) {
        List<GameObject> nonEmptyFoods = new List<GameObject>();
        foreach (GameObject food in foods) {
            Food foodComponent = food.GetComponent<Food>();
            if (foodComponent.availableFood > foodComponent.consideredEmpty) {
                nonEmptyFoods.Add(food);
            }
        }
        return nonEmptyFoods;
    }

    public static List<GameObject> FilterEmptyGenetium(List<GameObject> genetiums) {
        List<GameObject> nonEmptyGenetiums = new List<GameObject>();
        foreach (GameObject genetium in genetiums) {
            Genetium genetiumComponent = genetium.GetComponent<Genetium>();
            if (genetiumComponent.currentAmount > genetiumComponent.consideredEmpty) {
                nonEmptyGenetiums.Add(genetium);
            }
        }
        return nonEmptyGenetiums;
    }

    public static GameObject[] FilterDeadEnemies(GameObject[] enemies) {
        List<GameObject> aliveEnemies = new List<GameObject>();
        foreach (GameObject enemy in enemies) {
            Unit enemyComponent = enemy.GetComponent<Unit>();
            if (!enemyComponent.dead) {
                aliveEnemies.Add(enemy);
            }
        }
        return aliveEnemies.ToArray();
    }

    public static GameObject[] FilterSpecies(GameObject[] pets, string speciesName) {
        List<GameObject> species = new List<GameObject>();
        foreach (GameObject pet in pets) {
            Unit petComponent = pet.GetComponent<Unit>();
            if (petComponent.speciesName == speciesName) {
                species.Add(pet);
            }
        }
        return species.ToArray();
    }
    
    // points are defined in order of input [input, output]
    //  s0      s1       s2      s3
    // <-- [0] <--> [1] <--> [2] -->
    //    point0   point1   point2
    // length: 3
    // usage: UnitHelperFunctions.Interpolate(3, new float[,] {{-1,1}, {2,3}, {6,2}}) ---> 2.75
    public static float Interpolate(float value, float[,] points) {
        int section = 0; // section 0 is before the first point, section 1 is between point 0 and 1 etc.
        // loop until one after the points
        while (section <= points.GetLength(0)) {
            float[] leftPoint;
            float[] rightPoint;
            // get the left and right points for current section
            if (section == 0) {
                leftPoint  = new float[2] {-Mathf.Infinity,        points[section,     1]};
                rightPoint = new float[2] {points[section,     0], points[section,     1]};
            } else if (section == points.GetLength(0)) {
                leftPoint  = new float[2] {points[section - 1, 0], points[section - 1, 1]};
                rightPoint = new float[2] {Mathf.Infinity,         points[section - 1, 1]};
            } else {
                leftPoint  = new float[2] {points[section - 1, 0], points[section - 1, 1]};
                rightPoint = new float[2] {points[section,     0], points[section,     1]};
            }
            // calculate slope and offset
            float leftx = leftPoint[0];
            float lefty = leftPoint[1];
            float rightx = rightPoint[0];
            float righty = rightPoint[1];
            float slope;
            float offset;
            if (leftx == -Mathf.Infinity || rightx == Mathf.Infinity) { 
                slope = 0;
                offset = lefty;
            } else {
                slope = (righty - lefty) / (rightx - leftx);
                offset = lefty - slope * leftx;
            }
            // if not in correct section, go to next
            if (leftx <= value && value <= rightx) {
                return slope * value + offset;
            }
            section += 1;
        }

        throw new System.Exception("Something went wrong in the interpolation");
    }
}
