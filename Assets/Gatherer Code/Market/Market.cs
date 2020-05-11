using Assets.Gatherer_Code;
using Assets.Gatherer_Code.Market;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Market : MonoBehaviour
{

    public float woodPrice = 1;
    public float foodPrice = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<ResourceType, ResourceSellResult> sellAllGoodsInInventory(ResourceInventory inventory)
    {
        var result = new Dictionary<ResourceType, ResourceSellResult>();
        result[ResourceType.Food] = new ResourceSellResult(inventory.getResource(ResourceType.Food), foodPrice);
        result[ResourceType.Wood] = new ResourceSellResult(inventory.getResource(ResourceType.Wood), woodPrice);
        inventory.emptySpaceFillingInventory();

        inventory.addResource(ResourceType.Gold, result.Values.Sum(sellResult => sellResult.totalRevenue));
        return result;
    }
}
