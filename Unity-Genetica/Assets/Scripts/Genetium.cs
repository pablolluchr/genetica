using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genetium : Object
{

    public float transferRate;
    public float regenRate;
    public float capacity;
    public float currentAmount;
    public float radius;
    public float consideredEmpty;

    // Start is called before the first frame update
    new void Start()
    {
        currentAmount = capacity;
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {
        currentAmount = Mathf.Min(capacity, currentAmount + regenRate * Time.deltaTime);
    }

    //todo harvest should be done here in the same fashion as in food eaten.

}
