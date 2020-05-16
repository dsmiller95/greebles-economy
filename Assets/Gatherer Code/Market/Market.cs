using Assets.Gatherer_Code;
using Assets.Gatherer_Code.Market;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct SellPrice
{
    public ResourceType type;
    public float price;
}

public class Market : MonoBehaviour
{
    public SellPrice[] sellPrices;


    private Dictionary<ResourceType, float> exchangeRates;
    public ResourceInventory inventory;

    private void Awake()
    {
        exchangeRates = sellPrices.ToDictionary(x => x.type, x => x.price);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public ResourceSellResult PurchaseItemInto(ResourceInventory inventory, ResourceType type, float amountToPurchase, bool executePurchase)
    {
        var withdrawn = this.inventory.transferResourceInto(type, inventory, amountToPurchase, executePurchase);
        if (executePurchase)
        {
            var value = this.exchangeRates[type] * withdrawn;
            inventory.Consume(ResourceType.Gold, value);
        }

        return new ResourceSellResult(withdrawn, exchangeRates[type]);
    }

    public ResourceSellResult SellItemFrom(ResourceInventory inventory, ResourceType type, float amount, bool executeSale)
    {
        var deposited = inventory.transferResourceInto(type, this.inventory, amount, executeSale);
        if (executeSale)
        {
            var value = this.exchangeRates[type] * deposited;
            inventory.addResource(ResourceType.Gold, value);
        }

        return new ResourceSellResult(deposited, exchangeRates[type]);
    }

    public Dictionary<ResourceType, ResourceSellResult> sellAllGoodsInInventory(ResourceInventory inventory)
    {
        return SellAllGoods(inventory, this.inventory, ResourceInventory.spaceFillingItems, this.exchangeRates);
    }

    private static Dictionary<ResourceType, ResourceSellResult> SellAllGoods(ResourceInventory seller, ResourceInventory consumer, ResourceType[] types, Dictionary<ResourceType, float> prices)
    {
        var result = seller.DrainAllInto(consumer, types)
            .Select(pair =>
            {
                var sellResult = new ResourceSellResult(pair.Value, prices[pair.Key]);
                return new { type = pair.Key, sellResult };
            })
            .ToDictionary(x => x.type, x => x.sellResult);

        seller.addResource(ResourceType.Gold, result.Values.Sum(sellResult => sellResult.totalRevenue));
        return result;
    }
}
