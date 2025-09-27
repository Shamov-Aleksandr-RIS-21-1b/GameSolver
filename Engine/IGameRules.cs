namespace GameSolver.Engine;

public interface IGameRules<TState, TTraceData>
{
    bool IsFinalState(TState currentState);
    double EvalF(TState currentState, out TTraceData traceData);
    IEnumerable<TState> GetChildStates(TState currentState);
}
