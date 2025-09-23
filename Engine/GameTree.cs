namespace GameSolver.Engine;

public class GameTree<TGameState>
{
    public GameTree(IEqualityComparer<TGameState>? equalityComparer = null)
    {
        if (equalityComparer == null)
            equalityComparer = EqualityComparer<TGameState>.Default;
        Cache = new(equalityComparer);
    }

    public GameNode<TGameState> Root { get; init; } = null!;
    public HashSet<TGameState> Cache { get; }
    public PriorityQueue<GameNode<TGameState>, double> Leafs { get; } = new();
    public List<GameNode<TGameState>> Trace { get; } = new();
}
