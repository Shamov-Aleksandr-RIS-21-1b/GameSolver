using GameSolver.Engine;

namespace GameSolver.EightPuzzle;

public class BarleyBreakRules : IGameRules<BarleyBreakState>
{
	private readonly byte[,] _finalMatrix;
	private readonly int _gameSize;

	public BarleyBreakRules(byte[,] finalState)
	{
		_finalMatrix = finalState;
		_gameSize = _finalMatrix.GetLength(0);
	}

	public IEnumerable<BarleyBreakState> GetChildStates(BarleyBreakState currentState)
	{
		var currentZeroPos = currentState.Matrix.GetZeroPos();
		var childZeroPoses = new List<Position>();

		// LEFT
		if (currentZeroPos.Col != 0)
		{
			var nextZeroPos = new Position
			{
				Row = currentZeroPos.Row,
				Col = currentZeroPos.Col - 1
			};
			childZeroPoses.Add(nextZeroPos);
		}

		// DOWN
		if (currentZeroPos.Row != _gameSize - 1)
		{
			var nextZeroPos = new Position
			{
				Row = currentZeroPos.Row + 1,
				Col = currentZeroPos.Col
			};
			childZeroPoses.Add(nextZeroPos);
		}

		// UP
		if (currentZeroPos.Row != 0)
		{
			var nextZeroPos = new Position
			{
				Row = currentZeroPos.Row - 1,
				Col = currentZeroPos.Col
			};
			childZeroPoses.Add(nextZeroPos);
		}

		// RIGHT
		if (currentZeroPos.Col != _gameSize - 1)
		{
			var nextZeroPos = new Position
			{
				Row = currentZeroPos.Row,
				Col = currentZeroPos.Col + 1
			};
			childZeroPoses.Add(nextZeroPos);
		}

		return childZeroPoses.Select(childZeroPos => new BarleyBreakState
		{
			Matrix = currentState.Matrix.Swap(currentZeroPos, childZeroPos),
			Depth = currentState.Depth + 1
		});
	}

	public bool IsFinalState(BarleyBreakState currentState) =>
		GameFieldComparer.Instance.Equals(currentState.Matrix, _finalMatrix);

	public double EvalF(BarleyBreakState currentState, out double[] parameters)
	{
		var g = GetMissplasedTilesCount(currentState.Matrix, _finalMatrix);
		var h = currentState.Depth;
		var f = g + h;

		parameters = [g, h];

		return f;
	}

	private byte GetMissplasedTilesCount(byte[,] currentState, byte[,] finalState)
	{
		byte missplasedTilesCount = 0;

		for (var i = 0; i < _gameSize; ++i)
		{
			for (var j = 0; j < _gameSize; ++j)
			{
				if (currentState[i, j] != finalState[i, j])
					++missplasedTilesCount;
			}
		}

		return missplasedTilesCount;
	}
}
