using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles the target-based movement of units
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour
{

    private Transform target; //unit moves towards target
    public float minDistance = 3f;//stops moving when minDistance reached
    public float maxDistance = 20f; //only follow targets maxDistance appart
    public float speed =1f;
    public float walkAnimationSpeed = 10f;
    [SerializeField] private float eatAnimationSpeed = 1f;
    public float animationTilt = 10f;
    private Rigidbody rb;
    private Vector3 destination = Vector3.zero;
    private GravityAttractor planet;
    private UnitState unitState;
    private float destinationStampTime;
    public float foodRange=5f;
    public float originalEatRange = 1f;
    public float eatRange=1f;
    [SerializeField] private float stomachSize = 10f;
    [SerializeField] private float stomachDecreasePerSecond = 0.1f;
    private float stomachFilledAmount = 10f; //how much of the stomach is filled
    [SerializeField] private float hungerThreshold = 5f;
    [SerializeField] private float rotationSpeed = 2f;


    public void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false; //deactivate built-in downwards gravity
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb = GetComponent<Rigidbody>();
    }

     
    private void FixedUpdate()
    {
        //get a bit hungry
        stomachFilledAmount -= stomachDecreasePerSecond * Time.fixedDeltaTime;

        if (stomachFilledAmount < 0)
        {
            unitState = UnitState.Dead;
        }

        switch (unitState)
        {
            case UnitState.Wander:
                {
                    //get a new destination if appropriate
                    if (NeedsDestination())
                    {
                        GetDestination();
                    }

                    Move(destination);

                    //TODO: check for blocked path and recalculate destination if so? 

                    //todo: maybe consider chase state (to chase food and enemy?) and once in reach transition to eat or attack

                    Food targetToEat = CheckForFood(); //enemy nearby?
                    if (targetToEat != null)
                    {
                        target = targetToEat.GetComponent<Transform>();
                        unitState = UnitState.Eat;

                    }

                    Unit targetToAggro = CheckForAggro(); //enemy nearby?
                    if (targetToAggro != null)
                    {
                        target = targetToAggro.GetComponent<Transform>();
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
                    if (stomachFilledAmount >= stomachSize) //unit is full
                    {
                        eatRange = originalEatRange; //reset eatRange
                        unitState = UnitState.Wander;
                    }
                    else if ((target.transform.position - transform.position).magnitude <= eatRange)
                    {
                        eatRange = originalEatRange+1; //increase eatRange while eating
                        //start eating until full
                        stomachFilledAmount += target.GetComponent<Food>().StomachFillPerSecond * Time.fixedDeltaTime;
                        AnimateEat();

                    }
                    else{Move(target.transform.position);}//chase food source
                    break;
                }
            case UnitState.Dead:
                {
                    Object.Destroy(this.gameObject);
                    break;
                }
        }
    }

    //Rotate, move unit towards destination, affect gravity and animate
    private void Move(Vector3 destination)
    {
        //only rotate normal to the planet
        Vector3 projectedDestination = Vector3.ProjectOnPlane(destination, transform.up);
        Quaternion targetRotation = Quaternion.LookRotation(projectedDestination, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

        //move forward in the local axis
        rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * speed);


        //affect gravity
        planet.Attract(transform);

        //animate movement
        AnimateWalk();

    }

    //does the unit require to be given a new destination
    private bool NeedsDestination()
    {
        //no destination
        if(destination == Vector3.zero){ return true;}

        ////destination too close
        if ((destination-transform.position).magnitude <= minDistance){return true;}

        //if its wandering and couldn't reach the destination in 10 sec reset 
        if (Time.time -destinationStampTime > 10f && unitState == UnitState.Wander) { return true; }

        //otherwise
        return false;
    }

    //Find a random point in planet's surface 
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
        if(stomachFilledAmount > hungerThreshold) { return null; } //don't look for food if not hungry
        //TODO: check for efficiency. Is it iterating through all gameobjects in scene?
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        //find closest source of food and return it
        float closestDistance = Mathf.Infinity;
        Food closestFood = null;
        foreach (GameObject food in foods)
        {
            float distance = (transform.position - food.transform.position).magnitude;
            if (distance < foodRange && distance<closestDistance)
            {
                closestDistance = distance;
                closestFood= food.GetComponent<Food>();
            }
        }

        return closestFood;
    }

    private void AnimateWalk()
    {
        Transform prefab = this.gameObject.transform.GetChild(0);

        //reset position from eat animation
        prefab.localPosition = Vector3.Lerp(prefab.localPosition, new Vector3(0f, 0f, 0f),Time.deltaTime);

        //the z rotation goes frrom -animationTilt to animationTilt according to a sine.
        float currentRotation = Mathf.Sin(Time.time * walkAnimationSpeed) * animationTilt;
        prefab.localRotation = Quaternion.Euler(new Vector3(0, 0, currentRotation));
        return;
    }

    private void AnimateEat()
    {
        Transform prefab = this.gameObject.transform.GetChild(0);

        //reset rotation from walk animation
        prefab.localRotation = Quaternion.Lerp(prefab.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime);

        //jump animation
        float jumpHeight = Mathf.Abs(Mathf.Cos(Time.time * eatAnimationSpeed) * 2f)-.5f;
        prefab.localPosition = Vector3.Lerp(prefab.localPosition,new Vector3(0f, jumpHeight, 0f),Time.deltaTime*eatAnimationSpeed);
        return;
    }
}

public enum UnitState
{
    Wander,
    Eat,
    Attack,
    Dead
}