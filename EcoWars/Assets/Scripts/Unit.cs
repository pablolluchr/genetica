using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles the target-based movement of units
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour {

    public UnitState unitState;

    [Header("General Attributes")]
    public float maxHealth = 10f;

    public float viewDistance = 5f;
    [Range(.5f, 3.0f)] public float speed = 1f;
    [Range(.0f, 1.0f)] public float legsLength = .2f;
    [Range(.0f, 1.0f)] public float bodySize = .1f;
    [Range(.0f, 1.0f)] public float headSize = .2f;
    //public float maxDistance = 20f; //only follow targets maxDistance appart


    [Header("Eating Attributes")]
    public float amountFed; //how much of the stomach is filled
    public float maxFed;
    public float feedingPerSecond;
    public float hungerPerSecond;
    public float hungerChanceExponent;
    public float hungerDamage;

    [Header("Drinking Attributes")]
    public float amountQuenched = 10f; //how much of the stomach is filled
    public float maxQuenched = 10f;
    public float thirstPerSecond = 0.5f;
    public float thirstThreshold;
    public float thirstDamage = 0.1f;

    [Header("Mating Attributes")]
    public float hornyChancePerSecond;
    public bool horny;
    public float matingDistance;

    [Header("Attacking Attributes")]
    public float attackDamagePerSecond = 1f;
    public float attackRange = 2f;
    public float enemyDetectionRange = 10f;

    [Header("Animation")]
    public float walkAnimationSpeed = 10f;
    public float eatAnimationSpeed = 1f;
    public float animationTilt = 10f;
    public float gallopingThreshold = 2f;
    public float rotationSpeed;

    //not shown

    [System.NonSerialized] public Rigidbody rb;
    [System.NonSerialized] public float health;
    [System.NonSerialized] public bool isBeingOverride;
    [System.NonSerialized] public GravityAttractor planet;
    [System.NonSerialized] public float wanderTimeStamp;
    [System.NonSerialized] public float eatRange = 1f;
    [System.NonSerialized] public string enemyTag;
    [System.NonSerialized] public int maxUnits = 100;
    private Transform legFL;
    private Transform legFR;
    private Transform legBL;
    private Transform legBR;

    private Transform body;
    private Transform head;
    [System.NonSerialized] public Transform destinationGizmo;
    [System.NonSerialized] public Transform selectionGraphic;
    public GameObject targetGraphic;









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
        wanderTimeStamp = -Mathf.Infinity;
        destinationGizmo = transform.Find("Destination");
        selectionGraphic = transform.Find("SelectionGraphic");
        selectionGraphic.GetComponent<Canvas>().enabled = false;

        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false; //deactivate built-in downwards gravity
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {

        UnitActions.HungerEffect(this);
        UnitActions.ThirstEffect(this);

        unitState = UnitStateMachine.NextState(this);
        UpdateLegsLenghtModel();
        UpdateBodySizeModel();
        UpdateHeadSizeModel();
        UpdateMovingAnimation();



        UnitActions.Move(this);
        UnitActions.GravityEffect(this);


    }
    

   /// <summary>
   /// Physical changes
   /// </summary>


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