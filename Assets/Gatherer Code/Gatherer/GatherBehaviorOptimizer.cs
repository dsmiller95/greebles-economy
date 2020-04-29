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
        var profitPerTime = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>()
            .Select(type => new { type, profitPerTime = 
                sellResults.ContainsKey(type) && timeSpent.ContainsKey(type) 
                    ? sellResults[type].totalRevenue / timeSpent[type] 
                    : 0 });

        var totalProfitPerTime = profitPerTime.Select(x => x.profitPerTime).Sum();
        var normalizedProfitPerTime = profitPerTime.Select(pair => new { pair.type, weight = pair.profitPerTime / totalProfitPerTime });

        var regeneratedWeights = normalizedProfitPerTime.ToDictionary(pair => pair.type, pair => pair.weight);

        return averageInPlace(regeneratedWeights, previousWeights);
    }

    private Dictionary<T, float> averageInPlace<T>(Dictionary<T, float> average, Dictionary<T, float> input)
    {
        return average
            .Select(pair => new { pair.Key, Value = (pair.Value + input[pair.Key]) / 2 })
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
