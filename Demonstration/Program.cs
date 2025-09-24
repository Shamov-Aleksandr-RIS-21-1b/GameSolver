using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using GameSolver.EightPuzzle;
using GameSolver.Engine;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

internal class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

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

        /// FUCKING

        var mainForm = new Form();

        mainForm.SuspendLayout();
        mainForm.Controls.Add(new System.Windows.Forms.Label()
        {
            Text = $"КПД = {kpd}%\nГлубина = {finalNode.GameState.Depth + 1}\nРассмотренно ходов = {gameTree.Cache.Count}",
            AutoSize = true,
            Top = 20,
        });
        mainForm.ResumeLayout();

        mainForm.SuspendLayout();
        mainForm.Controls.Add(new GViewer
        {
            Graph = graph,
            Dock = DockStyle.Fill,
        });
        mainForm.ResumeLayout();

        /// PLOTTING

        var plotArea = new ChartArea();
        plotArea.AxisX.Minimum = 1;
        plotArea.AxisX.Maximum = gameTree.Trace.Count;
        plotArea.AxisX.Interval = 1;
        plotArea.AxisY.Maximum = 8;
        plotArea.AxisY.Interval = 1;
        
        var series = new Series("My Data");
        series.BorderWidth = 5;
        series.ChartType = SeriesChartType.Line;
        series.MarkerStyle = MarkerStyle.Circle;
        series.MarkerSize = 15;
        for (var i = 0; i < gameTree.Trace.Count; ++i)
        {
            var x = i + 1;
            var y = gameTree.Trace[i].Params[0]; // G
            series.Points.AddXY(x, y);
        }

        var plot = new Chart();
        plot.ChartAreas.Add(plotArea);
        plot.Series.Add(series);
        plot.Dock = DockStyle.Fill;

        var plotForm = new Form();
        plotForm.SuspendLayout();
        plotForm.Controls.Add(plot);
        plotForm.ResumeLayout();

        mainForm.Show();
        plotForm.Show();

        Application.Run();
    }
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
}