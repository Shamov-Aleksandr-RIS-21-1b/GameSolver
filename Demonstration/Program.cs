using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using GameSolver.EightPuzzle;
using GameSolver.Engine;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

Dictionary<(int, int), string> DirectionMap = new()
{
    [(-1, 0)] = "вверх",
    [(+1, 0)] = "вниз",
    [(0, -1)] = "влево",
    [(0, +1)] = "вправо",
};

var startState = new BarleyBreakState
{
    Field = new byte[3, 3]
    {
        {2, 4, 3},
        {1, 8, 5},
        {7, 0, 6}
    }
};

var finalField = new byte[3, 3]
{
    {1, 2, 3},
    {4, 5, 6},
    {7, 8, 0}
};

var eightPuzzleRules = new BarleyBreakRules(finalField);

var comparer = new BarleyBreakStateEqualityComparer();
var gameTree = Solver.Solve(startState, eightPuzzleRules, comparer);
var finalNode = gameTree.Leafs.Peek();
var kpd = (finalNode.GameState.Depth + 1f) / gameTree.Cache.Count * 100;

var graph = new Graph("BarleyBreak");
var currentNodes = new List<GameNode<BarleyBreakState>> { gameTree.Root };
while (currentNodes.Count > 0)
{
    var nextLevelAllChilds = new List<GameNode<BarleyBreakState>>(4);
    foreach (var current in currentNodes)
    {
        var source = MatrixToString(current.GameState.Field);
        var sourceZeroPos = current.GameState.Field.GetZeroPos();
        foreach (var currentChild in current.Childs)
        {
            var target = MatrixToString(currentChild.GameState.Field);
            var targetZeroPos = currentChild.GameState.Field.GetZeroPos();
            var direction = DirectionMap[(targetZeroPos.Row - sourceZeroPos.Row, targetZeroPos.Col - sourceZeroPos.Col)];
            graph.AddEdge(source, direction, target);
            nextLevelAllChilds.Add(currentChild);
        }
    }
    currentNodes = nextLevelAllChilds;
}

var currentNode = finalNode;
while (currentNode != null)
{
    if (currentNode.Parent != null)
    {
        var nodeId = MatrixToString(currentNode.GameState.Field);
        var parentEdge = graph.FindNode(nodeId).InEdges.First();
        parentEdge.Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
    }
    currentNode = currentNode.Parent;
}

var form = new Form();

form.SuspendLayout();
form.Controls.Add(new System.Windows.Forms.Label()
{
    Text = $"КПД = {kpd}%",
    AutoSize = true,
    Top = 20,
});
form.ResumeLayout();

form.SuspendLayout();
form.Controls.Add(new GViewer
{
    Graph = graph,
    Dock = DockStyle.Fill,
});
form.ResumeLayout();

form.SuspendLayout();
form.Controls.Add(new Chart());
form.ResumeLayout();

form.ShowDialog();

static string MatrixToString(byte[,] matrix)
{
    var sb = new StringBuilder((matrix.GetLength(0) + 1) * matrix.GetLength(1));
    for (var i = 0; i < matrix.GetLength(0); ++i)
    {
        for (var j = 0; j < matrix.GetLength(1) - 1; ++j)
        {
            sb.Append(matrix[i, j]);
            sb.Append(" | ");
        }
        sb.Append(matrix[i, matrix.GetLength(1) - 1]);
        sb.AppendLine("  ");
    }

    return sb.ToString();
}