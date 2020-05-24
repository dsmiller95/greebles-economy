using Assets.Economics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseUtilityPlottableAdapter : MonoBehaviour, IPlottableFunction
{
    public float increment = 1f;
    public WeightedRegion[] weightedRegions;
    public float offset = 1;
    private IIncrementalFunction utilityFunction;

    void Awake()
    {
        this.utilityFunction = new InverseWeightedUtility(weightedRegions, offset);
    }
    public float PlotAt(float x)
    {
        return this.utilityFunction.GetIncrementalValue(x, this.increment);
    }
}
