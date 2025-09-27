using BarleyBreak;
using GameSolver.Engine;

namespace GameSolver.EightPuzzle;

public class BarleyBreakRules : IGameRules<BarleyBreakState, BarleyBreakTraceData>
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
		var currentZeroPos = currentState.Field.GetPosition(0);
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
			Field = currentState.Field.Swap(currentZeroPos, childZeroPos),
			Depth = currentState.Depth + 1
		});
	}

	public bool IsFinalState(BarleyBreakState currentState) =>
		GameFieldComparer.Instance.Equals(currentState.Field, _finalMatrix);

	public double EvalF(BarleyBreakState currentState, out BarleyBreakTraceData parameters)
	{
		var g = GetDistance(currentState.Field);
        var h = currentState.Depth;
        var f = g + 0.35 * h;

        parameters = new BarleyBreakTraceData
		{
            MissplacedTilesCount = GetMissplasedTilesCount(currentState.Field),
			Distance = g,
        };

		return f;
	}

	private byte GetMissplasedTilesCount(byte[,] currentState)
	{
		byte missplasedTilesCount = 0;

		for (var i = 0; i < _gameSize; ++i)
		{
			for (var j = 0; j < _gameSize; ++j)
			{
				if (currentState[i, j] != _finalMatrix[i, j])
					++missplasedTilesCount;
			}
		}

		return missplasedTilesCount;
	}
    private byte GetDistance(byte[,] currentState)
    {
        byte distance = 0;
        for (var i = 0; i < _gameSize; ++i)
        {
            for (var j = 0; j < _gameSize; ++j)
            {
                var toFind = _finalMatrix[i, j];
                var currentPosition = currentState.GetPosition(toFind);
                distance += (byte)(Math.Abs(currentPosition.Row - i) + Math.Abs(currentPosition.Col - j));
            }
        }
        return distance;
    }
}
