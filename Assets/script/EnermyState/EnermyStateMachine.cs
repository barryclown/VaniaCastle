using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyStateMachine 
{
    public EnermyState currentState{ get; private set; }
    public void Initialize(EnermyState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }
    public void ChangeState(EnermyState _startState)
    {
        currentState.Exit();
        currentState = _startState;
        currentState.Enter();
    }
    
}
