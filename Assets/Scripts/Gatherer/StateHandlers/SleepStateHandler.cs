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
    public class SleepStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.Sleeping;

        public Task<GathererState> HandleState(GathererBehavior data)
        {
            if(TimeController.instance.GetTimezone() == Timezone.Day)
            {
                return Task.FromResult(GathererState.GoingHome);
            }
            return Task.FromResult(GathererState.Sleeping);
        }

        public GathererState validPreviousStates => GathererState.Consuming;
        public void TransitionIntoState(GathererBehavior data)
        {
        }

        public GathererState validNextStates => GathererState.GoingHome;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}