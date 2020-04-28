using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles the target-based movement of units
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour {

    [System.NonSerialized] public GameObject target; //food, enemy or biofuel source
    public float minDistance = 3f;//stops moving when minDistance reached
    public float maxDistance = 20f; //only follow targets maxDistance appart
    [Range(.5f, 3.0f)] public float speed = 1f;
    public float walkAnimationSpeed = 10f;
    public float eatAnimationSpeed = 1f;
    public float animationTilt = 10f;
    [Range(.0f, 1.0f)] public float legsLength=.2f;
    [Range(.0f, 1.0f)] public float bodySize=.1f;
    [Range(.0f, 1.0f)] public float headSize=.2f;
    [System.NonSerialized] public Rigidbody rb;
    public float maxHealth = 10f;
    [System.NonSerialized] public float health;
    [System.NonSerialized] public bool isBeingOverride;
    [System.NonSerialized] public Vector3 destination = Vector3.zero;
    [System.NonSerialized] public GravityAttractor planet;
    public UnitState unitState;
    [System.NonSerialized] public float destinationStampTime;
    public float viewDistance = 5f;
    [SerializeField] public float attackDamagePerSecond = 1f;
    [SerializeField] public float enemyDetectionRange = 10f;
    public float originalAttackRange = 2f;
    public float attackRange = 2f;
    [System.NonSerialized] public float eatRange = 1f;
    [System.NonSerialized] public string enemyTag;
    [System.NonSerialized] public int maxUnits = 100;


    // eating
    public float amountFed; //how much of the stomach is filled
    public float maxFed;
    public float feedingPerSecond;
    public float hungerPerSecond;
    public float hungerChanceExponent;
    public float hungerDamage;
    public float eatingDistance;

    // drinking
    public float amountQuenched = 10f; //how much of the stomach is filled
    public float maxQuenched = 10f;
    public float thirstPerSecond = 0.5f;
    public float thirstThreshold;
    public float thirstDamage = 0.1f;

    //mating
    public float hornyChancePerSecond;
    public bool horny;
    public float matingDistance;


    public float rotationSpeed;

    private Transform legFL;
    private Transform legFR;
    private Transform legBL;
    private Transform legBR;

    private Transform body;
    private Transform head;

    public float gallopingThreshold = 2f;




    // void (modifier) functions at the top #########################################################################
    //TODO: add spaces and headers to variables

    public void Awake() {
        if (transform.tag == "Pet") {
            enemyTag = "Hostile";
        } else if (transform.tag == "Hostile") {
            enemyTag = "Pet";
        } else {
            throw new System.Exception("Wrong tag for unit");
        }


        //set up transforms of bodyparts
        legFL = transform.GetChild(0).Find("LegFLPivot");
        legFR = transform.GetChild(0).Find("LegFRPivot");
        legBL = transform.GetChild(0).Find("LegBLPivot");
        legBR = transform.GetChild(0).Find("LegBRPivot");

        body = transform.GetChild(0).Find("BodyPivot");
        head = transform.GetChild(0).Find("HeadPivot");

        health = maxHealth;
        isBeingOverride = false;
        amountFed = maxFed;
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false; //deactivate built-in downwards gravity
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        unitState = UnitStateMachine.NextState(this);
        UpdateLegsLenghtModel();
        UpdateBodySizeModel();
        UpdateHeadSizeModel();
        UpdateMovingAnimation();
    }

    //Rotate, move unit towards destination, affect gravity and animate
    public void Move(Vector3 destination) {

        //only rotate normal to the planet
        Vector3 projectedDestination = Vector3.ProjectOnPlane(destination, transform.up);
        Quaternion targetRotation = Quaternion.LookRotation(projectedDestination, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

        //move forward in the local axis
        rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * speed);

        this.GravityEffect();

        //animate movement
        //AnimateWalk();
    }

    public void GravityEffect() {
        planet.Attract(transform);
    }

    //Find a random point in planet's surface 
    public void GetDestination() {
        //random position somewhere on the surface of the planet
        destination = UnityEngine.Random.onUnitSphere * 5f;
        destinationStampTime = Time.time;
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) { this.Die(); }
    }

    ////TODO: REPLACE BY BLENDER-MADE ANIMATION SET UP IN ANIMATION HANDLER
    //public void AnimateWalk() {
    //    Transform prefab = this.gameObject.transform.GetChild(0);

    //    //reset position from eat animation
    //    prefab.localPosition = Vector3.Lerp(prefab.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime);

    //    //the z rotation goes frrom -animationTilt to animationTilt according to a sine.
    //    float currentRotation = Mathf.Sin(Time.time * walkAnimationSpeed) * animationTilt;
    //    prefab.localRotation = Quaternion.Euler(new Vector3(0, 0, currentRotation));
    //    return;
    //}

    ////TODO: REPLACE BY BLENDER-MADE ANIMATION SET UP IN ANIMATION HANDLER
    //public void AnimateEat() {
    //    Transform prefab = this.gameObject.transform.GetChild(0);

    //    //reset rotation from walk animation
    //    prefab.localRotation = Quaternion.Lerp(prefab.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime);

    //    //jump animation
    //    float jumpHeight = Mathf.Abs(Mathf.Cos(Time.time * eatAnimationSpeed) * 2f) - .5f;
    //    prefab.localPosition = Vector3.Lerp(prefab.localPosition, new Vector3(0f, jumpHeight, 0f), Time.deltaTime * eatAnimationSpeed);
    //    return;
    //}

    public void OverrideDestination(Vector3 newDestination) {
        minDistance = 1f; //update minDistance as new destinations are more accurate
        destination = newDestination;
        isBeingOverride = true;
        unitState = UnitState.Wander;
        //todo: override to false;
    }

    public void HungerEffect() {
        this.amountFed -= this.hungerPerSecond * Time.deltaTime;
        if (this.amountFed <= 0) { this.TakeDamage(this.hungerDamage); }
    }

    public void ThirstEffect() {
        // suffer thirst
    }

    public void Die() {
        unitState = UnitState.Dead;
    }



    //does the unit require to be given a new destination
    public bool NeedsDestination() {
        //no destination TODO: check maybe null
        if (destination == Vector3.zero) { return true; }

        ////destination already reached
        if ((destination - transform.position).magnitude <= minDistance) { return true; }

        //if its wandering and couldn't reach the destination in 10 sec reset 
        if (Time.time - destinationStampTime > 10f && unitState == UnitState.Wander) { return true; }

        //otherwise
        return false;
    }

    //Check for enemy units in range
    public Unit CheckForEnemy() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        //find closest enemy and return it
        float closestDistance = Mathf.Infinity;
        Unit closestEnemy = null;
        foreach (GameObject enemy in enemies) {
            float distance = (transform.position - enemy.transform.position).magnitude;
            if (distance < enemyDetectionRange && distance < closestDistance) {
                closestDistance = distance;
                closestEnemy = enemy.GetComponent<Unit>();
            }
        }
        return closestEnemy;
    }

    //Check for enemy units in range
    public Food CheckForFood() {
        // if (amountFed > hungerThreshold) { return null; } //don't look for food if not hungry
        //TODO: check for efficiency. Is it iterating through all gameobjects in scene?
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        //find closest source of food and return it
        float closestDistance = Mathf.Infinity;
        Food closestFood = null;
        foreach (GameObject food in foods) {
            float distance = (transform.position - food.transform.position).magnitude;
            if (distance < viewDistance && distance < closestDistance) {
                closestDistance = distance;
                closestFood = food.GetComponent<Food>();
            }
        }

        return closestFood;
    }


    //Physically change legs length
    public void UpdateLegsLenghtModel()
    {
        float minLength = 0.6f;
        float maxLength = 5f;

        float minCollider = -.6f;
        float maxCollider = .6f;

        float minSpeed = 0.2f;
        float maxSpeed = 1f;

        //scale legs
        legFR.transform.localScale = Vector3.Lerp(legFR.transform.localScale,
            new Vector3(legFR.transform.localScale.x,
            legsLength * (maxLength - minLength) + minLength,
            legFR.transform.localScale.z), Time.deltaTime*2);
        legFL.transform.localScale = Vector3.Lerp(legFL.transform.localScale,
            new Vector3(legFL.transform.localScale.x,
            legsLength * (maxLength - minLength) + minLength,
            legFL.transform.localScale.z),Time.deltaTime*2);
        legBR.transform.localScale = Vector3.Lerp(legBR.transform.localScale,
            new Vector3(legBR.transform.localScale.x,
            legsLength * (maxLength - minLength) + minLength,
            legBR.transform.localScale.z), Time.deltaTime*2);
        legBL.transform.localScale = Vector3.Lerp(legBL.transform.localScale,
            new Vector3(legBL.transform.localScale.x,
            legsLength * (maxLength - minLength) + minLength,
            legBL.transform.localScale.z), Time.deltaTime*2);

        //change position of sphere collider
        GetComponent<SphereCollider>().center = Vector3.Lerp(GetComponent<SphereCollider>().center,
            new Vector3(0, maxCollider + legsLength * (minCollider - maxCollider), 0), Time.deltaTime);


        ////change animation speed. Remove as animation is fully changed to galloping
        if (transform.GetChild(0).GetComponent<Animator>().GetBool("isGalloping"))
        {
            transform.GetChild(0).GetComponent<Animator>().speed = Mathf.Lerp(transform.GetChild(0).GetComponent<Animator>().speed,
                1, Time.deltaTime);
        }
        else
        {
            transform.GetChild(0).GetComponent<Animator>().speed = Mathf.Lerp(transform.GetChild(0).GetComponent<Animator>().speed,
                maxSpeed - (maxSpeed - minSpeed) * legsLength, Time.deltaTime);

        }

        //toggle between walk and gallop when over threshold
    }

    //Physically change size of body
    public void UpdateBodySizeModel()
    {
        float minSize = 0.8f;
        float maxSize = 1.4f;

        body.transform.localScale = Vector3.Lerp(body.transform.localScale,
            new Vector3(bodySize * (maxSize - minSize) + minSize,
            bodySize * (maxSize - minSize) + minSize,
            bodySize * (maxSize - minSize) + minSize), Time.deltaTime*2);
    }

    //Physically change size of head
    public void UpdateHeadSizeModel()
    {
        float minSize = 1f;
        float maxSize = 3.0f;

        head.transform.localScale = Vector3.Lerp(head.transform.localScale,
            new Vector3(headSize * (maxSize - minSize) + minSize,
            headSize * (maxSize - minSize) + minSize,
            headSize * (maxSize - minSize) + minSize), Time.deltaTime * 2);
    }

    //toggles from walking to galloping based on the speed
    public void UpdateMovingAnimation()
    {
        if (speed >= gallopingThreshold)
        {
            //gallop
            transform.GetChild(0).GetComponent<Animator>().SetBool("isGalloping", true);
        }
        else
        {
            //walk
            transform.GetChild(0).GetComponent<Animator>().SetBool("isGalloping", false);

        }
        return;
       
    }
}