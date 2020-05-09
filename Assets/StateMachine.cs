using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class StateMachine<T, ParamType> where T : Enum
    {
        public struct StateChangeHandler
        {
            public T fromState;
            public T toState;
            public Action<ParamType> handler;
        }

        private T state;

        private Dictionary<T, Func<ParamType, T>> updateHandlers;
        private ICollection<StateChangeHandler> stateChangeHandlers;


        public StateMachine(T initalState)
        {
            updateHandlers = new Dictionary<T, Func<ParamType, T>>();
            this.state = initalState;
            this.stateChangeHandlers = new List<StateChangeHandler>();
        }

        public void registerStateHandler(T state, Func<ParamType, T> handler)
        {
            updateHandlers[state] = handler;
        }

        public void registerStateTransitionHandler(T initialState, T endState, Action<ParamType> handler)
        {
            this.stateChangeHandlers.Add(new StateChangeHandler
            {
                fromState = initialState,
                toState = endState,
                handler = handler
            });
        }

        public void registerGenericHandler(GenericStateHandler<T, ParamType> genericHandler)
        {
            this.registerStateHandler(genericHandler.stateHandle, param => genericHandler.HandleState(param));
            this.registerStateTransitionHandler(
                genericHandler.validPreviousStates,
                genericHandler.stateHandle,
                param => genericHandler.TransitionIntoState(param));
            this.registerStateTransitionHandler(
                genericHandler.stateHandle,
                genericHandler.validNextStates,
                param => genericHandler.TransitionOutOfState(param));
        }

        public void update(ParamType updateParam)
        {
            var updateAction = updateHandlers[state];
            if (updateAction == null)
            {
                throw new NotImplementedException($"no state handler found for state {Enum.GetName(typeof(T), state)}");
            }
            var newState = updateAction(updateParam);

            if(!newState.Equals(state))
            {
                Debug.Log($"Transitioning from {Enum.GetName(typeof(T), state)} into {Enum.GetName(typeof(T), newState)}");
                foreach (var changeHandler in stateChangeHandlers)
                {
                    if (changeHandler.fromState.HasFlag(state) && changeHandler.toState.HasFlag(newState))
                    {
                        changeHandler.handler(updateParam);
                    }
                }
            }

            this.state = newState;
        }
    }
}
