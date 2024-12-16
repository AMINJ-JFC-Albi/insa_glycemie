using System;
using System.Collections.Generic;
using Tools;

namespace States
{
    public class StateMachine<TState> : IStateMachine where TState : Enum
    {
        private TState currentState;
        private Dictionary<TState, Action> stateActions;
        private Dictionary<(TState, TState), Action> transitionActions;
        private Dictionary<TState, IStateMachine> subStateMachines;
        private Dictionary<TState, Type> subStateTypes;

        public TState CurrentState { get; private set; }
        Enum IStateMachine.CurrentState { get => (Enum)currentState; }

        public StateMachine(
            TState initialState,
            Dictionary<TState, Action> stateActions,
            Dictionary<(TState, TState), Action> transitionActions = null,
            Dictionary<TState, IStateMachine> subStateMachines = null,
            Dictionary<TState, Type> subStateTypes = null)
        {
            this.stateActions = stateActions ?? new Dictionary<TState, Action>();
            this.transitionActions = transitionActions ?? new Dictionary<(TState, TState), Action>();
            this.subStateMachines = subStateMachines ?? new Dictionary<TState, IStateMachine>();
            this.subStateTypes = subStateTypes ?? new Dictionary<TState, Type>();

            currentState = initialState;
            if (stateActions.TryGetValue(currentState, out var onState))
                onState?.Invoke();
        }

        public void ChangeState(Enum newState)
        {
            if (newState is TState typedState)
            {
                ChangeState(typedState);
            }
        }

        public void ChangeState(TState newState)
        {
            TState oldState = currentState;
            if (currentState.Equals(newState)) return;

            var transition = (currentState, newState);
            if (transitionActions.TryGetValue(transition, out var onTransition))
                onTransition?.Invoke();

            currentState = newState;
        }

        public void Update()
        {
            if (stateActions.TryGetValue(currentState, out var onState))
                onState?.Invoke();
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine))
            {
                subStateMachine.Update();
            }
        }

        public void IncrementState()
        {
            int currentIndex = Convert.ToInt32(currentState);
            int maxIndex = Enum.GetValues(typeof(TState)).Length - 1;

            if (currentIndex < maxIndex)
            {
                TState newState = (TState)Enum.ToObject(typeof(TState), currentIndex + 1);

                var transition = (currentState, newState);
                if (transitionActions.TryGetValue(transition, out var onTransition))
                    onTransition?.Invoke();

                currentState = newState;
            }
            else
            {
                stateActions[currentState]?.Invoke();
            }
        }

        public bool IsLastSubState()
        {
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine))
            {
                var subStateCurrent = subStateMachine.CurrentState;
                int currentIndex = Convert.ToInt32(subStateMachine.CurrentState);

                // Récupère le type de la sous-machine et calcule l'index maximum
                if (subStateTypes.TryGetValue(currentState, out var subStateType))
                {
                    int maxIndex = Enum.GetValues(subStateType).Length - 1;
                    return currentIndex >= maxIndex;
                }
            }

            // Cas où il n'y a pas de sous-état ou si la sous-machine n'a pas d'état suivant
            return true;
        }


        public void IncrementSubState()
        {
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine))
            {
                subStateMachine.IncrementState();
            }
        }
    }
}
