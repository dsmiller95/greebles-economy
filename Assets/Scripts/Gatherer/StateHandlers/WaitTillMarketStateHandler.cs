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
    public class WaitTillMarketStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.WaitingForMarket;

        public Task<GathererState> HandleState(GathererBehavior data)
        {
            if(TimeController.instance.GetTimezone() == Timezone.Evening)
            {
                return Task.FromResult(GathererState.Selling);
            }
            return Task.FromResult(GathererState.WaitingForMarket);
        }

        public GathererState validPreviousStates => GathererState.GoingHome;
        public void TransitionIntoState(GathererBehavior data)
        {
        }

        public GathererState validNextStates => GathererState.Selling;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}