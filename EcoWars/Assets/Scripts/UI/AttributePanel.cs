using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MonoBehaviour
{
    public float speed;
    public float legsLength;
    public string speciesName;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: remove button and replace by slide down
        transform.Find("Close").GetComponent<Button>().onClick.AddListener(ClosePanel);
        transform.Find("Apply").GetComponent<Button>().onClick.AddListener(ApplyChanges);
    }

    void OpenPanel(string speciesName)
    {
        gameObject.SetActive(true); //replace by sliding up animation
    }

    void ClosePanel()
    {

        //TODO: swipe down (kinda like the notificaiton bar). X button in the meanwhile

        //open other menus and close current
        GameManager.gameManager.bottomControls.SetActive(true);
        GameManager.gameManager.speciesSelectionPanel.SetActive(true);
        gameObject.SetActive(false); //replace by sliding up animation
    }

    void ApplyChanges()
    {
        //update species with the new values
        Species species = GameManager.gameManager.GetSpecies(speciesName);
        species.speed = speed;
        species.legsLength = legsLength;

        //TODO: recall all units to base
        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        GameObject[] filteredSpecied = UnitHelperFunctions.FilterSpecies(pets, speciesName);
        foreach (GameObject pet in filteredSpecied) pet.GetComponent<Unit>().needsChange = true;

        ClosePanel();
    }
}

public enum Attribute
{
    Speed,
    LegsLength,
}
