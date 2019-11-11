using UnityEngine;

public class StateMachine
{
    private IState currentState = null;
    public IState CurrentState { get => currentState; }
    private IState previousState = null;


    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        previousState = currentState;
        currentState = newState;

        currentState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
}