using Assets.Gatherer_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GoingHomeStateHandler : GenericStateHandler<GathererState, Gatherer>
{
    public GathererState stateHandle => GathererState.GoingHome;
    public GathererState HandleState(Gatherer data)
    {
        if (data.seekTargetToTouch())
        {
            if (data.home.depositAllGoods(data.inventory))
            {
                //Our home is full; time to go to market
                data.attachBackpack();
                data.home.withdrawAllGoods(data.inventory);
                return GathererState.Selling;
            }
            return GathererState.Gathering;
        }
        return GathererState.GoingHome;
    }

    public GathererState validPreviousStates => GathererState.Gathering;
    public void TransitionIntoState(Gatherer data)
    {
        data.currentTarget = data.home.gameObject;
    }

    public GathererState validNextStates => GathererState.Gathering | GathererState.Selling;
    public void TransitionOutOfState(Gatherer data)
    {
    }
}
