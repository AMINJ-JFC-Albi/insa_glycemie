using System;

public interface IStateMachine
{
    Enum CurrentState { get; }

    void Update();
    void ChangeState(Enum newState);
    Enum IncrementState();
}
