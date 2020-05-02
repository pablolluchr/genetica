using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehaviour : MonoBehaviour
{
    public float lightIntensity;
    public bool isMaterialEmittor;
    // Start is called before the first frame update
    void Start()
    {
        lightIntensity = GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lightHorizontalPosition = new Vector2(transform.position.x, transform.position.z).normalized;

        Vector2 sunHorizontalPosition = new Vector2(GameManager.gameManager.sun.transform.position.x,
            GameManager.gameManager.sun.transform.transform.position.z).normalized;

        if ((lightHorizontalPosition+ sunHorizontalPosition).magnitude <1.5) //area is dark
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }


    }
    public Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    public void TurnOn()
    {
        //turn on animation
        GetComponent<Light>().intensity = lightIntensity;
        if (isMaterialEmittor)
            transform.parent.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);

    }

    public void TurnOff()
    {
        //turn off animation
        GetComponent<Light>().intensity = 0;
        if (isMaterialEmittor)
            transform.parent.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);


    }

}
