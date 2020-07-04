using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gatherer.StateHandlers
{
    class GoingHomeStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.GoingHome;
        public Task<GathererState> HandleState(GathererBehavior data)
        {
            if (data.objectSeeker.seekTargetToTouch() != null)
            {
                if (data.home.depositAllGoods(data.inventory))
                {
                    //Our home is full. We have to wait till tonight before we can go to market
                    return Task.FromResult(GathererState.WaitingForMarket);
                }
                if(TimeController.instance.GetTimezone() == Timezone.Evening)
                {
                    //Time to go to market! even if our house isn't full we nave to sell our goods before sleeping
                    return Task.FromResult(GathererState.Selling);
                }
                return Task.FromResult(GathererState.Gathering);
            }
            return Task.FromResult(GathererState.GoingHome);
        }

        public GathererState validPreviousStates => GathererState.Gathering | GathererState.Sleeping;
        public void TransitionIntoState(GathererBehavior data)
        {
            data.objectSeeker.BeginApproachingNewTarget(data.home.gameObject);
        }

        public GathererState validNextStates => GathererState.Gathering | GathererState.Selling | GathererState.WaitingForMarket;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}