using GameSolver.EightPuzzle;
using GameSolver.Engine;

namespace Demonstration
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            var startField = new byte[3, 3]
{
    {2, 4, 3},
    {1, 8, 5},
    {7, 0, 6}
};

            var startState = new BarleyBreakState
            {
                Matrix = startField
            };

            var finalField = new byte[3, 3]
            {
    {1, 2, 3},
    {4, 5, 6},
    {7, 8, 0}
            };

            var eightPuzzleRules = new BarleyBreakRules(finalField);
            var comparer = new BarleyBreakStateEqualityComparer();
            var gameTree = Solver.Solve(
                startState, eightPuzzleRules, comparer);
            var finalState = gameTree.Leafs.Peek();
            Console.WriteLine($"ÊÏÄ = {(finalState.GameState.Depth + 1f) / gameTree.Cache.Count * 100}%");
        }
    }
}