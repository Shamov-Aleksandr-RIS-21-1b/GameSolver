namespace GameSolver.Engine;

public static class Solver
{
	public static GameTree<TState, TTraceData> Solve<TState, TTraceData>(TState initialState, IGameRules<TState, TTraceData> rules, IEqualityComparer<TState>? statesComparer = null)
	{
		var initialF = rules.EvalF(initialState, out var traceData);
		var gameTree = new GameTree<TState, TTraceData>(statesComparer)
		{
			Root = new GameNode<TState, TTraceData>
			{
				GameState = initialState,
				TraceData = traceData
            }
		};
		gameTree.Cache.Add(initialState);
		gameTree.Leafs.Enqueue(gameTree.Root, initialF);

		for (; ; )
		{
			var currentNode = gameTree.Leafs.Peek();
			gameTree.Trace.Add(currentNode.TraceData);
			var currentState = currentNode.GameState;
			if (rules.IsFinalState(currentState))
				break;
			gameTree.Leafs.Dequeue();
			var childStates = rules
				.GetChildStates(currentState)
				.Where(childState => !gameTree.Cache.Contains(childState));
			foreach (var childState in childStates)
			{
				gameTree.Cache.Add(childState);
				var childF = rules.EvalF(childState, out traceData);
				var childNode = new GameNode<TState, TTraceData>
				{
					GameState = childState,
					TraceData = traceData
                };
				currentNode.AddChild(childNode);

				gameTree.Leafs.Enqueue(childNode, childF);
			}
		}

		return gameTree;
	}
}
