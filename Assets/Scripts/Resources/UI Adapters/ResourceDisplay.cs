using Assets.Scrips.Resources.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scrips.Resources.UI
{
    public class ResourceDisplay : MonoBehaviour
    {
        [Serializable]
        public struct ResourceBarConfiguration
        {
            public ResourceType type;
            public Sprite icon;
        }

        public GameObject ResourceBarPrefab;

        public ResourceBarConfiguration[] resourceConfiguration;
        public Vector2 offset = new Vector2(0, 10);

        public ResourceInventory inventoryToTrack;
        public NotifyingInventory<ResourceType> _inventoryToTrack;

        private Dictionary<ResourceType, ResourceBar> resourceBars = new Dictionary<ResourceType, ResourceBar>();

        void Awake()
        {
            _inventoryToTrack = inventoryToTrack.backingInventory;
            for (var i = 0; i < resourceConfiguration.Length; i++)
            {
                var newBar = Instantiate(ResourceBarPrefab, this.transform);
                newBar.transform.position += this.transform.TransformVector((Vector3)(offset * i));
                var config = resourceConfiguration[i];
                var resourceBar = newBar.GetComponent<ResourceBar>();

                resourceBar.setResourceType(config.type, config.icon, ResourceConfiguration.resourceColoring[config.type]);

                resourceBars[config.type] = resourceBar;
            }
            this._inventoryToTrack.resourceAmountChanges += (sender, change) =>
            {
                this.setValue(change.type, change.newValue);
            };
            this._inventoryToTrack.resourceCapacityChanges += (sender, change) =>
            {
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
}