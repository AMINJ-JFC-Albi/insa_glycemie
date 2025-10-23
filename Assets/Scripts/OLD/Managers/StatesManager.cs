using System;
using System.Collections.Generic;

namespace States {
    public class StateMachine<TState> : IStateMachine where TState : Enum {
        private readonly Dictionary<TState, Action> stateActions;
        private readonly Dictionary<(TState, TState), Action> transitionActions;
        private readonly Dictionary<TState, IStateMachine> subStateMachines;
        private readonly Dictionary<TState, Type> subStateTypes;

        private TState currentState { get; set; }

        public string CurrentStringState() => currentState.ToString();

        Enum IStateMachine.CurrentState => (Enum)currentState;

        public StateMachine(
            TState initialState,
            Dictionary<TState, Action> stateActions,
            Dictionary<(TState, TState), Action> transitionActions = null,
            Dictionary<TState, IStateMachine> subStateMachines = null,
            Dictionary<TState, Type> subStateTypes = null,
            bool onStateExecute = true) {
            this.stateActions = stateActions ?? new Dictionary<TState, Action>();
            this.transitionActions = transitionActions ?? new Dictionary<(TState, TState), Action>();
            this.subStateMachines = subStateMachines ?? new Dictionary<TState, IStateMachine>();
            this.subStateTypes = subStateTypes ?? new Dictionary<TState, Type>();

            currentState = initialState;
            if (stateActions != null && stateActions.TryGetValue(currentState, out var onState) && onStateExecute)
                onState?.Invoke();
        }

        public void ChangeState(Enum newState) {
            if (newState is TState typedState) {
                ChangeState(typedState);
            }
        }

        private void ChangeState(TState newState) {
            if (currentState.Equals(newState)) return;

            (TState CurrentState, TState newState) transition = (currentState, newState);
            if (transitionActions.TryGetValue(transition, out var onTransition))
                onTransition?.Invoke();

            currentState = newState;
        }

        public void Update() {
            if (stateActions.TryGetValue(currentState, out var onState))
                onState?.Invoke();
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine)) {
                subStateMachine.Update();
            }
        }

        public Enum IncrementState() {
            int currentIndex = Convert.ToInt32(currentState);
            int maxIndex = Enum.GetValues(typeof(TState)).Length - 1;

            if (currentIndex < maxIndex) {
                TState newState = (TState)Enum.ToObject(typeof(TState), currentIndex + 1);

                (TState CurrentState, TState newState) transition = (currentState, newState);
                if (transitionActions.TryGetValue(transition, out var onTransition))
                    onTransition?.Invoke();

                currentState = newState;
            } else {
                stateActions[currentState]?.Invoke();
            }

            return currentState;
        }

        public Enum GetCurrentSubState() {
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine)) {
                return subStateMachine.CurrentState;
            }

            return null;
        }

        public bool IsLastSubState() {
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine)) {
                int currentIndex = Convert.ToInt32(subStateMachine.CurrentState);

                // Récupère le type de la sous-machine et calcule l'index maximum
                if (subStateTypes.TryGetValue(currentState, out var subStateType)) {
                    int maxIndex = Enum.GetValues(subStateType).Length - 1;
                    return currentIndex >= maxIndex;
                }
            }

            // Cas où il n'y a pas de sous-état ou si la sous-machine n'a pas d'état suivant
            return true;
        }


        public void IncrementSubState() {
            if (subStateMachines.TryGetValue(currentState, out var subStateMachine)) {
                subStateMachine.IncrementState();
            }
        }
    }
}
