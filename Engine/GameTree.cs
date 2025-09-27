namespace GameSolver.Engine;

public class GameTree<TGameState, TTraceData>
{
    public GameTree(IEqualityComparer<TGameState>? equalityComparer = null)
    {
        if (equalityComparer == null)
            equalityComparer = EqualityComparer<TGameState>.Default;
        Cache = new(equalityComparer);
    }

    public GameNode<TGameState, TTraceData> Root { get; init; } = null!;
    public HashSet<TGameState> Cache { get; }
    public PriorityQueue<GameNode<TGameState, TTraceData>, double> Leafs { get; } = new();
    public List<TTraceData> Trace { get; } = new();
}
