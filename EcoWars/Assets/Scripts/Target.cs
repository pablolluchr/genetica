using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target : MonoBehaviour {
    public GameObject targetGameObject;
    public Vector3 targetVector3; //inits to vector3.zero
    public float radius; 
    public float dummyRadius = .5f;

    public void Update()
    {
        if (targetGameObject) targetVector3 = targetGameObject.transform.position;
        transform.Find("Destination").transform.position = targetVector3;
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
    public void Change(Vector3 target)
    {
        targetVector3 = target;
        targetGameObject = null;
        radius = dummyRadius;

    }

    public bool IsNear(Unit unit)
    {
        return (unit.transform.position - targetVector3).magnitude <= radius;
    }
}



    




    
