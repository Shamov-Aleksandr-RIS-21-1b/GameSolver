namespace GameSolver.Engine;

public static class Solver
{
	public static GameTree<TState> Solve<TState>(TState initialState, IGameRules<TState> rules, IEqualityComparer<TState>? statesComparer = null)
	{
		var initialF = rules.EvalF(initialState, out var parameters);
		var gameTree = new GameTree<TState>(statesComparer)
		{
			Root = new GameNode<TState>
			{
				GameState = initialState,
				Params = parameters
			}
		};
		gameTree.Cache.Add(initialState);
		gameTree.Leafs.Enqueue(gameTree.Root, initialF);

		for (; ; )
		{
			var currentNode = gameTree.Leafs.Peek();
			gameTree.Trace.Add(currentNode);
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
				var childF = rules.EvalF(childState, out parameters);
				var childNode = new GameNode<TState>
				{
					GameState = childState,
					Params = parameters
				};
				currentNode.AddChild(childNode);

				gameTree.Leafs.Enqueue(childNode, childF);
			}
		}

		return gameTree;
	}
}
