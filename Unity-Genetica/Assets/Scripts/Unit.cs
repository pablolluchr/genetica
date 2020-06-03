using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Handles the target-based movement of units

public class Unit : MonoBehaviour {

    public UnitState unitState;

    [Header("General Attributes")]
    public string speciesName;
    public float viewDistance = 5f;
    public float interactionRadius;
    public float areaRadius;
    public bool swimming;
    public bool needsChange;
    public float swimspeed;
    public float speed;
    public int updateCycleCounter;
  
    //[Range(.01f, 3.0f)] public float speed = 1f;
    
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

    [Header("References")]
    public Transform unitModel;
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
    public Transform destinationGizmo;
    public GameObject selectionGraphic;
    public GameObject targetGraphic;
    public Animator animator;
    public Transform waterDetector;


    [Header("Renderers")]
    public Renderer headRenderer;
    public Renderer earLRenderer;
    public Renderer earRRenderer;
    public Renderer bellyRenderer;
    public Renderer tailRenderer;
    public SpriteRenderer thoughtRenderer;

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


    //TODO: make private
    [Header("State info")]
    public bool isBeingOverride;
    public float wanderTimeStamp;
    public float eatRange = 1f;
    public string enemyTag;
    public int updateCounter;
    public Vector3 areaCenter;

    


    public void Start() {

        if (gameObject.CompareTag("Preview") ){ return; }
        if (gameObject.CompareTag("Pet")) healthbar.color = healthbarPetColor;
        else healthbar.color = healthbarHostileColor;


        health = maxHealth;
        isBeingOverride = false;
        amountFed = maxFed;
        wanderTimeStamp = -Mathf.Infinity;
        destinationGizmo = transform.Find("DestinationGizmo");

        updateCounter = Random.Range(0, GameManager.gameManager.countsBetweenUpdates);

    }

    private void Update()
    {

        if (dead) return;
        if (gameObject.CompareTag("Preview")) { return; }

        UnitActions.Move(this);
        UpdateMovingAnimation();
        UnitActions.UpdateIsSwimming(this);

        updateCounter = (updateCounter + 1) % GameManager.gameManager.countsBetweenUpdates;
        if (updateCounter == 0)
        {

            
            UnitActions.SetThought(this);

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


    // visual update functions

    public void UpdateFurColor(string color)
    {
        Material material = GameManager.gameManager.GetFurMaterial(color);

        headRenderer.material = material;
        tailRenderer.material = material;
        earRRenderer.material = material;
        earLRenderer.material = material;
        bellyRenderer.material = material;

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

        //TODO: update tail
        UpdateUIPivot();

    }

    public void UpdateLegSize(float size)
    {
        float newLengthScale = GetInterpolated(size, 1, 4.5f);
        float newWidthScale = GetInterpolated(size, 1, 1.5f);
        legL.localScale = new Vector3(newWidthScale, newLengthScale, newWidthScale);
        legR.localScale = new Vector3(newWidthScale, newLengthScale, newWidthScale);

        float newModelPosition = GetInterpolated(size, 0.43f, 1.25f);
        unitModel.localPosition = new Vector3(unitModel.localPosition.x, newModelPosition, unitModel.localPosition.z);


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
        //TODO
        if (UIPivot == null) return;
        //float newY = head.localPosition.y+ (head.localScale.y - 1) * 4f;
        //UIPivot.localPosition = new Vector3(UIPivot.localPosition.x, newY, UIPivot.localPosition.z);


    }

    //return model local scale given a model min and max scale and a size from 0 to 1
    public float GetInterpolated(float size, float minScale, float maxScale)
    {
        if (size < 0 || size > 1) { throw new System.Exception("Head size has to be between 0 and 1"); }
        return minScale + (maxScale - minScale) * size;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water")) { }
        else if (other.gameObject.CompareTag("Food")) { }
        else if (other.gameObject.CompareTag("Genetium")) { }
        else GetComponent<Target>().SetObstacle(other.gameObject.transform.position);

    }



    //check for water to stop swimming
    private void OnTriggerExit(Collider other)
    {
        //TODO: Do this using distance instead
        if (other.gameObject.CompareTag("Water")) { }
        else if (other.gameObject.CompareTag("Food")) { }
        else if (other.gameObject.CompareTag("Genetium")) { }
        else GetComponent<Target>().ResetObstacle();


    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hut");
        //foreach (ContactPoint contact in collision.contacts)
        //{
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}
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
                animator.SetBool("isWalking", true);
                animator.SetBool("isSwimming", false);

        }
       
    }
}