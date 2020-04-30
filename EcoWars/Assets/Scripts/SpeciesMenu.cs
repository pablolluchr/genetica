using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesMenu : MonoBehaviour
{
    public GameObject speciesToggle;

    void Start()
    {
        float offset = 0;
        foreach (var species in GameManager.gameManager.speciesList)
        {

            GameObject button = Instantiate(speciesToggle);
            button.transform.SetParent(this.gameObject.transform);
            button.GetComponent<SpeciesButton>().speciesName = species.speciesName;
            button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text=species.speciesName;
            button.transform.position += new Vector3(offset, 0, 0);
            offset += 250f;
        }
    }
}
