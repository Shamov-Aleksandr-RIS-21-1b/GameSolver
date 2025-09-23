using System.Diagnostics.CodeAnalysis;

namespace GameSolver.EightPuzzle;
public class BarleyBreakStateEqualityComparer : IEqualityComparer<BarleyBreakState>
{
	public bool Equals(BarleyBreakState? x, BarleyBreakState? y)
	{
		return GameFieldComparer.Instance.Equals(x.Field, y.Field);
	}

	public int GetHashCode([DisallowNull] BarleyBreakState obj)
	{
		return GameFieldComparer.Instance.GetHashCode(obj.Field);
	}
}
