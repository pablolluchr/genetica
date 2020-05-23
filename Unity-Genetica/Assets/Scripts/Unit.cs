using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Handles the target-based movement of units
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour {

    public UnitState unitState;

    [Header("General Attributes")]
    public string species;
    public float viewDistance = 5f;
    public float interactionRadius;
    public float areaRadius = 3f;
    public bool swimming;
    public bool needsChange;
    public float swimspeed;
    public float walkspeed;
    public int updateCycleCounter;
  
    [Range(.01f, 3.0f)] public float speed = 1f;
    [Range(.0f, 1.0f)] public float legsLength = .2f;
    [Range(.0f, 1.0f)] public float bodySize = .1f;
    [Range(.0f, 1.0f)] public float headSize = .2f;
    //public float maxDistance = 20f; //only follow targets maxDistance appart

    [Header("Health")]
    public float maxHealth;
    public float health;
    public float healthRegen;
    public float criticalHealth;
    public float deathPeriod;
    public bool dead = false;
    public float deathTimeStamp;


    [Header("Eating Attributes")]
    public float amountFed; //how much of the stomach is filled
    public float maxFed;
    public float feedingPerSecond;
    public float hungerPerSecond;
    public float hungerChanceExponent;
    public float hungerDamage;
    public bool hungry;
    public float criticalHunger;

    [Header("Drinking Attributes")]
    public float amountQuenched; //how much of the stomach is filled
    public float maxQuenched;
    public float thirstPerSecond;
    public float thirstChanceExponent;
    public float thirstDamage;
    public bool thirsty;
    public float criticalThirst;
    public float quenchRate;


    [Header("Mating Attributes")]
    public float hornyChancePerSecond;
    public bool horny;
    public float matingDistance;
    public float hornyCurveExponent;

    [Header("Attacking Attributes")]
    public float attackDamage = 1f;
    public float attackRange;
    public float enemyDetectionRange;
    public float attackRate;
    public float lastAttacked = 0;
    public float aggression;
    public int attackForce;
    
    [Header("Harvesting Attributes")]
    public float carryingCapacity;
    public float currentGenetiumAmount;

    [Header("Animation")]
    public float walkAnimationSpeed = 10f;
    public float eatAnimationSpeed = 1f;
    public float animationTilt = 10f;
    public float gallopingThreshold = 2f;
    public float rotationSpeed;

    [Header("UI")]
    public Transform thoughtPivot;
    public Sprite thirstSprite;
    public Sprite hornySprite;
    public Sprite hungrySprite;
    public Sprite genetiumSprite;
    public Sprite baseSprite;
    public Sprite attackSprite;
    public Canvas healthbarPivot;
    public SpriteRenderer healthbar;
    public Color healthbarPetColor;
    public Color healthbarHostileColor;


    //not shown

    public Rigidbody rb;
    public bool isBeingOverride;
    public GravityAttractor planet;
    public float wanderTimeStamp;
    public float eatRange = 1f;
    public string enemyTag;
    public int maxUnits = 50;
    private Transform legFL;
    private Transform legFR;
    private Transform legBL;
    private Transform legBR;

    private Transform body;
    private Transform head;
    public Transform destinationGizmo;
    public GameObject selectionGraphic;
    public GameObject targetGraphic;
    public Vector4 selectionColor;
    private Animator animator;

    private int fixedUpdateCounter;

    [System.NonSerialized] public Vector3 areaCenter;

    public void Awake() {
        if (gameObject.tag == "Pet") healthbar.color = healthbarPetColor;
        else healthbar.color = healthbarHostileColor;



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
        destinationGizmo = transform.Find("DestinationGizmo");

        GetComponent<Rigidbody>().useGravity = false; //deactivate built-in downwards gravity
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb = GetComponent<Rigidbody>();
        animator = transform.GetChild(0).GetComponent<Animator>();

        fixedUpdateCounter = Random.Range(0, GameManager.gameManager.countsBetweenFixedUpdates);
    }



    private void FixedUpdate()
    {
        //if (transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().isVisible)
        //    transform.GetChild(0).GetComponent<Animator>().enabled = true;
        //else transform.GetChild(0).GetComponent<Animator>().enabled = false;
        
        if (!dead)
        {
            UnitActions.Move(this);
            UnitActions.GravityEffect(this);
            UnitActions.SetThought(this);
            UnitActions.SetHealthBar(this);
        }

        fixedUpdateCounter = (fixedUpdateCounter + 1) % GameManager.gameManager.countsBetweenFixedUpdates;
        if (fixedUpdateCounter == 0) {

            UpdateMovingAnimation();
            UnitActions.WanderIfDeadTarget(this);
            UnitActions.HungerEffect(this);
            UnitActions.ThirstEffect(this);
            UnitActions.TurnHungryChance(this);
            UnitActions.TurnThirstyChance(this);
            UnitActions.TurnHornyChance(this);
            UnitActions.HealthRegenEffect(this);
            unitState = UnitStateMachine.NextState(this);
        }
    }


    /// <summary>
    /// Physical changes
    /// </summary>


    //Physically change legs length
    public void UpdateLegsLenghtModel()
    {
        return;
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
        return;
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
        return;
        float minSize = 1f;
        float maxSize = 3.0f;

        head.transform.localScale = Vector3.Lerp(head.transform.localScale,
            new Vector3(headSize * (maxSize - minSize) + minSize,
            headSize * (maxSize - minSize) + minSize,
            headSize * (maxSize - minSize) + minSize), Time.deltaTime * 2);
    }
    //check water to start swimming
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water") swimming = true;
    }

    //check for water to stop swimming
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water") swimming = false;
    }


    public void UpdateMovingAnimation()
    {
        if (unitState==UnitState.Harvest) animator.SetBool("isCollectingGenetium", true);
        else animator.SetBool("isCollectingGenetium", false);

        //start eating
        if (unitState == UnitState.Drink || unitState == UnitState.Eat) animator.SetBool("isEating-Drinking", true);
        else animator.SetBool("isEating-Drinking", false);

        //start swimming
        if (swimming){
            if (!animator.GetBool("isSwimming"))
            {
                animator.SetBool("isSwimming", true);
                animator.SetBool("isWalking", false);
            }
        }
        //start walkiing
        else if (!animator.GetBool("isWalking")){
                transform.GetChild(0).GetComponent<Animator>().SetBool("isWalking", true);
                transform.GetChild(0).GetComponent<Animator>().SetBool("isSwimming", false);

        }
       
    }
}