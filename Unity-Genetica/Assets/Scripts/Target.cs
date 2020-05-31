using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public GameObject targetGameObject;
    public Vector3 targetVector3; 
    public Vector3 obstacleToAvoid;
    public float radius; 
    public float dummyRadius = 0;

    private void Start()
    {
        ResetObstacle();
        targetVector3 = Vector3.zero;
    }
    public void Update()
    {
        if (targetGameObject) targetVector3 = targetGameObject.transform.position;
        GetComponent<Unit>().destinationGizmo.transform.position = targetVector3;
    }

    public void Change(GameObject target, float new_radius)
    {
        targetGameObject = target;
        targetVector3 = targetGameObject.transform.position;
        radius = new_radius;
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

    public bool IsNear(Unit unit,bool increasedRadius)
    {
        float buffer = 0;
        if (increasedRadius) buffer = .1f;
        return (unit.transform.position - targetVector3).magnitude <= radius + unit.interactionRadius+buffer;
    }
}



    




    
