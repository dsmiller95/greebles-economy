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

        class ConsumingStateData
        {
            public float lastConsumedTime;
            public bool ateFood;
        }

        public GathererState stateHandle => GathererState.Consuming;

        public Task<GathererState> HandleState(GathererBehavior data)
        {
            ConsumingStateData myState = data.stateData[GathererState.Consuming];
            if ((myState.lastConsumedTime + timeDelay) < Time.time)
            {
                var inventoryToConsumeFrom = data.home.inventory.backingInventory;
                myState.lastConsumedTime = Time.time;
                var firstAvailableResource = data.StuffIEat
                    .Where(type => inventoryToConsumeFrom.Get(type) > 0)
                    .FirstOrDefault();

                if (firstAvailableResource == default)
                {
                    //if no food, guess I'll die?
                    // otherwise time to slep
                    return Task.FromResult(myState.ateFood ? GathererState.Sleeping : GathererState.Dying);
                }
                inventoryToConsumeFrom.Consume(firstAvailableResource, 1);
                if(firstAvailableResource == ResourceType.Food)
                {
                    myState.ateFood = true;
                }
            }
            return Task.FromResult(GathererState.Consuming);
        }

        public GathererState validPreviousStates => GathererState.GoingHomeToEat;
        public void TransitionIntoState(GathererBehavior data)
        {
            data.stateData[GathererState.Consuming] = new ConsumingStateData
            {
                lastConsumedTime = Time.time,
                ateFood = false
            };
            data.objectSeeker.ClearCurrentTarget();
            data.inventory.DrainAllInto(data.home.inventory.backingInventory, ResourceConfiguration.spaceFillingItems);
            data.removeBackpack();
        }

        public GathererState validNextStates => GathererState.Sleeping | GathererState.Dying;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}