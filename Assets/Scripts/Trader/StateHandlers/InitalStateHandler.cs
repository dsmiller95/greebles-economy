using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Trader.StateHandlers
{
    public class InitalStateHandler : GenericStateHandler<TraderState, TraderBehavior>
    {
        public TraderState stateHandle => TraderState.Initial;

        public Task<TraderState> HandleState(TraderBehavior data)
        {
            if (data.hasTradeNodeTarget)
            {
                return Task.FromResult(TraderState.TradeTransit);
            }
            return Task.FromResult(TraderState.Initial);
        }

        public TraderState validPreviousStates => TraderState.None;
        public void TransitionIntoState(TraderBehavior data)
        {
        }

        public TraderState validNextStates => ~TraderState.Initial;
        public void TransitionOutOfState(TraderBehavior data)
        {
        }
    }
}
