using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseAttributeButton : MonoBehaviour
{
    public string attributeType;
    public AttributePanel attributePanel;
    public Slider bar;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(UpdateAttribute);
    }

    public void UpdateAttribute()
    {
        //update graphic
        bar.value = Mathf.Min(5, bar.value + 1);

        if (attributeType == "head")
            attributePanel.headSize = bar.value/5f;
        if (attributeType == "legs")
            attributePanel.legSize = bar.value / 5f;
        if (attributeType == "belly")
            attributePanel.bellySize = bar.value / 5f;
        if (attributeType == "tail")
            attributePanel.tailSize = bar.value / 5f;
        if (attributeType == "ears")
            attributePanel.earSize = bar.value / 5f;
        if (attributeType == "arms")
            attributePanel.armSize = bar.value / 5f;

    }

}
