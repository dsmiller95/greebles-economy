using Assets.Scripts.Resources;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gatherer.StateHandlers
{
    public class ConsumingStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        private float timeDelay;
        public ConsumingStateHandler(float timeDelay = 0.2f)
        {
            this.timeDelay = timeDelay;
        }

        struct ConsumingStateData
        {
            public float lastConsumedTime;
        }

        public GathererState stateHandle => GathererState.Consuming;

        public Task<GathererState> HandleState(GathererBehavior data)
        {
            ConsumingStateData myState = data.stateData[GathererState.Consuming];
            if ((myState.lastConsumedTime + timeDelay) < Time.time)
            {
                data.stateData[GathererState.Consuming] = new ConsumingStateData { lastConsumedTime = Time.time };
                var firstAvailableResource = data.StuffIEat
                    .Where(type => data.inventory.Get(type) > 0)
                    .FirstOrDefault();

                if (firstAvailableResource == default)
                {
                    return Task.FromResult(GathererState.Gathering);
                }
                data.inventory.Consume(firstAvailableResource, 1);
            }
            return Task.FromResult(GathererState.Consuming);
        }

        public GathererState validPreviousStates => GathererState.GoingHomeToEat;
        public void TransitionIntoState(GathererBehavior data)
        {
            data.stateData[GathererState.Consuming] = new ConsumingStateData
            {
                lastConsumedTime = Time.time
            };
            data.currentTarget = null;
        }

        public GathererState validNextStates => GathererState.Gathering;
        public void TransitionOutOfState(GathererBehavior data)
        {
            data.removeBackpack();
        }
    }
}