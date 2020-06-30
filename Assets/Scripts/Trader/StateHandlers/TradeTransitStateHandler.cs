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
            if (data.objectSeeker.seekTargetToTouch())
            {
                return Task.FromResult(TraderState.TradeExecute);
            }
            return Task.FromResult(TraderState.TradeTransit);
        }

        public TraderState validPreviousStates => ~TraderState.TradeTransit;
        public void TransitionIntoState(TraderBehavior data)
        {
            data.objectSeeker.CurrentTarget = data.currentTradeNodeTarget.target.gameObject;
        }

        public TraderState validNextStates => TraderState.TradeExecute;
        public void TransitionOutOfState(TraderBehavior data)
        {
        }
    }
}
