using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles the target-based movement of units
[RequireComponent(typeof(Rigidbody))]
public class UnitMovement : MonoBehaviour
{

    public Transform target; //unit moves towards target
    public float minDistance = 1f;//stops moving when minDistance reached
    public float maxDistance = 6f; //only follow targets maxDistance appart
    public float speed;
    private Rigidbody rb;
    private GravityAttractor planet;


    public void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false; //deactivate built-in downwards gravity
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = target.position - rb.position;
        moveDir = Vector3.ProjectOnPlane(moveDir, transform.up); //movement only tangent to the planet

        if (moveDir.magnitude > minDistance && moveDir.magnitude < maxDistance) //target in reach and not too close
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

            //only change rotation of y axis
            transform.rotation = targetRotation;

            //move forward in the local axis
            rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * speed);
        }

        planet.Attract(transform);
    }
}
