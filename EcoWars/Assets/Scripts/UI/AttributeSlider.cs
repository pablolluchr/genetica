using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeSlider : MonoBehaviour
{
    public int lowerThreshold;
    public int upperThreshold;
    public Attribute attribute;

    private Slider slider;
    private AttributePanel attributePanel;

    private void Start()
    {

        slider = transform.Find("Slider").GetComponent<Slider>();
        attributePanel = transform.parent.GetComponentInParent<AttributePanel>();
        slider.onValueChanged.AddListener(ThresholdValue);
    }
    public void ThresholdValue(float value)
    {
        //clamp value
        int clampedValue = (int) Mathf.Clamp(value, lowerThreshold, upperThreshold);
        slider.value = clampedValue;

        //map that int value to the actual parameter value
        float paramValue = (float)clampedValue;
        
        switch (attribute)
        {
            case Attribute.Speed:
                attributePanel.speed = paramValue;
                break;
            case Attribute.LegsLength:
                attributePanel.legsLength = paramValue;
                break;
        }



    }

    //todo: for performance add and remove listeners on enable and disable.
}