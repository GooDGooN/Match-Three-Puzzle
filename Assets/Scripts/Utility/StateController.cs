using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateController<T>
{
    private Dictionary<Type, BaseFSM<T>> stateContainer = new();
    private T myTarget;
    public BaseFSM<T> CurrentState;

    public StateController(T target)
    {
        myTarget = target;
    }

    public void ChangeState<ST>() where ST : BaseFSM<T>, new()
    {
        CurrentState?.StateExit();
        if (!stateContainer.TryGetValue(typeof(ST), out var state))
        {
            state = new ST();
            stateContainer.Add(typeof(ST), state);
            state.self = myTarget;
            state.stateManager = this;
        }
        CurrentState = state;
        Debug.Log(CurrentState?.ToString()); // !!TEST!!
        CurrentState.StateEnter();
    }
}

public class BaseFSM<T>
{
    public T self;
    public StateController<T> stateManager;
    public virtual void StateEnter() { }
    public virtual void StateExit() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate() { }
}
