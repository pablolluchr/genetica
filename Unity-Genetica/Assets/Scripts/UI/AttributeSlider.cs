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
        //todo: get values from species and available ranges and display them accordingly.
        slider = transform.Find("Slider").GetComponent<Slider>();
        attributePanel = transform.parent.GetComponentInParent<AttributePanel>();
        slider.onValueChanged.AddListener(ThresholdValue);
    }
    public void ThresholdValue(float value)
    {
        //clamp value
        int clampedValue = (int) Mathf.Clamp(value, lowerThreshold, upperThreshold);
        slider.value = clampedValue;

        float paramValue;

        switch (attribute)
        {
            case Attribute.Speed:
                //map that int value to the actual parameter value
                paramValue = (float)clampedValue;
                attributePanel.speed = paramValue;
                break;
            case Attribute.LegsLength:
                //map that int value to the actual parameter value
                paramValue = (float)clampedValue / 4f ;
                attributePanel.legsLength = paramValue;
                break;
        }



    }

}