using GameSolver.Engine;
using GameSolver.EightPuzzle;
// var startField = new byte[3, 3]
// {
//     {2, 1, 6},
//     {4, 0, 8},
//     {7, 5, 3}
// };

// var finalField = new byte[3, 3]
// {
//     {1, 2, 3},
//     {8, 0, 4},
//     {7, 6, 5}
// };

var startField = new byte[3, 3]
{
    {2, 4, 3},
    {1, 8, 5},
    {7, 0, 6}
};

var startState = new EightPuzzleState
{
    Matrix = startField
};

var finalField = new byte[3, 3]
{
    {1, 2, 3},
    {4, 5, 6},
    {7, 8, 0}
};

var eightPuzzleRules = new EightPuzzleRules(finalField);
var comparer = new EightPuzzleStateEqualityComparer();
var gameTree = Solver.Solve(
    startState, eightPuzzleRules, comparer);
var finalState = gameTree.Leafs.Peek();
Console.WriteLine($"КПД = {(finalState.GameState.Depth + 1f) / gameTree.Cache.Count * 100}%");
Console.WriteLine(gameTree.Trace.Count);
foreach (var trace in gameTree.Trace)
{
    Console.WriteLine(trace.Params[0]);
    WriteMatrix(trace.GameState.Matrix);
}

void WriteTree(GameNode<EightPuzzleState> root)
{
	var currents = new List<GameNode<EightPuzzleState>>()
    {
	    gameTree.Root
    };
	while (currents.Count > 0)
	{
		Console.WriteLine($"Next Level");
		var nexts = new List<GameNode<EightPuzzleState>>();
		foreach (var current in currents)
		{
			Console.WriteLine(current.GameState.Depth);
			WriteMatrix(current.GameState.Matrix);
			nexts.AddRange(current.Childs);
		}
		currents = nexts;
	}
}

void WriteMatrix(byte[,] matrix)
{
    for (var i = 0; i < matrix.GetLength(0); ++i)
    {
        for (var j = 0; j < matrix.GetLength(1); ++j)
        {
            Console.Write(matrix[i, j]);
            Console.Write("\t");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}