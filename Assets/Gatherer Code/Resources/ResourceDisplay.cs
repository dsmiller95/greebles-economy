using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceDisplay : MonoBehaviour
{
    [Serializable]
    public struct ResourceBarConfiguration
    {
        public ResourceType type;
        public Sprite icon;
        public Color fillColor;
    }

    public GameObject ResourceBarPrefab;

    public ResourceBarConfiguration[] resourceConfiguration;
    public Vector2 offset = new Vector2(0, 10);

    private Dictionary<ResourceType, ResourceBar> resourceBars;

    // Start is called before the first frame update
    void Start()
    {
        resourceBars = new Dictionary<ResourceType, ResourceBar>();
        for(var i = 0; i < resourceConfiguration.Length; i++)
        {
            var newBar = Instantiate(ResourceBarPrefab, this.transform);
            newBar.transform.position += this.transform.TransformVector((Vector3)(offset * i));
            var config = resourceConfiguration[i];
            var resourceBar = newBar.GetComponent<ResourceBar>();

            resourceBar.setResourceType(config.type, config.icon, config.fillColor);

            resourceBars[config.type] = resourceBar;
        }
    }

    public void setValue(ResourceType type, float value)
    {
        resourceBars[type]?.setResourceValue(value);
    }

    public void setMaxForType(ResourceType type, float max)
    {
        resourceBars[type]?.SetMaxResourceValue(max);
    }
}
