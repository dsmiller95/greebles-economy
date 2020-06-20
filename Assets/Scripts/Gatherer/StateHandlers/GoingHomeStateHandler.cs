using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.Gatherer.StateHandlers
{
    class GoingHomeStateHandler : GenericStateHandler<GathererState, GathererBehavior>
    {
        public GathererState stateHandle => GathererState.GoingHome;
        public Task<GathererState> HandleState(GathererBehavior data)
        {
            if (data.seekTargetToTouch())
            {
                if (data.home.depositAllGoods(data.inventory))
                {
                    //Our home is full; time to go to market
                    data.attachBackpack();
                    data.home.withdrawAllGoods(data.inventory);
                    return Task.FromResult(GathererState.Selling);
                }
                return Task.FromResult(GathererState.Gathering);
            }
            return Task.FromResult(GathererState.GoingHome);
        }

        public GathererState validPreviousStates => GathererState.Gathering;
        public void TransitionIntoState(GathererBehavior data)
        {
            data.currentTarget = data.home.gameObject;
        }

        public GathererState validNextStates => GathererState.Gathering | GathererState.Selling;
        public void TransitionOutOfState(GathererBehavior data)
        {
        }
    }
}