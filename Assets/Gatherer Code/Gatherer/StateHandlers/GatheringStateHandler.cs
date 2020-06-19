using Assets.Gatherer_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GatheringStateHandler : GenericStateHandler<GathererState, Gatherer>
{
    public GathererState stateHandle => GathererState.Gathering;
    public async Task<GathererState> HandleState(Gatherer data)
    {
        if (data.attemptToEnsureTarget(UserLayerMasks.Resources,
            (gameObject, distance) => {
                var resource = gameObject?.GetComponent<IResource>();
                if (resource != null && resource.amount > 1E-05)
                {
                    var type = resource._type;
                    return -(distance / data.gatheringWeights[type]);
                }
                return float.MinValue;
            }))
        {
            Debug.Log($"{data.gameObject.name} Found target {data.currentTarget.gameObject.name}"); 
            data.timeTracker.startTrackingResource(data.currentTarget.GetComponent<IResource>()._type);
        };
        if (data.seekTargetToTouch())
        {
            Debug.Log($"{data.gameObject.name} Touched target {data.currentTarget.gameObject.name}");
            await data.eatResource(data.currentTarget);
            data.ClearCurrentTarget();
        }

        if (data.inventory.getFullRatio() >= 1)
        {
            return GathererState.GoingHome;
        }
        return GathererState.Gathering;
    }

    public GathererState validPreviousStates => GathererState.GoingHome | GathererState.Selling;
    public void TransitionIntoState(Gatherer data)
    {
    }

    public GathererState validNextStates => GathererState.GoingHome;
    public void TransitionOutOfState(Gatherer data)
    {
        data.timeTracker.pauseTracking();
    }
}
