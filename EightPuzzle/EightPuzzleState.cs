namespace GameSolver.EightPuzzle;

public class EightPuzzleState
{
    public byte[,] Matrix { get; init; } = null!;

    public int Depth { get; init; } = 0;
}