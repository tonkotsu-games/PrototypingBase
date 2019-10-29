public class StateMachine
{
    private IState currentState = null;
    private IState previousState = null;

    public void CangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        previousState = currentState;
        currentState = newState;
    }

    public void ExecuteStateUpdate()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
}