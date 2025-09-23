using GameSolver.Engine;

namespace GameSolver.EightPuzzle;

public class EightPuzzleRules : IGameRules<EightPuzzleState>
{
	private readonly byte[,] _finalMatrix;
	private readonly int _gameSize;

	public EightPuzzleRules(byte[,] finalState)
	{
		_finalMatrix = finalState;
		_gameSize = _finalMatrix.GetLength(0);
	}

	public IEnumerable<EightPuzzleState> GetChildStates(EightPuzzleState currentState)
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

		return childZeroPoses.Select(childZeroPos => new EightPuzzleState
		{
			Matrix = currentState.Matrix.Swap(currentZeroPos, childZeroPos),
			Depth = currentState.Depth + 1
		});
	}

	public bool IsFinalState(EightPuzzleState currentState) =>
		GameFieldComparer.Instance.Equals(currentState.Matrix, _finalMatrix);

	public double EvalF(EightPuzzleState currentState, out double[] parameters)
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
