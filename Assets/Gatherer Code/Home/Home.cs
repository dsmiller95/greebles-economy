using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Home : MonoBehaviour
{
    public ResourceInventory inventory;

    /// <summary>
    /// Deposit all the items from the given inventory into this inventory
    /// </summary>
    /// <param name="inventoryToDrain">the inventory to drain</param>
    /// <returns>True if the home's inventory is full</returns>
    public bool depositAllGoods(ResourceInventory inventoryToDrain)
    {
        inventoryToDrain.DrainAllInto(inventory, ResourceInventory.spaceFillingItems);
        return this.inventory.getFullRatio() >= 1;
    }
    public void withdrawAllGoods(ResourceInventory inventoryToDepositTo)
    {
        inventory.DrainAllInto(inventoryToDepositTo, ResourceInventory.spaceFillingItems);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
