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
    public SpaceFillingInventory<ResourceType> _inventory;

    private void Awake()
    {
        exchangeRates = sellPrices.ToDictionary(x => x.type, x => x.price);
        this._inventory = inventory.backingInventory;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public ResourceSellResult PurchaseItemInto(SpaceFillingInventory<ResourceType> inventory, ResourceType type, float amountToPurchase, bool executePurchase)
    {
        var withdrawn = _inventory.transferResourceInto(type, inventory, amountToPurchase, executePurchase);
        if (executePurchase)
        {
            var value = this.exchangeRates[type] * withdrawn;
            inventory.Consume(ResourceType.Gold, value);
        }

        return new ResourceSellResult(withdrawn, exchangeRates[type]);
    }

    public ResourceSellResult SellItemFrom(SpaceFillingInventory<ResourceType> inventory, ResourceType type, float amount, bool executeSale)
    {
        var deposited = inventory.transferResourceInto(type, _inventory, amount, executeSale);
        if (executeSale)
        {
            var value = this.exchangeRates[type] * deposited;
            inventory.addResource(ResourceType.Gold, value);
        }

        return new ResourceSellResult(deposited, exchangeRates[type]);
    }

    public Dictionary<ResourceType, ResourceSellResult> sellAllGoodsInInventory(SpaceFillingInventory<ResourceType> inventory)
    {
        return SellAllGoods(inventory, _inventory, ResourceConfiguration.spaceFillingItems, this.exchangeRates);
    }

    private static Dictionary<ResourceType, ResourceSellResult> SellAllGoods(SpaceFillingInventory<ResourceType> seller, SpaceFillingInventory<ResourceType> consumer, ResourceType[] types, Dictionary<ResourceType, float> prices)
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
