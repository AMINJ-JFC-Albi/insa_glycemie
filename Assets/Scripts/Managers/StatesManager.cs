using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tools;

namespace States
{
    public class StateMachine<TState> : IStateMachine where TState : Enum
    {
        private Dictionary<TState, Action> stateActions;
        private Dictionary<(TState, TState), Action> transitionActions;
        private Dictionary<TState, IStateMachine> subStateMachines;
        private Dictionary<TState, Type> subStateTypes;

        public TState CurrentState { get; private set; }

        public string CurrentStringState() => CurrentState.ToString();

        Enum IStateMachine.CurrentState { get => (Enum)CurrentState; }

        public StateMachine(
            TState initialState,
            Dictionary<TState, Action> stateActions,
            Dictionary<(TState, TState), Action> transitionActions = null,
            Dictionary<TState, IStateMachine> subStateMachines = null,
            Dictionary<TState, Type> subStateTypes = null,
            bool onStateExecute = true)
        {
            this.stateActions = stateActions ?? new Dictionary<TState, Action>();
            this.transitionActions = transitionActions ?? new Dictionary<(TState, TState), Action>();
            this.subStateMachines = subStateMachines ?? new Dictionary<TState, IStateMachine>();
            this.subStateTypes = subStateTypes ?? new Dictionary<TState, Type>();

            CurrentState = initialState;
            if (stateActions.TryGetValue(CurrentState, out var onState) && onStateExecute)
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
            TState oldState = CurrentState;
            if (CurrentState.Equals(newState)) return;

            var transition = (CurrentState, newState);
            if (transitionActions.TryGetValue(transition, out var onTransition))
                onTransition?.Invoke();

            CurrentState = newState;
        }

        public void Update()
        {
            if (stateActions.TryGetValue(CurrentState, out var onState))
                onState?.Invoke();
            if (subStateMachines.TryGetValue(CurrentState, out var subStateMachine))
            {
                subStateMachine.Update();
            }
        }

        public Enum IncrementState()
        {
            int currentIndex = Convert.ToInt32(CurrentState);
            int maxIndex = Enum.GetValues(typeof(TState)).Length - 1;

            if (currentIndex < maxIndex)
            {
                TState newState = (TState)Enum.ToObject(typeof(TState), currentIndex + 1);

                var transition = (CurrentState, newState);
                if (transitionActions.TryGetValue(transition, out var onTransition))
                    onTransition?.Invoke();

                CurrentState = newState;
            }
            else
            {
                stateActions[CurrentState]?.Invoke();
            }

            return CurrentState;
        }

        public Enum GetCurrentSubState()
        {
            if (subStateMachines.TryGetValue(CurrentState, out var subStateMachine))
            {
                return subStateMachine.CurrentState;
            }

            return null;
        }

        public bool IsLastSubState()
        {
            if (subStateMachines.TryGetValue(CurrentState, out var subStateMachine))
            {
                int currentIndex = Convert.ToInt32(subStateMachine.CurrentState);

                // Récupère le type de la sous-machine et calcule l'index maximum
                if (subStateTypes.TryGetValue(CurrentState, out var subStateType))
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
            if (subStateMachines.TryGetValue(CurrentState, out var subStateMachine))
            {
                subStateMachine.IncrementState();
            }
        }
    }
}
