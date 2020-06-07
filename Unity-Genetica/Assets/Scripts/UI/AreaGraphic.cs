using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaGraphic : MonoBehaviour
{
    //public Image icon;
    //public Renderer[] areaShaders;

    //public Texture red;
    //public Texture blue;
    //// Start is called before the first frame update
    ////private float spawnTime;
    //private float scaleMultiplier;
    ////private float alphaMultiplier;
    //private Vector3 originalScale;
    //public Species species;
    //public Color speciesColor;

    //private void Awake()
    //{
    //    transform.position = Vector3.zero;
    //    //spawnTime = Time.time;
    //    scaleMultiplier = 10;
    //    //alphaMultiplier = 1;
    //    originalScale = transform.localScale;
    //    transform.localScale = originalScale * scaleMultiplier;
    //    SetAreaShader(0);


    //    //TODO: the species icon should be the highlight color of the pet.
    //}

    //public void SetSpecies(Species species)
    //{
    //    this.species = species;
    //    speciesColor = GetColorFromString();

    //    Vector3 targetDir = (- species.areaCenter).normalized; //center of the planet

    //    //transition to the target direction (pointing to the center of the planet)
    //    transform.rotation = Quaternion.FromToRotation(transform.up, targetDir) * transform.rotation;
    //    transform.position = species.areaCenter - targetDir * .2f;

    //    DeselectArea();
    //}

    //public void SelectArea()
    //{
    //    //UnitActions.DisableAllSelectionGraphics();
    //    //icon.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
    //    icon.color = speciesColor;
    //    SetAreaShader(species.areaSize);

    //}

    //public void SetAreaShader(int level)
    //{
    //    for (int i = 0; i < areaShaders.Length; i++)
    //    {
    //        if (i < level)
    //        {
    //            areaShaders[i].enabled = true;
    //            areaShaders[i].material.mainTexture = TextureFromString(species.color);

    //        }
    //        else
    //            areaShaders[i].enabled = false;
    //    }
    //}

    //public Texture TextureFromString(string color)
    //{
    //    switch (color)
    //    {
    //        case "red":
    //            return red;
    //        case "blue":
    //            return blue;
    //        default:
    //            return null;
    //    }
    //}

    //public void DeselectArea()
    //{
    //    //UnitActions.DisableAllSelectionGraphics();
    //    //icon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
    //    icon.color = new Color(speciesColor.r, speciesColor.g, speciesColor.b, 0.5f);
    //    SetAreaShader(0);




    //}
    //void OnDrawGizmos()
    //{
    //    if (species == null) return;
    //    Gizmos.color = new Color(0, 1, 1, 0.4f);
    //    Gizmos.DrawSphere(transform.position, species.AreaRadiusFromSize());
    //}

    //public Color GetColorFromString()
    //{
    //    Color imageColor;
    //    switch (species.color)
    //    {

    //        case "red":
    //            imageColor = new Color(1, 0, 0); break;
    //        case "blue":
    //            imageColor = new Color(0, 0, 1); break;
    //        default:
    //            imageColor = new Color(0, 0, 0); break;
    //    }
    //    return imageColor;
    //}
}
