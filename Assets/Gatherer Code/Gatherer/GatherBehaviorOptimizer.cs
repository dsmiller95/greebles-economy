using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Gatherer_Code;
using Assets.Gatherer_Code.Market;
using System.Linq;
using System;

public class GatherBehaviorOptimizer
{

    public Dictionary<ResourceType, float> generateNewWeights(
        Dictionary<ResourceType, float> previousWeights,
        Dictionary<ResourceType, float> timeSpent,
        Dictionary<ResourceType, ResourceSellResult> sellResults)
    {
        //TODO: counteract the single-gather case. If the agent only gathered one type of resource, We shouldn't make much if any changes to the weights
        // Because no new comparitive information was gained
        var profitPerTime = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>()
            .Select(type => new { type, profitPerTime = 
                sellResults.ContainsKey(type) && timeSpent.ContainsKey(type) 
                    ? sellResults[type].totalRevenue / Math.Max(timeSpent[type], 0.1f) 
                    : 0.1f });

        var totalProfitPerTime = profitPerTime.Select(x => x.profitPerTime).Sum();
        var normalizedProfitPerTime = profitPerTime.Select(pair => new { pair.type, weight = pair.profitPerTime / totalProfitPerTime });

        var regeneratedWeights = normalizedProfitPerTime.ToDictionary(pair => pair.type, pair => pair.weight);

        return averageInPlace(previousWeights, regeneratedWeights, 0.1f);
    }

    private Dictionary<T, float> averageInPlace<T>(Dictionary<T, float> baseValue, Dictionary<T, float> input, float inputWeight)
    {
        return baseValue
            .Select(pair => new { pair.Key, Value = (pair.Value * (1 - inputWeight)) + (input[pair.Key] * inputWeight) })
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public Dictionary<ResourceType, float> generateInitialWeights()
    {
        return new Dictionary<ResourceType, float>
        {
            {ResourceType.Food, 0.5f},
            {ResourceType.Wood, 0.5f}
        };
    }
}
