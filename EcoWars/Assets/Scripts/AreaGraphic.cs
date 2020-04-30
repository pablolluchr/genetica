using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaGraphic : MonoBehaviour
{
    // Start is called before the first frame update
    private float spawnTime;
    private float scaleMultiplier;
    private float alphaMultiplier;
    private Vector3 originalScale;

    private void Awake()
    {
        transform.position = Vector3.zero;
        spawnTime = Time.time;
        scaleMultiplier = 10;
        alphaMultiplier = 1;
        originalScale = transform.localScale;
        transform.localScale = originalScale * scaleMultiplier;


    }

    private void Update()
    {

        //scaleMultiplier = Mathf.Lerp(scaleMultiplier, 5, Time.deltaTime * 2);
        //alphaMultiplier = Mathf.Lerp(alphaMultiplier, 0, Time.deltaTime * 4);

        //transform.localScale = originalScale * scaleMultiplier;
        //UnityEngine.UI.Image image = transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        //image.color = new Color(image.color.r, image.color.g, image.color.b, alphaMultiplier);

    }


    public void SetPosition(Vector3 position, Vector3 planetPosition)
    {
        Vector3 targetDir = (planetPosition - position).normalized; //center of the planet
        Vector3 bodyUP = transform.up;

        //transition to the target direction (pointing to the center of the planet)
        transform.rotation = Quaternion.FromToRotation(bodyUP, targetDir) * transform.rotation;
        transform.position = position - targetDir * .2f;

    }
}
