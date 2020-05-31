using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaGraphic : MonoBehaviour
{
    public Image icon;
    // Start is called before the first frame update
    //private float spawnTime;
    private float scaleMultiplier;
    //private float alphaMultiplier;
    private Vector3 originalScale;
    public Species species;
    public Color speciesColor;

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
        speciesColor = GetColorFromString();
        Vector3 planetPosition = GameManager.gameManager.planet.transform.position;

        Vector3 targetDir = (planetPosition - species.areaCenter).normalized; //center of the planet
        Vector3 bodyUP = transform.up;

        //transition to the target direction (pointing to the center of the planet)
        transform.rotation = Quaternion.FromToRotation(bodyUP, targetDir) * transform.rotation;
        transform.position = species.areaCenter - targetDir * .2f;

        DeselectArea();
    }

    public void SelectArea()
    {
        //UnitActions.DisableAllSelectionGraphics();
        icon.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);

        icon.color = speciesColor;

    }

    public void DeselectArea()
    {
        //UnitActions.DisableAllSelectionGraphics();
        icon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
        icon.color = (new Color(speciesColor.r, speciesColor.g, speciesColor.b, 0.5f));



    }

    public Color GetColorFromString()
    {
        Color imageColor;
        switch (species.color)
        {

            case "red":
                imageColor = new Color(1, 0, 0); break;
            case "blue":
                imageColor = new Color(0, 0, 1); break;
            default:
                imageColor = new Color(0, 0, 0); break;
        }
        return imageColor;
    }
}
