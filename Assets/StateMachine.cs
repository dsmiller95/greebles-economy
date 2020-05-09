using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class StateMachine<T> where T : Enum
    {
        public struct StateChangeHandler
        {
            public T fromState;
            public T toState;
            public Action handler;
        }

        private T state;

        private Dictionary<T, Func<T>> updateHandlers;
        private ICollection<StateChangeHandler> stateChangeHandlers;


        public StateMachine(T initalState)
        {
            updateHandlers = new Dictionary<T, Func<T>>();
            this.state = initalState;
            this.stateChangeHandlers = new List<StateChangeHandler>();
        }

        public void registerStateHandler(T state, Func<T> handler)
        {
            updateHandlers[state] = handler;
        }

        public void registerStateTransitionHandler(T initialState, T endState, Action handler)
        {
            this.stateChangeHandlers.Add(new StateChangeHandler
            {
                fromState = initialState,
                toState = endState,
                handler = handler
            });
        }

        public void update()
        {
            var updateAction = updateHandlers[state];
            if (updateAction == null)
            {
                throw new NotImplementedException($"no state handler found for state {Enum.GetName(typeof(T), state)}");
            }
            var newState = updateAction();

            if(!newState.Equals(state))
            {
                foreach (var changeHandler in stateChangeHandlers)
                {
                    if (changeHandler.fromState.HasFlag(state) && changeHandler.toState.HasFlag(newState))
                    {
                        changeHandler.handler();
                    }
                }
            }

            this.state = newState;
        }
    }
}
