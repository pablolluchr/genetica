using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesSelectionPanel : MonoBehaviour
{
    //public string unitType;
    //public GameObject speciesButton;
    //public GameObject bottomControls;
    


    //public void ShowSpecies()
    //{

    //    //reset previous species
    //    foreach (Transform child in transform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    float offset = 80f;
    //    foreach (var species in GameManager.gameManager.speciesList)
    //    {
    //        if (species.tag == unitType)
    //        {

    //            GameObject button = Instantiate(speciesButton);
    //            button.transform.SetParent(this.gameObject.transform);
    //            button.GetComponent<SpeciesButton>().speciesName = species.speciesName;
    //            button.GetComponent<SpeciesButton>().unitType = unitType;
    //            button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = species.speciesName;
    //            RectTransform rt = button.GetComponent<RectTransform>();
    //            rt.anchoredPosition = new Vector2(offset, 0);
    //            offset += 125;
    //        }


    //    }
    //    gameObject.SetActive(false);
    //}

    //public void ClosePanel()
    //{
    //    bottomControls.SetActive(true);
    //    gameObject.SetActive(false);
    //}
}
