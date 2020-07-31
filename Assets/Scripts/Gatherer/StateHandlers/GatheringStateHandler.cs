using Assets.Scripts.Resources;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Gatherer.StateHandlers
{
    class GatheringStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.Gathering;
        public async Task<GathererState> HandleState(GathererBehavior data)
        {
            if (data.AttemptToEnsureTarget<IResource>(
                gameObject => true,
                (resource, distance) =>
                {
                    if (resource != null && resource.amount > 1E-05)
                    {
                        var type = resource._type;
                        return -(distance / data.gatheringWeights[type]);
                    }
                    return float.MinValue;
                }))
            {
                data.timeTracker.startTrackingResource(data.objectSeeker.CurrentTarget.GetComponent<IResource>()._type);
            };
            var touchedObject = data.objectSeeker.seekTargetToTouch();
            if (touchedObject != null)
            {
                await data.eatResource(touchedObject);
            }
            if(data.inventory is ISpaceFillingInventory<ResourceType> spaceFilling)
            {
                if(spaceFilling.getFullRatio() >= 1)
                {
                    return GathererState.GoingHome;
                }
            }else
            {
                // only collect one thing at a time if our inventory has no capacity
                return GathererState.GoingHome;
            }
            return GathererState.Gathering;
        }

        public GathererState validPreviousStates => GathererState.GoingHome | GathererState.Selling;
        public void TransitionIntoState(GathererBehavior data)
        {
        }

        public GathererState validNextStates => GathererState.GoingHome;
        public void TransitionOutOfState(GathererBehavior data)
        {
            data.timeTracker.pauseTracking();
        }
    }
}