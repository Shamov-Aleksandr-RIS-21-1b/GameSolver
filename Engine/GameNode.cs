namespace GameSolver.Engine;

public class GameNode<TState>
{
    private List<GameNode<TState>> _childs = new(); 

    public GameNode<TState>? Parent { get; private set; } = null;
    public IReadOnlyList<GameNode<TState>> Childs => _childs;
    public TState GameState { get; init; } = default!;
    public double[] Params { get; init; } = null!;

    public void AddChild(GameNode<TState> child)
    {
        _childs.Add(child);
        child.Parent = this;
    }
}