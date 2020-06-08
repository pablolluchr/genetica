using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanel : MovingUIPanel
{
    public Unit targetUnit;
    public GameObject previewUnit;
    public GameObject previewCamera;
    private Slider health;
    private Slider food;
    private Slider water;
    private TMPro.TextMeshProUGUI genetium;
    private TMPro.TextMeshProUGUI genetiumMax;

    void Start()
    {
        //TODO: do this myself in inspector
        health = transform.Find("Health").GetComponent<Slider>();
        food = transform.Find("Food").GetComponent<Slider>();
        water = transform.Find("Water").GetComponent<Slider>();
        genetium = transform.Find("Genetium").GetComponent<TMPro.TextMeshProUGUI>();
        genetiumMax = transform.Find("GenetiumMax").GetComponent<TMPro.TextMeshProUGUI>();
        transform.Find("Species").GetComponent<Button>().onClick.AddListener(OpenAttributePanel);
        previewUnit.gameObject.SetActive(false);
        targetUnit = null;
        Hide();
        
    }

    public void OpenAttributePanel()
    {
        //todo: do this through a game manager state.
        //Hide();
        //GameManager.gameManager.attributePanel.OpenPanel(targetUnit.speciesName);
        GameManager.gameManager.SetSpeciesAttributes(true);
    }

    private void Update()
    {
        if (targetUnit && targetUnit.updateCounter == 0)
        {
                health.value = targetUnit.health / targetUnit.maxHealth;
                food.value = targetUnit.amountFed / targetUnit.maxFed;
                water.value = targetUnit.amountQuenched / targetUnit.maxQuenched;
                genetium.text = Mathf.Round(targetUnit.currentGenetiumAmount).ToString();
                genetiumMax.text= Mathf.Round(targetUnit.carryingCapacity).ToString();
        }
    }
    public void Show(Unit unit)
    {

        targetUnit = unit;
        GameManager.gameManager.GetSpeciesFromName(unit.speciesName).UpdateUnit(previewUnit.GetComponent<Unit>());
        previewUnit.SetActive(true);
        previewCamera.SetActive(true);

        Show();

    }

    new public void Hide()
    {
        base.Hide();
        StartCoroutine(DisablePreviewUnitDelayed());
    }

    IEnumerator DisablePreviewUnitDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        previewUnit.gameObject.SetActive(false);

    }


}
