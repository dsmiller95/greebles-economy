using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Slider slider;
    public Image iconImage;
    public Image fillImage;

    public ResourceType type = ResourceType.Gold;

    public void setResourceType(ResourceType type, Sprite iconImage = null, Color fillColor = default)
    {
        this.type = type;
        if (iconImage)
        {
            this.iconImage.sprite = iconImage;
        }
        this.fillImage.color = fillColor;
    }

    public void SetMaxResourceValue(float value)
    {
        slider.maxValue = value;
    }

    public void setResourceValue(float value)
    {
        this.slider.value = value;
    }
}
