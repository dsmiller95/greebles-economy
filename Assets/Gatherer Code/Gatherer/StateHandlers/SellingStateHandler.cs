using Assets.Gatherer_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SellingStateHandler : GenericStateHandler<GathererState, Gatherer>
{
    public GathererState stateHandle => GathererState.Selling;
    public GathererState HandleState(Gatherer data)
    {
        data.attemptToEnsureTarget(UserLayerMasks.Market,
            (gameObject, distance) => {
                if (gameObject?.GetComponent<Market>() != null)
                {
                    return -distance;
                }
                return float.MinValue;
            });
        if (data.currentTarget != null)
        {
            data.approachPositionWithDistance(data.currentTarget.transform.position, Time.deltaTime * data.speed);
            if (data.distanceToCurrentTarget() < data.touchDistance)
            {
                return GathererState.Gathering;
            }
        }
        return GathererState.Selling;
    }

    public GathererState validPreviousStates => GathererState.Gathering;
    public void TransitionIntoState(Gatherer data)
    {
    }

    public GathererState validNextStates => GathererState.Gathering;
    public void TransitionOutOfState(Gatherer data)
    {
        var market = data.currentTarget.GetComponent<Market>();
        var sellResult = market.sellAllGoodsInInventory(data.inventory);

        var timeSummary = data.timeTracker.getResourceTimeSummary();

        data.gatheringWeights = data.optimizer.generateNewWeights(data.gatheringWeights, timeSummary, sellResult);
        Debug.Log(Utilities.SerializeDictionary(data.gatheringWeights));
        data.timeTracker.clearTime();
    }
}
