using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesSelectionPanel : MovingUIPanel {
    public GameObject speciesButton;
    //public GameObject bottomControls;
    //todo: use hide and show from MovingUIPanel
    private void Start() {
        Show();
    }

    new public void Show() {

        //reset previous species
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        //Add button for each species
        float offset = 80f;
        foreach (var species in GameManager.gameManager.speciesList) {
            if (species.tag == "Hostile") break;
            GameObject button = Instantiate(speciesButton);
            button.transform.SetParent(this.gameObject.transform);
            button.GetComponent<SpeciesButton>().species = species;
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(offset, 0);
            offset += 125;

        }
        base.Show();
    }

}
