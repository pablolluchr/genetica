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

    [Header("BodyParts")]
    //[Range(0f, 1f)] public float headSize;
    //[Range(0f, 1f)] public float legsSize = .2f;
    //[Range(0f, 1f)] public float armsSize = .2f;
    //[Range(0f, 1f)] public float bellySize = .1f;
    //[Range(0f, 1f)] public float tailSize = .2f;
    //[Range(0f, 1f)] public float earSize = .2f;

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

    public Transform destinationGizmo;
    public GameObject selectionGraphic;
    public GameObject targetGraphic;
    public Vector4 selectionColor;
    private Animator animator;

    private int fixedUpdateCounter;

    [System.NonSerialized] public Vector3 areaCenter;

    //body parts
    public Transform legL;
    public Transform legR;
    public Transform head;
    public Transform belly;
    public Transform armL;
    public Transform armR;
    public Transform earL;
    public Transform earR;
    public Transform tail;
    public Transform UIPivot;

    //body parts for material
    public Renderer headRenderer;
    public Renderer earLRenderer;
    public Renderer earRRenderer;
    public Renderer bellyRenderer;
    public Renderer tailRenderer;


    public void Awake() {

        if (gameObject.CompareTag("Preview") ){ return; }
        if (gameObject.tag == "Pet") healthbar.color = healthbarPetColor;
        else healthbar.color = healthbarHostileColor;

        

        //InitBodyParts();


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
    ///

    //public void InitBodyParts()
    //{
    //    legL = transform.GetChild(0).Find("LegLPivot");
    //    legR = transform.GetChild(0).Find("LegRPivot");
    //    head = transform.GetChild(0).Find("HeadPivot");
    //    belly = transform.GetChild(0).Find("BellyPivot");
    //    armL = transform.GetChild(0).Find("ArmLPivot");
    //    armR = transform.GetChild(0).Find("ArmRPivot");
    //    earR = transform.GetChild(0).Find("EarRPivot");
    //    earL = transform.GetChild(0).Find("EarLPivot");
    //    tail = transform.GetChild(0).Find("TailPivot");
    //    UIPivot = transform.GetChild(0).Find("UIPivot");

    //}

    public void UpdateFurColor(Color color)
    {

        headRenderer.material.color = color;
        tailRenderer.material.color = color;
        earRRenderer.material.color = color;
        earLRenderer.material.color = color;
        bellyRenderer.material.color = color;

    }

    public void UpdateHeadSize(float size)
    {
        float newScale = GetInterpolated(size, 1, 2);
        head.localScale = new Vector3(newScale, newScale, newScale);

        float newEarPosition = GetInterpolated(size, 0.6f, 1.6f);
        earR.localPosition = new Vector3(newEarPosition,earR.localPosition.y, earR.localPosition.z);
        earL.localPosition = new Vector3(-newEarPosition,earL.localPosition.y, earL.localPosition.z);

        UpdateUIPivot();

    }

    public void UpdateBellySize(float size)
    {
        float newScale = GetInterpolated(size, 1, 2);
        belly.localScale = new Vector3(newScale, newScale, newScale);

        float newHeadPosition = GetInterpolated(size, -.25f, 1.25f);
        head.localPosition = new Vector3( head.localPosition.x, newHeadPosition, earR.localPosition.z);

        float newTailPosition = GetInterpolated(size, 0.9f, 1.85f);
        tail.localPosition = new Vector3(tail.localPosition.x, tail.localPosition.y,-newTailPosition);

        float newEarPosition = GetInterpolated(size, 0.32f, 1.73f);
        earR.localPosition = new Vector3(earR.localPosition.x, newEarPosition, earR.localPosition.z);
        earL.localPosition = new Vector3(earL.localPosition.x, newEarPosition, earL.localPosition.z);

        //update tail
        UpdateUIPivot();

    }

    public void UpdateLegSize(float size)
    {
        float newLengthScale = GetInterpolated(size, 1, 4.5f);
        float newWidthScale = GetInterpolated(size, 1, 1.5f);
        legL.localScale = new Vector3(newWidthScale, newLengthScale, newWidthScale);
        legR.localScale = new Vector3(newWidthScale, newLengthScale, newWidthScale);

        float newColliderPosition = GetInterpolated(size, -0.35f, 0.4f);
        GetComponent<CapsuleCollider>().center = new Vector3(0, -newColliderPosition, 0);

    }

    public void UpdateEarSize(float size)
    {
        float newScale = GetInterpolated(size, 1, 2.7f);
        earL.localScale = new Vector3(newScale, newScale, newScale);
        earR.localScale = new Vector3(newScale, newScale, newScale);

    }

    public void UpdateTailSize(float size)
    {
        float newScale = GetInterpolated(size, 1, 3.5f);
        tail.localScale = new Vector3(newScale, newScale, newScale);

    }

    public void UpdateArmSize(float size)
    {
        float newScale = GetInterpolated(size, 1, 2.3f);
        armL.localScale = new Vector3(newScale, newScale, newScale);
        armR.localScale = new Vector3(newScale, newScale, newScale);

    }

    public void UpdateUIPivot()
    {
        float newY = head.localPosition.y+ (head.localScale.y - 1) * 2f;
        UIPivot.localPosition = new Vector3(UIPivot.localPosition.x, newY, UIPivot.localPosition.z);


    }

    //return model local scale given a model min and max scale and a size from 0 to 1
    public float GetInterpolated(float size, float minScale, float maxScale)
    {
        if (size < 0 || size > 1) { throw new System.Exception("Head size has to be between 0 and 1"); }
        return minScale + (maxScale - minScale) * size;
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