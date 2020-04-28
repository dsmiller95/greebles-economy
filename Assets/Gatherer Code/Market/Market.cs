using Assets.Gatherer_Code;
using Assets.Gatherer_Code.Market;
using System.Collections;
using System.Collections.Generic;
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

    public ResourceSellResult sellAllGoods(ResourceInventory inventory)
    {
        var result = new ResourceSellResult();
        result.FoodResult = inventory.getResource(ResourceType.Food) * foodPrice;
        result.WoodResult = inventory.getResource(ResourceType.Wood) * woodPrice;
        inventory.empty();
        return result;
    }
}
