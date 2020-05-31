using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    private Animator anim;
    public Unit targetUnit;
    public GameObject previewUnit;
    public GameObject previewCamera;
    private Slider health;
    private Slider food;
    private Slider water;
    private TMPro.TextMeshProUGUI genetium;
    private TMPro.TextMeshProUGUI genetiumMax;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: do this myself in inspector
        health = transform.Find("Health").GetComponent<Slider>();
        food = transform.Find("Food").GetComponent<Slider>();
        water = transform.Find("Water").GetComponent<Slider>();
        genetium = transform.Find("Genetium").GetComponent<TMPro.TextMeshProUGUI>();
        genetiumMax = transform.Find("GenetiumMax").GetComponent<TMPro.TextMeshProUGUI>();
        anim = transform.parent.GetComponent<Animator>();
        transform.Find("Species").GetComponent<Button>().onClick.AddListener(OpenSpeciesPanel);
        previewUnit.gameObject.SetActive(false);
        targetUnit = null;
        
    }

    public void OpenSpeciesPanel()
    {
        Hide();
        GameManager.gameManager.attributePanel.GetComponent<AttributePanel>().OpenPanel(targetUnit.speciesName);
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

        anim.SetBool("shown", true);
    }

    public void Hide()
    {
        anim.SetBool("shown", false);
        previewUnit.gameObject.SetActive(false);

        StartCoroutine(DisablePanelDelayed());

    }

    IEnumerator DisablePanelDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        //gameObject.SetActive(false);

    }


}
