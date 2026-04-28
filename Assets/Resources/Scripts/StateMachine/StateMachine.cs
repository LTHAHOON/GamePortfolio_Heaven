using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine<TEnumState, TOwner>
{
    private readonly Dictionary<TEnumState, State<TEnumState, TOwner>> _dicState = new();
    private State<TEnumState, TOwner> _currentState;
    public State<TEnumState, TOwner> CurrentState => _currentState;
    public StateMachineOwner<TOwner> _stateMachineOwner = new();

    public StateMachine(TOwner owner, IStateData[] stateData)
    {
        _stateMachineOwner.Owner = owner;
        for (int i = 0; i < stateData.Length; i++)
        {
            _stateMachineOwner.AddStateData(stateData[i]);
        }
    }

    public void AddState(State<TEnumState, TOwner> state)
    {
        if (_stateMachineOwner == null) return;
        if(!_dicState.ContainsValue(state))
        {
            _dicState.Add(state.EState, state);
            state.InitState(this);
        }
    }

    public void ChangeState(TEnumState eState)
    {
        if(_dicState.TryGetValue(eState, out State<TEnumState, TOwner> state))
        {
            _currentState?.ExitState(this);
            _currentState = state;
            if (_currentState._bOnceEnter) return;
            _currentState?.EnterState(this);
        }
    }

    public void UpdateCurrentState()
    {
        _currentState?.UpdateState(this);
    }
    public bool TryGetStateData<T>(out T result)
    {
        if (_stateMachineOwner._ownerStateData.TryGetValue(typeof(T), out var data) && data is IStateData<T> stateData)
        {
            result = stateData.GetData();
            return true;
        }

        result = default;
        return false;
    }

    public TOwner GetOwner() => _stateMachineOwner.Owner;
}

public class StateMachineOwner<TOwner>
{
    public TOwner Owner { get; set; }
    public Dictionary<Type, IStateData> _ownerStateData = new();

    public void AddStateData(IStateData stateData)
    {
        _ownerStateData.Add(stateData.GetType(), stateData);
    }
}

