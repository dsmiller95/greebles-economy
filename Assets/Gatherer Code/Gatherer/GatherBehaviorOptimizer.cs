using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Gatherer_Code;
using System.Linq;
using System;

public class GatherBehaviorOptimizer
{
    public const float weightChangeFactor = 0.6f;

    private Queue<IDictionary<ResourceType, float>> pastWeights;
    public int rollingAverageWindow = 6;

    public GatherBehaviorOptimizer()
    {
        this.pastWeights = new Queue<IDictionary<ResourceType, float>>();
        this.pastWeights.Enqueue(this.generateInitialWeights());
    }

    public IDictionary<ResourceType, float> nextWeights(
        IDictionary<ResourceType, float> timeSpent,
        IDictionary<ResourceType, float> utilityPerResource)
    {
        //TODO: counteract the single-gather case. If the agent only gathered one type of resource, We shouldn't make much if any changes to the weights
        // Because no new comparitive information was gained
        var utilityGained = utilityPerResource.Keys
            .Where(type => !float.IsNaN(utilityPerResource[type]))
            .Select(type => {
                var gainedUtility = utilityPerResource[type];
                var time = timeSpent[type];
                return new
                {
                    type,
                    utility = gainedUtility
                };
            });

        var totalUtilityPerResource = utilityGained.Select(x => x.utility).Sum();
        var newWeightsGenerated = utilityGained.Select(pair => new { pair.type, weight = pair.utility / totalUtilityPerResource });

        var regeneratedWeights = newWeightsGenerated.ToDictionary(pair => pair.type, pair => pair.weight);

        var nextWeights = new Dictionary<ResourceType, float>(this.pastWeights.Last());
        ApplyNewWeightsByFactorInPlace(nextWeights, regeneratedWeights, 1);
        this.pastWeights.Enqueue(nextWeights);
        if (this.pastWeights.Count > rollingAverageWindow)
        {
            this.pastWeights.Dequeue();
        }

        return this.AverageOverAll(this.pastWeights);
    }

    /// <summary>
    /// Take the weights in the input dictionary relative to each other, and use them to adjust the weights in the base value
    /// This assumes both dictionaries are normalized to sum to 1
    /// If a key is missing from the input dictionary relative to the baseValue; the baseValue's entry will not be changed
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="input"></param>
    /// <param name="factor"></param>
    private void ApplyNewWeightsByFactorInPlace<T>(IDictionary<T, float> baseValue, IDictionary<T, float> input, float factor)
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

    // TODO: allow for partial ratios to be used in each dictionary in the list. If a partial ratio is turned into a full
    //     ratio and stored, it can incorrectly reinforce a preference based on the previous ratio baked into it
    private IDictionary<T, float> AverageOverAll<T>(Queue<IDictionary<T, float>> dictionaries)
    {
        var totalDictionaries = dictionaries.Count;
        return dictionaries.Peek()
            .Select(x => x.Key)
            .ToDictionary(key => key, key =>
            {
                return dictionaries.Select(x => x[key]).Sum() / totalDictionaries;
            });
    }

    private IDictionary<T, float> averageInPlace<T>(IDictionary<T, float> baseValue, IDictionary<T, float> input, float inputWeight)
    {
        return baseValue
            .Select(pair => new { pair.Key, Value = (pair.Value * (1 - inputWeight)) + (input[pair.Key] * inputWeight) })
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public IDictionary<ResourceType, float> generateInitialWeights()
    {
        return new Dictionary<ResourceType, float>
        {
            {ResourceType.Food, 0.5f},
            {ResourceType.Wood, 0.5f}
        };
    }
}
