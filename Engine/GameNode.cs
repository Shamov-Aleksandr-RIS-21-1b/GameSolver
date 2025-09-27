namespace GameSolver.Engine;

public class GameNode<TState, TTraceData>
{
    private List<GameNode<TState, TTraceData>> _childs = new(); 

    public GameNode<TState, TTraceData>? Parent { get; private set; } = null;
    public IReadOnlyList<GameNode<TState, TTraceData>> Childs => _childs;
    public TState GameState { get; init; } = default!;
    public TTraceData TraceData { get; init; } = default!;

    public void AddChild(GameNode<TState, TTraceData> child)
    {
        _childs.Add(child);
        child.Parent = this;
    }
}