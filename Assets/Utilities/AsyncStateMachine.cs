using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Utilities
{


    public enum StateChangeExecutionOrder
    {
        StateExit = 0,
        StateEntry = 10
    }
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
            public StateChangeExecutionOrder executionOrder;
        }

        private T state;

        private Dictionary<T, Func<ParamType, Task<T>>> updateHandlers;
        private IEnumerable<StateChangeHandler> stateChangeHandlers;


        public AsyncStateMachine(T initalState)
        {
            updateHandlers = new Dictionary<T, Func<ParamType, Task<T>>>();
            state = initalState;
            stateChangeHandlers = new List<StateChangeHandler>();
        }

        public void registerStateHandler(T state, Func<ParamType, Task<T>> handler)
        {
            if (updateHandlers.ContainsKey(state))
            {
                Debug.LogWarning($"Warning: {Enum.GetName(typeof(T), state)} already has a declared state handler. overwriting");
            }
            updateHandlers[state] = handler;
        }

        /// <summary>
        /// Registers a handler to be called when state changes. The first handler added that matches
        ///     will be the first handler to be called, but will not block any other handlers
        /// </summary>
        /// <param name="initialState">The possible initial state, or states if the enum is flags</param>
        /// <param name="endState">the possible end state, or states if the enum is flags</param>
        /// <param name="handler">the handler to execute</param>
        /// <param name="isStateEntry">Whether or not this represents an entry into a state, or an exit from a state.
        ///     All state handlers which represent a state Exit will run before any which represent a state Entry</param>
        public void registerStateTransitionHandler(T initialState, T endState, Action<ParamType> handler, StateChangeExecutionOrder executionOrder)
        {
            var newHandler = new StateChangeHandler
            {
                fromState = initialState,
                toState = endState,
                handler = handler,
                executionOrder = executionOrder
            };
            stateChangeHandlers = stateChangeHandlers.Append(newHandler);
        }

        private bool buildingState = true;

        public void LockStateHandlers()
        {
            this.buildingState = false;
            this.stateChangeHandlers = this.stateChangeHandlers
                .OrderBy(handler => handler.executionOrder)
                .ToList();
        }

        public void registerGenericHandler(GenericStateHandler<T, ParamType> genericHandler)
        {
            if (!this.buildingState)
            {
                throw new Exception("Error: not in state handler building mode");
            }
            registerStateHandler(genericHandler.stateHandle, param => genericHandler.HandleState(param));
            registerStateTransitionHandler(
                genericHandler.validPreviousStates,
                genericHandler.stateHandle,
                param => genericHandler.TransitionIntoState(param), StateChangeExecutionOrder.StateEntry);
            registerStateTransitionHandler(
                genericHandler.stateHandle,
                genericHandler.validNextStates,
                param => genericHandler.TransitionOutOfState(param), StateChangeExecutionOrder.StateExit);
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
            if (this.buildingState)
            {
                throw new Exception("Error: Must lock modifications to the state handlers before running the state machine");
            }
            lock (this)
            {
                if (stateLocked)
                {
                    return false;
                }
                stateLocked = true;
            }

            Func<ParamType, Task<T>> updateAction;
            if (!updateHandlers.TryGetValue(state, out updateAction))
            {
                throw new NotImplementedException($"no state handler found for state {Enum.GetName(typeof(T), state)}");
            }
            var newState = await updateAction(updateParam);

            if (!newState.Equals(state))
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
                stateLocked = false;
            }

            state = newState;
            return true;
        }
    }
}
