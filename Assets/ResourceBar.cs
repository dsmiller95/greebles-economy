using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Slider slider;

    public ResourceType type = ResourceType.Gold;

    public void setResourceType(ResourceType type)
    {
        this.type = type;
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
