using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float stomachFillPerSecond = 1f;
    public float regeneratePerSecond = .1f;
    public float maxFood = 100f;
    public float availableFood;
    public float radius = 2f;
    public float consideredEmpty = 1f;

    void Start()
    {
        availableFood = maxFood;
    }

    void FixedUpdate()
    {
        availableFood = Mathf.Min(maxFood, availableFood + regeneratePerSecond*Time.fixedDeltaTime);
    }

    public float Eat(float stomachLeft)
    {
      
        float foodEaten = Mathf.Min(stomachLeft, stomachFillPerSecond * Time.fixedDeltaTime,availableFood);
        availableFood -= foodEaten;
        return foodEaten;
    }
}
