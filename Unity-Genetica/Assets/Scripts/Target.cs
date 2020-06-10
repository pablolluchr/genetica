using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public GameObject targetGameObject;
    public Vector3 targetVector3; 
    public Vector3 obstacleToAvoid;
    public float radius; 
    public float dummyRadius = 0;
    private Unit unit;

    private void Start()
    {
        ResetObstacle();
        targetVector3 = Vector3.zero;
        unit = GetComponent<Unit>();

    }
    public void Update()
    {
        if (unit.dead) return;
        if (targetGameObject) targetVector3 = targetGameObject.transform.position;
        unit.destinationGizmo.transform.position = targetVector3;
    }

    public void Change(GameObject target, float new_radius)
    {
        targetGameObject = target;
        targetVector3 = targetGameObject.transform.position;
        radius = new_radius;
    }

    public void ChangeWithRandomPositionAround(GameObject target,float interactionRadius)
    {
        Vector3 targetPosition = target.transform.position;
        targetGameObject = target;

        //random position in a circle normal to the surface centered in the targets position
        Vector2 randomInCircle = Random.insideUnitCircle.normalized * interactionRadius;
        Vector3 basisX = new Vector3(1, 0, 0);
        Vector3 basisY = new Vector3(0, 1, 0);
        Vector3 basisZ = new Vector3(0, 0, 1);
        Quaternion basisRotation = Quaternion.FromToRotation(basisZ, targetPosition);
        Vector3 rotatedX =  basisRotation * basisX;
        Vector3 rotatedY = basisRotation * basisY;
        targetVector3 = targetPosition + rotatedX * randomInCircle.x + rotatedY * randomInCircle.y;
    }

    public void Change(Vector3 target, float new_radius)
    {
        targetVector3 = target;
        targetGameObject = null;
        radius = new_radius;
    }

    public void ResetObstacle()
    {
        obstacleToAvoid = Vector3.zero;
    }

    public void SetObstacle(Vector3 obstacleToAvoid)
    {
        this.obstacleToAvoid = obstacleToAvoid;
    }

    public void Change(Vector3 target)
    {
        targetVector3 = target;
        targetGameObject = null;
        radius = dummyRadius;

    }

    public bool IsNear()
    {
        return (unit.transform.position - targetVector3).sqrMagnitude <= radius + unit.interactionRadius;
    }
}



    




    
