using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPoint : MonoBehaviour
{
    public float radius;
    public float quenchRate;
    
    void OnDrawGizmos() {
        Gizmos.color = new Color(0, 0, 1, 0.4f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
