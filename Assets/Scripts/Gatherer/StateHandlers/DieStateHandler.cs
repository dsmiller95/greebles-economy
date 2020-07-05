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
    public class DieStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.Dying;

        public Task<GathererState> HandleState(GathererBehavior data)
        {
            Debug.LogWarning("Blarghaghghahghghg");
            data.Die();
            return Task.FromResult(GathererState.Dying);
        }

        public GathererState validPreviousStates => GathererState.Consuming;
        public void TransitionIntoState(GathererBehavior data)
        {
        }

        public GathererState validNextStates => GathererState.None;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}