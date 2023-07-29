using System;
using System.Collections.Generic;

public class StateMachine<T> where T : Enum
{
    private Dictionary<T, StateBase> _dictionaryState;
    private StateBase _currentState;

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
        if (_currentState != null) _currentState.OnStateExit();

        _currentState = _dictionaryState[enumState];
        _currentState.OnStateEnter(objs);
    }
}
