public interface IState
{
    void Enter(IState previousSate);

    void Execute();

    void Exit();
}
