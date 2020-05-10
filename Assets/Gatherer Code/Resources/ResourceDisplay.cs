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

    public ResourceInventory inventoryToTrack;

    private Dictionary<ResourceType, ResourceBar> resourceBars = new Dictionary<ResourceType, ResourceBar>();

    void Awake()
    {
        for (var i = 0; i < resourceConfiguration.Length; i++)
        {
            var newBar = Instantiate(ResourceBarPrefab, this.transform);
            newBar.transform.position += this.transform.TransformVector((Vector3)(offset * i));
            var config = resourceConfiguration[i];
            var resourceBar = newBar.GetComponent<ResourceBar>();

            resourceBar.setResourceType(config.type, config.icon, config.fillColor);

            resourceBars[config.type] = resourceBar;
        }
        this.inventoryToTrack.resourceAmountChanges += (sender, change) => {
            this.setValue(change.type, change.newValue);
        };
        this.inventoryToTrack.resourceCapacityChanges += (sender, change) => {
            this.setMaxForType(change.type, change.newValue);
        };
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void setValue(ResourceType type, float value)
    {
        resourceBars[type]?.setResourceValue(value);
    }

    private void setMaxForType(ResourceType type, float max)
    {
        resourceBars[type]?.SetMaxResourceValue(max);
    }
}
