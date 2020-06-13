using Assets.Gatherer_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Inventories;
using UnityEngine;


/// <summary>
/// Will find the nearest market and sell all goods in the inventory when it reaches the market
/// 
/// Will also adjust the gathering weights based on the optimizer attached to the Gatherer
/// </summary>
class SellingStateHandler : GenericStateHandler<GathererState, Gatherer>
{
    public GathererState stateHandle => GathererState.Selling;
    public GathererState HandleState(Gatherer data)
    {
        data.attemptToEnsureTarget(UserLayerMasks.Market,
            (gameObject, distance) => {
                if (gameObject?.GetComponent<Market>() != null)
                {
                    return -distance;
                }
                return float.MinValue;
            });
        if (data.seekTargetToTouch())
        {
            var market = data.currentTarget.GetComponent<Market>();
            // TODO:

            var exchangeAdapters = market.GetExchangeAdapter();
            var optimizer = new PurchaseOptimizer<ResourceType, SpaceFillingInventory<ResourceType>, SpaceFillingInventory<ResourceType>>(
                data.inventory,
                market._inventory,
                ResourceConfiguration.spaceFillingItems,
                exchangeAdapters, exchangeAdapters,
                data.utilityFunction
                );

            var ledger = optimizer.Optimize();
            var sourceUtilities = (new UtilityAnalyzer<ResourceType>()).GetUtilityPerInitialResource(
                ResourceConfiguration.spaceFillingItems,
                data.inventory,
                ledger,
                data.utilityFunction);

            Debug.Log("Ledger");
            foreach (var transaction in ledger)
            {
                Debug.Log($"Sold: {transaction.Item1?.amount} {this.str(transaction.Item1?.type)}");
                foreach (var bought in transaction.Item2.exchages)
                {
                    Debug.Log($"Bought: {bought.amount} {this.str(bought.type)}");
                }
            }
            Debug.Log(data.inventory.ToString(x => Enum.GetName(typeof(ResourceType), x)));
            Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(sourceUtilities));

            var timeSummary = data.timeTracker.getResourceTimeSummary();

            data.gatheringWeights = data.optimizer.generateNewWeights(data.gatheringWeights, timeSummary, sourceUtilities);
            Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(data.gatheringWeights));
            data.timeTracker.clearTime();
            return GathererState.GoingHomeToEat;
        }
        return GathererState.Selling;
    }

    private string str(ResourceType? type)
    {
        if (type.HasValue)
        {
            return Enum.GetName(typeof(ResourceType), type);
        } else
        {
            return "none";
        }
    }

    public GathererState validPreviousStates => GathererState.GoingHome;
    public void TransitionIntoState(Gatherer data)
    {

    }

    public GathererState validNextStates => GathererState.GoingHomeToEat;
    public void TransitionOutOfState(Gatherer data)
    {
    }
}
