using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Home : MonoBehaviour
{
    public ResourceInventory inventory;

    public void depositAllGoods(ResourceInventory inventoryToDrain)
    {
        inventoryToDrain.drainAllInto(inventory);
    }
    public void withdrawAllGoods(ResourceInventory inventoryToDepositTo)
    {
        inventory.drainAllInto(inventoryToDepositTo);
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
