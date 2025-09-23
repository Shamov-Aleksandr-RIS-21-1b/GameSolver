namespace GameSolver.EightPuzzle;

public class BarleyBreakState
{
    public byte[,] Field { get; init; } = null!;

    public int Depth { get; internal init; } = 0;
}