using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Object
{
    
    public float stomachFillPerSecond = 1f;
    public float regeneratePerSecond = .1f;
    public float maxFood = 100f;
    public float availableFood;
    public float radius = 2f;
    public float consideredEmpty = 1f;

    new void Start()
    {
        availableFood = maxFood;
        base.Start();
    }

    void Update()
    {
        availableFood = Mathf.Min(maxFood, availableFood + regeneratePerSecond*
            Time.deltaTime * GameManager.gameManager.countsBetweenUpdates);
    }

    public float Eat(float stomachLeft)
    {
        float foodEaten = Mathf.Min(stomachLeft, stomachFillPerSecond *
            Time.deltaTime * GameManager.gameManager.countsBetweenUpdates, availableFood);
        availableFood -= foodEaten;
        return foodEaten;
    }

    
}
