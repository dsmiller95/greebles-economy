using Assets.Gatherer_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GoingToConsumeHandler : GenericStateHandler<GathererState, Gatherer>
{
    public GathererState stateHandle => GathererState.GoingHomeToEat;
    public GathererState HandleState(Gatherer data)
    {
        if (data.seekTargetToTouch())
        {
            return GathererState.Consuming;
        }
        return GathererState.GoingHomeToEat;
    }

    public GathererState validPreviousStates => GathererState.Gathering | GathererState.Selling | GathererState.GoingHome;
    public void TransitionIntoState(Gatherer data)
    {
        data.currentTarget = data.home.gameObject;
    }

    public GathererState validNextStates => GathererState.Consuming;
    public void TransitionOutOfState(Gatherer data)
    {
    }
}
