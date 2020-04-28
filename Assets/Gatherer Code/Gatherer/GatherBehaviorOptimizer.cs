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

        Debug.Log("generating weights");

        var profitPerTime = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>()
            .Select(type => new { type, profitPerTime = 
                sellResults.ContainsKey(type) && timeSpent.ContainsKey(type) 
                    ? sellResults[type].totalRevenue / timeSpent[type] 
                    : 0 });
        Debug.Log(Gatherer.serializeDictionary(profitPerTime.ToDictionary(x => x.type, x => x.profitPerTime)));

        var totalProfitPerTime = profitPerTime.Select(x => x.profitPerTime).Sum();
        var normalizedProfitPerTime = profitPerTime.Select(pair => new { pair.type, weight = pair.profitPerTime / totalProfitPerTime });

        //TODO: move the previous weigh towards the result, instead of replacing it
        return normalizedProfitPerTime.ToDictionary(pair => pair.type, pair => pair.weight);
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
