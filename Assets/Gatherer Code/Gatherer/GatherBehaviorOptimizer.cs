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
    private Queue<IDictionary<ResourceType, float>> pastUtilities;
    private Queue<IDictionary<ResourceType, float>> pastTimeCosts;

    public int rollingAverageWindow = 6;

    public GatherBehaviorOptimizer()
    {
        this.pastWeights = new Queue<IDictionary<ResourceType, float>>();
        this.pastWeights.Enqueue(this.generateInitialWeights());

        this.pastUtilities = new Queue<IDictionary<ResourceType, float>>();
        this.pastTimeCosts = new Queue<IDictionary<ResourceType, float>>();
    }

    public IDictionary<ResourceType, float> nextWeights(
        IDictionary<ResourceType, float> timeSpentTotal,
        IDictionary<ResourceType, float> totalUtilityPerResource)
    {
        this.AddInRollingWindow(this.pastUtilities, new Dictionary<ResourceType, float>(totalUtilityPerResource));
        this.AddInRollingWindow(this.pastTimeCosts, new Dictionary<ResourceType, float>(timeSpentTotal));

        var summedTimeSpend = pastTimeCosts.SumTogether();
        var summedUtilities = pastUtilities.SumTogether();

        Debug.Log("Summation info");
        Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(summedUtilities));
        Debug.Log(TradeModeling.MyUtilities.SerializeEnumDictionary(summedTimeSpend));

        return this.GetWeightsFromTimeSpentAndUtilityGained(summedTimeSpend, summedUtilities);
    }

    private IDictionary<ResourceType, float> GetWeightsFromTimeSpentAndUtilityGained(
        IDictionary<ResourceType, float> timeSpent,
        IDictionary<ResourceType, float> utilityPerResource)
    {
        var utilityGainedPerTime = utilityPerResource.Keys
            .Where(type => 
                !float.IsNaN(utilityPerResource[type]) && utilityPerResource[type] > 0f
                && timeSpent.ContainsKey(type) && timeSpent[type] > 0f)
            .ToDictionary(type => type, type => {
                var gainedUtility = utilityPerResource[type];
                var time = timeSpent[type];
                return gainedUtility / time;
            });

        var totalUtilityPerResourcePerTime = utilityGainedPerTime.Select(x => x.Value).Sum();
        // In some cases we may not have gathered any of some types of resources
        //  We will take the average value of all entries in the dictionary and pad
        //  the resources we haven't gathered with that average
        var averageUtilityPResourcePTime = totalUtilityPerResourcePerTime / utilityGainedPerTime.Count();
        var paddedUtilityPerTime = ResourceConfiguration.spaceFillingItems.ToDictionary(
            resource => resource,
            resource => utilityGainedPerTime.ContainsKey(resource) ? utilityGainedPerTime[resource] : averageUtilityPResourcePTime);

        var newWeightsGenerated = paddedUtilityPerTime.Normalize();

        return newWeightsGenerated;
    }

    private void AddInRollingWindow<T>(Queue<T> queue, T value)
    {
        queue.Enqueue(value);
        if (queue.Count > rollingAverageWindow)
        {
            queue.Dequeue();
        }

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
