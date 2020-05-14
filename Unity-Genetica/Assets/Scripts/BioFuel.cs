using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioFuel : MonoBehaviour
{
    public float maxCapacity = 100f;
    private float availableAmount;
    public float regenerationPerSecond = 1f;
    // Start is called before the first frame update
    void Start()
    {
        availableAmount = maxCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        availableAmount = Mathf.Max(maxCapacity, availableAmount + regenerationPerSecond);
    }



}
