using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaGraphic : MonoBehaviour
{
    // Start is called before the first frame update
    //private float spawnTime;
    private float scaleMultiplier;
    //private float alphaMultiplier;
    private Vector3 originalScale;
    public Species species;

    private void Awake()
    {
        transform.position = Vector3.zero;
        //spawnTime = Time.time;
        scaleMultiplier = 10;
        //alphaMultiplier = 1;
        originalScale = transform.localScale;
        transform.localScale = originalScale * scaleMultiplier;

        //TODO: the species icon should be the highlight color of the pet.
    }

    public void SetSpecies(Species species)
    {
        this.species = species;
        Vector3 planetPosition = GameManager.gameManager.planet.transform.position;

        Vector3 targetDir = (planetPosition - species.areaCenter).normalized; //center of the planet
        Vector3 bodyUP = transform.up;

        //transition to the target direction (pointing to the center of the planet)
        transform.rotation = Quaternion.FromToRotation(bodyUP, targetDir) * transform.rotation;
        transform.position = species.areaCenter - targetDir * .2f;
    }

    public void SelectArea()
    {
        //UnitActions.DisableAllSelectionGraphics();
        transform.Find("Image").GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
        transform.Find("Image").GetComponent<Image>().color = (new Vector4(1, 1, 1, 1));

    }

    public void DeselectArea()
    {
        //UnitActions.DisableAllSelectionGraphics();
        transform.Find("Image").GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);

        transform.Find("Image").GetComponent<Image>().color = (new Vector4(1, 1, 1, 0.5f)) ;


    }
}
