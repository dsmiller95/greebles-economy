using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Utilities
{
    /// <summary>
    /// A state machine which supports asynchronous operations
    ///     If a state handler returns a long-running task, all other attempts to update the state will be blocked
    ///     until the long running task completes
    /// </summary>
    /// <typeparam name="T">The enum type which represents the state options</typeparam>
    /// <typeparam name="ParamType">The type of object which all state handlers will pull their data from</typeparam>
    public class AsyncStateMachine<T, ParamType> where T : Enum
    {
        public struct StateChangeHandler
        {
            public T fromState;
            public T toState;
            public Action<ParamType> handler;
        }

        private T state;

        private Dictionary<T, Func<ParamType, Task<T>>> updateHandlers;
        private IList<StateChangeHandler> stateChangeHandlers;


        public AsyncStateMachine(T initalState)
        {
            updateHandlers = new Dictionary<T, Func<ParamType, Task<T>>>();
            this.state = initalState;
            this.stateChangeHandlers = new List<StateChangeHandler>();
        }

        public void registerStateHandler(T state, Func<ParamType, Task<T>> handler)
        {
            updateHandlers[state] = handler;
        }

        /// <summary>
        /// Registers a handler to be called when state changes. The first handler added that matches
        ///     will be the first handler to be called, but will not block any other handlers
        /// </summary>
        /// <param name="initialState">The possible initial state, or states if the enum is flags</param>
        /// <param name="endState">the possible end state, or states if the enum is flags</param>
        /// <param name="handler">the handler to execute</param>
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

        private bool stateLocked = false;

        /// <summary>
        /// Attempt to step the state machine. Returns true if the machine has stepped, false if the machine is in the middle of an
        ///     asyncronous step and it cannot advance
        /// This method can be fired off and ignored safely, as long as the caller does not care about when the effects are executed
        /// </summary>
        /// <param name="updateParam">The data</param>
        /// <returns>true if the state machine executed a step or attempted to execute a step</returns>
        public async Task<bool> update(ParamType updateParam)
        {
            lock (this)
            {
                if (stateLocked)
                {
                    return false;
                }
                this.stateLocked = true;
            }

            Func<ParamType, Task<T>> updateAction;
            if (!updateHandlers.TryGetValue(state, out updateAction))
            {
                throw new NotImplementedException($"no state handler found for state {Enum.GetName(typeof(T), state)}");
            }
            var newState = await updateAction(updateParam);

            if(!newState.Equals(state))
            {
                //Debug.Log($"Transitioning from {Enum.GetName(typeof(T), state)} into {Enum.GetName(typeof(T), newState)}");
                foreach (var changeHandler in stateChangeHandlers)
                {
                    if (changeHandler.fromState.HasFlag(state) && changeHandler.toState.HasFlag(newState))
                    {
                        changeHandler.handler(updateParam);
                    }
                }
            }

            lock (this)
            {
                this.stateLocked = false;
            }

            this.state = newState;
            return true;
        }
    }
}
