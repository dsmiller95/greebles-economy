using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Gatherer_Code;
using System.Linq;
using System;

public class GatherBehaviorOptimizer
{

    public const float weightChangeFactor = 0.3f;

    public Dictionary<ResourceType, float> generateNewWeights(
        Dictionary<ResourceType, float> previousWeights,
        Dictionary<ResourceType, float> timeSpent,
        Dictionary<ResourceType, ResourceSellResult> sellResults)
    {
        //TODO: counteract the single-gather case. If the agent only gathered one type of resource, We shouldn't make much if any changes to the weights
        // Because no new comparitive information was gained
        var profitPerTime = sellResults.Keys
            .Where(type => timeSpent.ContainsKey(type))
            .Select(type => {
                var profit = sellResults[type].totalRevenue;
                var time = timeSpent[type];
                return new
                {
                    type,
                    profitPerTime = time == 0 ? 0 : profit / time
                };
            });

        var totalProfitPerTime = profitPerTime.Select(x => x.profitPerTime).Sum();
        var newWeightsGenerated = profitPerTime.Select(pair => new { pair.type, weight = pair.profitPerTime / totalProfitPerTime });

        var regeneratedWeights = newWeightsGenerated.ToDictionary(pair => pair.type, pair => pair.weight);

        ApplyNewWeightsByFactorInPlace(previousWeights, regeneratedWeights, weightChangeFactor);

        return previousWeights;
    }

    /// <summary>
    /// Take the weights in the input dictionary relative to each other, and use them to adjust the weights in the base value
    /// This assumes both dictionaries are normalized to sum to 1
    /// If a key is missing from the input dictionary relative to the baseValue; the baseValue's entry will not be changed
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="input"></param>
    /// <param name="factor"></param>
    private void ApplyNewWeightsByFactorInPlace<T>(Dictionary<T, float> baseValue, Dictionary<T, float> input, float factor)
    {
        var sumOfIntersectingBaseValues = input.Keys
            .Select(key => baseValue[key])
            .Sum();
        foreach (var key in input.Keys)
        {
            var modifiedWeight = input[key] * sumOfIntersectingBaseValues;
            baseValue[key] = Mathf.Lerp(baseValue[key], modifiedWeight, factor);
        }
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
