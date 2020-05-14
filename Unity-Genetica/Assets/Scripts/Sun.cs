using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private float sunSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate sun around planet lol
        transform.rotation = Quaternion.Euler(0, sunSpeed * Time.deltaTime, 0) * transform.rotation;
    }
}
