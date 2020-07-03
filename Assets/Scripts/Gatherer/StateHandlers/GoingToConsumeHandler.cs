using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gatherer.StateHandlers
{
    class GoingToConsumeHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.GoingHomeToEat;
        public Task<GathererState> HandleState(GathererBehavior data)
        {
            if (data.objectSeeker.seekTargetToTouch() != null)
            {
                return Task.FromResult(GathererState.Consuming);
            }
            return Task.FromResult(GathererState.GoingHomeToEat);
        }

        public GathererState validPreviousStates => GathererState.Gathering | GathererState.Selling | GathererState.GoingHome;
        public void TransitionIntoState(GathererBehavior data)
        {
            data.objectSeeker.BeginApproachingNewTarget(data.home.gameObject);
        }

        public GathererState validNextStates => GathererState.Consuming;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}