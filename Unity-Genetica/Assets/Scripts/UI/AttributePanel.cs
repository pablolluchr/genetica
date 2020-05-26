using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MonoBehaviour
{
    public float headSize;
    public Species species;
    public Transform headPanel;
    //public float legsLength;
    // Start is called before the first frame update
    void Awake()
    {
        //headPanel = transform.Find("HeadPanel");
        transform.Find("Close").GetComponent<Button>().onClick.AddListener(ClosePanel);
        transform.Find("Apply").GetComponent<Button>().onClick.AddListener(ApplyChanges);
        gameObject.SetActive(false);
    }

    public void OpenPanel(string speciesName)
    {
        this.species = GameManager.gameManager.GetSpeciesFromName(speciesName);
        gameObject.SetActive(true); //replace by sliding up animation
        SetupAttributeBars();

    }

    void SetupAttributeBars()
    {
        headSize = species.headSize;
        headPanel.Find("Bar").GetComponent<Slider>().value = (int) (headSize * 5);
    }

    void ClosePanel()
    {
        //TODO: swipe down animation
        gameObject.SetActive(false); //replace by sliding up animation
    }

    void ApplyChanges()
    {
        //update species with the new values
        species.headSize = headSize;


        //call species home for update
        List<Unit> pets = species.GetAllUnitsOfSpecies();
        foreach (Unit pet in pets) pet.needsChange = true;

        ClosePanel();
    }
}

public enum Attribute
{
    Speed,
    LegsLength,
}
