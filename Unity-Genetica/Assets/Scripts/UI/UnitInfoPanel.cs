using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanel : MovingUIPanel
{
    public Unit targetUnit;
    public GameObject previewPet;
    public GameObject previewEnemy;
    public GameObject previewCamera;
    private Slider health;
    private Slider food;
    private Slider water;
    private TMPro.TextMeshProUGUI genetium;
    private TMPro.TextMeshProUGUI genetiumMax;
    //todo: create enemyInfoPanel for enemies

    void Start()
    {
        //TODO: do this myself in inspector
        health = transform.Find("Health").GetComponent<Slider>();
        food = transform.Find("Food").GetComponent<Slider>();
        water = transform.Find("Water").GetComponent<Slider>();
        genetium = transform.Find("Genetium").GetComponent<TMPro.TextMeshProUGUI>();
        genetiumMax = transform.Find("GenetiumMax").GetComponent<TMPro.TextMeshProUGUI>();
        transform.Find("Species").GetComponent<Button>().onClick.AddListener(OpenAttributePanel);
        previewPet.gameObject.SetActive(false);
        previewEnemy.gameObject.SetActive(false);
        targetUnit = null;
        Hide();
        
    }

    public void OpenAttributePanel()
    {
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
    public void Show(Unit unit) {
        targetUnit = unit;
        previewCamera.SetActive(true);

        if (unit.CompareTag("Pet")) {
            GameManager.gameManager.GetSpeciesFromName(unit.speciesName).UpdateUnit(previewPet.GetComponent<Unit>());
            previewPet.SetActive(true);
            previewEnemy.SetActive(false);
        } else {
            previewEnemy.SetActive(true);
            previewPet.SetActive(false);
        }
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
        previewPet.SetActive(false);
        previewEnemy.SetActive(false);

    }


}
