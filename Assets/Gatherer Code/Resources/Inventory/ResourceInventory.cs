using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gatherer_Code;
using UnityEngine;

public class ResourceInventory: MonoBehaviour
{
    public int inventoryCapacitySetForUI = 10;

    public NotifyingInventory<ResourceType> backingInventory
    {
        get;
        private set;
    }

    void Awake()
    {
        var initialInventory = new Dictionary<ResourceType, float>();
        var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        foreach (var resource in resourceTypes)
        {
            // create the key with default. Emit set events in Start(); once everyone has had a chance to subscribe to updates
            initialInventory[resource] = 0;
        }
        backingInventory = new NotifyingInventory<ResourceType>(this.inventoryCapacitySetForUI, initialInventory, ResourceConfiguration.spaceFillingItems);
    }

    public void Start()
    {
        this.backingInventory.NotifyAll(new[]
        {
            new ResourceChanged<ResourceType>(ResourceType.Gold, 200)
        });
    }
    
    public void Update()
    {
    }
}
