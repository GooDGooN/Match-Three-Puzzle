using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateManager<T>
{
    private Dictionary<Type, BaseFSM> stateContainer = new();
    private T myTarget;
    public BaseFSM CurrentState;

    public StateManager(T target)
    {
        myTarget = target;
    }

    public void ChangeState<ST>() where ST : BaseFSM, new()
    {
        CurrentState?.StateExit();
        if (!stateContainer.TryGetValue(typeof(ST), out var state))
        {
            state = new ST();
            stateContainer.Add(typeof(ST), state);
            state.self = myTarget;
        }
        CurrentState.StateEnter();
    }


    public class BaseFSM
    {
        public T self;
        public void StateEnter() { }
        public void StateExit() { }
        public void StateFixedUpdate() { }
        public void StateUpdate() { }

    }
}
