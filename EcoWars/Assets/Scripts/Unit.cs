using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles the target-based movement of units
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour
{

    private Unit target; //unit moves towards target
    public float minDistance = 3f;//stops moving when minDistance reached
    public float maxDistance = 20f; //only follow targets maxDistance appart
    public float speed =1f;
    public float animationSpeed = 10f;
    public float animationTilt = 10f;
    private Rigidbody rb;
    private Vector3 destination = Vector3.zero;
    private GravityAttractor planet;
    private UnitState unitState;
    private float destinationStampTime;
    [SerializeField] private float rotationSpeed = 2f;


    public void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false; //deactivate built-in downwards gravity
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb = GetComponent<Rigidbody>();
    }

    //private void FixedUpdate()
    //{

    //    //move towards target
    //    Vector3 moveDir = target.position - rb.position;
    //    moveDir = Vector3.ProjectOnPlane(moveDir, transform.up); //movement only tangent to the planet

    //    if (moveDir.magnitude > minDistance && moveDir.magnitude < maxDistance) //target in reach and not too close
    //    {
    //        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

    //        //only change rotation of y axis
    //        transform.rotation = targetRotation;

    //        //move forward in the local axis
    //        rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * speed);
    //    }

    //    //effect of gravity
    //    planet.Attract(transform);
    //}

    private void FixedUpdate()
    {

        switch (unitState)
        {
            case UnitState.Wander:
                {
                    if (NeedsDestination())
                    {
                        GetDestination();
                    }

                    //only rotate normal to the planet
                    Vector3 projectedDestination = Vector3.ProjectOnPlane(destination, transform.up);
                    Quaternion targetRotation = Quaternion.LookRotation(projectedDestination, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
                    //move forward in the local axis
                    rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * speed);

                    //TODO: check for blocked path and recalculate destination if so? 

                    //TODO: CheckForFood. same as below.

                    //todo: maybe consider chase state (to chase food and enemy?) and once in reach transition to eat or attack

                    Food targetToEat = CheckForFood(); //enemy nearby?
                    if (targetToEat != null)
                    {
                        unitState = UnitState.Eat;

                    }

                    Unit targetToAggro = CheckForAggro(); //enemy nearby?
                    if (targetToAggro != null)
                    {
                        target = targetToAggro;
                        unitState = UnitState.Attack;

                    }

                    break;

                }
            case UnitState.Attack:
                {
                    break;

                }
            case UnitState.Eat:
                {
                    break;

                }


        }

        //affect gravity
        planet.Attract(transform);

        //animate movement
        Transform prefab = this.gameObject.transform.GetChild(0);
        //the z rotation goes frrom -animationTilt to animationTilt according to a sine.
        float currentRotation = Mathf.Sin(Time.time * animationSpeed) * animationTilt;
        prefab.localRotation = Quaternion.Euler(new Vector3(0, 0, currentRotation));

    }

    //does the unit require to be given a new destination
    private bool NeedsDestination()
    {
        //no destination
        if(destination == Vector3.zero){ return true;}

        ////destination too close
        //Vector3 moveDir = target.GetComponent<Transform>().position - rb.position;
        //moveDir = Vector3.ProjectOnPlane(moveDir, transform.up); //movement only tangent to the planet
        if ((destination-transform.position).magnitude <= minDistance)
        {
            return true;
        }

        //if its wandering and couldn't reach the destination in 10 sec
        if (Time.time -destinationStampTime > 10f && unitState == UnitState.Wander) { return true; }
        //otherwise
        return false;
    }

    //Find a random point in front 
    private void GetDestination()
    {
        //random position somewhere on the surface of the planet
        destination = UnityEngine.Random.onUnitSphere * 5f;
        destinationStampTime = Time.time;

    }

    //Check for enemy units in range
    private Unit CheckForAggro()
    {
        return null;
    }

    //Check for enemy units in range
    private Food CheckForFood()
    {
        return null;
    }
}

public enum UnitState
{
    Wander,
    Eat,
    Attack
}