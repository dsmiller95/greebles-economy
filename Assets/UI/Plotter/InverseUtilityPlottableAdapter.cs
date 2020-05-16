using Assets.Economics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseUtilityPlottableAdapter : MonoBehaviour, IPlottable
{
    public float increment = 1f;
    public WeightedRegion[] weightedRegions;
    private IUtilityFunction utilityFunction;

    public InverseUtilityPlottableAdapter()
    {
        this.utilityFunction = new InverseWeightedUtility(weightedRegions);
    }
    public float PlotAt(float x)
    {
        return this.utilityFunction.GetIncrementalUtility(x, this.increment);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
