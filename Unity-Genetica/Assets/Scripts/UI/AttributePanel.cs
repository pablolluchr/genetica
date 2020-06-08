using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MovingUIPanel
{
    public float headSize;
    public float legSize;
    public float bellySize;
    public float tailSize;
    public float earSize;
    public float armSize;
    public Species species;
    public Transform headPanel;
    public Transform legPanel;
    public Transform bellyPanel;
    public Transform tailPanel;
    public Transform earPanel;
    public Transform armPanel;

    public Button close;
    public Button apply;
    //todo: extend this as movingUIpanel
    //todo: create attributeSelection gamestate and handle the closing and opening of the other panels from there
    // Start is called before the first frame update
    void Start()
    {
        apply.onClick.AddListener(ApplyChanges);
        Hide();
    }

    public void Show(string speciesName)
    {
        this.species = GameManager.gameManager.GetSpeciesFromName(speciesName);
        SetupAttributeBars();
        base.Show();

    }

    void SetupAttributeBars()
    {
        headSize = species.headSize;
        legSize = species.legSize;
        bellySize = species.bellySize;
        tailSize = species.tailSize;
        earSize = species.earSize;
        armSize = species.armSize;
        headPanel.Find("Bar").GetComponent<Slider>().value = (int) (headSize * 5);
        legPanel.Find("Bar").GetComponent<Slider>().value = (int) (legSize * 5);
        bellyPanel.Find("Bar").GetComponent<Slider>().value = (int) (bellySize * 5);
        tailPanel.Find("Bar").GetComponent<Slider>().value = (int) (tailSize * 5);
        earPanel.Find("Bar").GetComponent<Slider>().value = (int) (earSize * 5);
        armPanel.Find("Bar").GetComponent<Slider>().value = (int) (armSize * 5);
    }

    void ApplyChanges()
    {
        //update species with the new values
        species.headSize = headSize;
        species.legSize = legSize;
        species.bellySize = bellySize;
        species.tailSize = tailSize;
        species.earSize = earSize;
        species.armSize = armSize;


        //call species home for update
        List<Unit> pets = species.GetAllUnitsOfSpecies();
        foreach (Unit pet in pets) pet.needsChange = true;

        Hide();
    }
}