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

        // This state can act as an exit or failsafe state to back into
        //  if something has gone wrong when handling any other state and it must be aborted
        //  for example if a UI action invalidates the current behavior
        public TraderState validPreviousStates => ~TraderState.Initial;
        public void TransitionIntoState(TraderBehavior data)
        {
        }

        public TraderState validNextStates => ~TraderState.Initial;
        public void TransitionOutOfState(TraderBehavior data)
        {
        }
    }
}
