using System;
using System.Collections.Generic;

public class StateMachine<T> where T : Enum
{
    public StateBase CurrentState { get; private set; }

    private Dictionary<T, StateBase> _dictionaryState;

    public StateMachine()
    {
        _dictionaryState = new Dictionary<T, StateBase>();
    }

    public void RegisterState(T enumState, StateBase state)
    {
        _dictionaryState.Add(enumState, state);
    }

    public void SwitchState(T enumState, params object[] objs)
    {
        if (CurrentState != null) CurrentState.OnStateExit();

        CurrentState = _dictionaryState[enumState];
        CurrentState.OnStateEnter(objs);
    }
}
