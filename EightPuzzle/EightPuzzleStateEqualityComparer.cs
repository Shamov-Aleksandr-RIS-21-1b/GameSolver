using System.Diagnostics.CodeAnalysis;

namespace GameSolver.EightPuzzle;
public class EightPuzzleStateEqualityComparer : IEqualityComparer<EightPuzzleState>
{
	public bool Equals(EightPuzzleState? x, EightPuzzleState? y)
	{
		return GameFieldComparer.Instance.Equals(x.Matrix, y.Matrix);
	}

	public int GetHashCode([DisallowNull] EightPuzzleState obj)
	{
		return GameFieldComparer.Instance.GetHashCode(obj.Matrix);
	}
}
