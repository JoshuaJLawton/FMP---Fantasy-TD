using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private State CurrentState;
    private State PreviousState;

    public void ChangeState(State NewState)
    {
        if (this.CurrentState != null)
        {
            this.CurrentState.Exit();
        }
        this.PreviousState = this.CurrentState;

        this.CurrentState = NewState;
        this.CurrentState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
    }

    public void SwitchToPreviousState()
    {
        this.CurrentState.Exit();
        this.CurrentState = this.PreviousState;
        this.CurrentState.Enter();
    }
}
