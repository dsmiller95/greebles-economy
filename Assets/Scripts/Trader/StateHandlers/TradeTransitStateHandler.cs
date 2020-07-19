using Assets.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Trader.StateHandlers
{
    public class TradeTransitStateHandler : GenericStateHandler<TraderState, TraderBehavior>
    {
        public TradeTransitStateHandler()
        {
        }

        public TraderState stateHandle => TraderState.TradeTransit;

        public Task<TraderState> HandleState(TraderBehavior data)
        {
            if(data.objectSeeker.CurrentTarget == null)
            {
                // if we never had a target, or if our current target was destroyed
                // abort and re-evaluate our life
                return Task.FromResult(TraderState.Initial);
            }
            if (data.objectSeeker.seekTargetToTouch())
            {
                return Task.FromResult(TraderState.TradeExecute);
            }
            return Task.FromResult(TraderState.TradeTransit);
        }

        public TraderState validPreviousStates => ~TraderState.TradeTransit;
        public void TransitionIntoState(TraderBehavior data)
        {
            data.objectSeeker.BeginApproachingNewTarget(data.currentTradeNodeTarget?.target.gameObject);
        }

        public TraderState validNextStates => TraderState.TradeExecute | TraderState.Initial;
        public void TransitionOutOfState(TraderBehavior data)
        {
        }
    }
}
