using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genetium : MonoBehaviour
{

    public float transferRate;
    public float regenRate;
    public float capacity;
    public float currentAmount;
    public float radius;
    public float consideredEmpty;

    // Start is called before the first frame update
    void Start()
    {
        currentAmount = capacity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentAmount = Mathf.Min(capacity, currentAmount + regenRate * Time.fixedDeltaTime);
    }

}
