using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("This is a test message");
        int test = 1; //Debugger should stop here
        Debug.Log("This shouldn't be printed until debugger resumes");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
