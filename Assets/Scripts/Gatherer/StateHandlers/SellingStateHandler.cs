using Assets.Scripts.Market;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.UI;
using Assets.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Gatherer.StateHandlers
{
    /// <summary>
    /// Will find the nearest market and sell all goods in the inventory when it reaches the market
    /// 
    /// Will also adjust the gathering weights based on the optimizer attached to the Gatherer
    /// </summary>
    class SellingStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public void InstantiateOnObject(GathererBehavior obj)
        {
            var weightsChart = obj.gameObject.AddComponent<ResourceDictionaryTimeSeries>();
            obj.stateData[stateHandle] = new SellingStateData
            {
                weightsChart = weightsChart
            };
            weightsChart.values = obj.gatheringWeights;
            weightsChart.resourcePlotConfig = new[]
            {
                new ResourceGraphConfiguration
                {
                    type = ResourceType.Food,
                    yScale = 1f
                },
                new ResourceGraphConfiguration
                {
                    type = ResourceType.Wood,
                    yScale = 1f
                }
            };
            weightsChart.timeRange = 360f;
            weightsChart.totalSteps = 180f;
        }
        public class SellingStateData
        {
            public ResourceDictionaryTimeSeries weightsChart;
        }


        public GathererState stateHandle => GathererState.Selling;
        public Task<GathererState> HandleState(GathererBehavior data)
        {
            var sellingStateDate = data.stateData[stateHandle] as SellingStateData;
            data.AttemptToEnsureTarget(gameObject => gameObject.GetComponentInChildren<MarketBehavior>() != null,
                (gameObject, distance) =>   
                {
                    if (gameObject?.GetComponentInChildren<MarketBehavior>() != null)
                    {
                        return -distance;
                    }
                    return float.MinValue;
                });
            var touchedTarget = data.objectSeeker.seekTargetToTouch();
            if (touchedTarget != null)
            {
                var initialInventory = ResourceConfiguration.spaceFillingItems.ToDictionary(type => type, type => data.inventory.Get(type));

                var market = touchedTarget.GetComponentInChildren<MarketBehavior>();

                var exchangeAdapter = market.GetExchangeAdapter();
                var optimizer = new PurchaseOptimizer<ResourceType, SpaceFillingInventory<ResourceType>, SpaceFillingInventory<ResourceType>>(
                    data.inventory,
                    market._inventory,
                    ResourceConfiguration.spaceFillingItems,
                    exchangeAdapter, exchangeAdapter,
                    data.utilityFunction
                    );

                var ledger = optimizer.Optimize();
                var sourceUtilities = (new UtilityAnalyzer<ResourceType>()).GetTotalUtilityByInitialResource(
                    ResourceConfiguration.spaceFillingItems,
                    data.inventory,
                    ledger,
                    data.utilityFunction);

                //Debug.Log("Ledger");
                //foreach (var transaction in ledger)
                //{
                //    Debug.Log($"Sold: {transaction.Item1?.amount} {this.str(transaction.Item1?.type)}");
                //    foreach (var bought in transaction.Item2.exchages)
                //    {
                //        Debug.Log($"Bought: {bought.amount} {this.str(bought.type)}");
                //    }
                //}


                var timeSummary = data.timeTracker.getResourceTimeSummary();

                //Debug.Log(data.inventory.ToString(x => Enum.GetName(typeof(ResourceType), x)));
                //Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(sourceUtilities));
                //Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(timeSummary));

                data.gatheringWeights = data.optimizer.nextWeights(timeSummary, sourceUtilities);
                sellingStateDate.weightsChart.values = data.gatheringWeights;

                //Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(data.gatheringWeights));
                data.timeTracker.clearTime();
                return Task.FromResult(GathererState.GoingHomeToEat);
            }
            return Task.FromResult(GathererState.Selling);
        }

        private string str(ResourceType? type)
        {
            if (type.HasValue)
            {
                return Enum.GetName(typeof(ResourceType), type);
            }
            else
            {
                return "none";
            }
        }

        public GathererState validPreviousStates => GathererState.GoingHome | GathererState.WaitingForMarket;
        public void TransitionIntoState(GathererBehavior data)
        {
            data.attachBackpack();
            data.home.withdrawAllGoods(data.inventory);
        }

        public GathererState validNextStates => GathererState.GoingHomeToEat;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}