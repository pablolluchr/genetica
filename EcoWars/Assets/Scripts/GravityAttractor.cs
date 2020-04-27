using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//objects that create gravity should use this script (planets)
public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;
    
    //nake body stand up on planet (force + rotation)
    public void Attract(Transform body){

        Vector3 targetDir = (body.position-transform.position).normalized; //center of the planet
        Vector3 bodyUP = body.up;

        //transition to the target direction (pointing to the center of the planet)
        body.rotation = Quaternion.FromToRotation(bodyUP,targetDir) * body.rotation;

        //Apply force in the direction of the earth 
        body.GetComponent<Rigidbody>().AddForce(targetDir*gravity);
    }
}
