using Assets.Scripts.Resources.Inventory;
using System;
using System.Collections.Generic;
using TradeModeling.Inventories;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Resources.UI
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

        private Dictionary<ResourceType, ResourceBar> resourceBars = new Dictionary<ResourceType, ResourceBar>();

        void Awake()
        {
            for (var i = 0; i < resourceConfiguration.Length; i++)
            {
                var newBar = Instantiate(ResourceBarPrefab, transform);
                newBar.transform.position += transform.TransformVector((Vector3)(offset * i));
                var config = resourceConfiguration[i];
                var resourceBar = newBar.GetComponent<ResourceBar>();

                resourceBar.setResourceType(config.type, config.icon, ResourceConfiguration.resourceColoring[config.type]);

                resourceBars[config.type] = resourceBar;
            }

            inventoryToTrack.ResourceCapacityChangedAsObservable()
                .Subscribe(change =>
                {
                    setMaxForType(change.type, change.newValue);
                }).AddTo(this);
            inventoryToTrack.ResourceAmountsChangedAsObservable()
                .Subscribe(change =>
                {
                    setValue(change.type, change.newValue);
                }).AddTo(this);
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