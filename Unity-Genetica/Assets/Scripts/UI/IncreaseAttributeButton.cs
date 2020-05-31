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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
