namespace GameSolver.Engine;

public interface IGameRules<TState>
{
    bool IsFinalState(TState currentState);
    double EvalF(TState currentState, out double[] parameters);
    IEnumerable<TState> GetChildStates(TState currentState);
}
