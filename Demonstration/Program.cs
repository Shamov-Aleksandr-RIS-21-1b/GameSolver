﻿using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using BarleyBreak;
using GameSolver.EightPuzzle;
using GameSolver.Engine;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

internal class Program
{
    private static readonly Dictionary<(int, int), string> DirectionMap = new()
    {
        [(-1, 0)] = "↑",
        [(+1, 0)] = "↓",
        [(0, -1)] = "←",
        [(0, +1)] = "→",
    };

    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var startState = new BarleyBreakState
        {
            Field = new byte[3, 3]
            {
                //{2, 4, 3},
                //{1, 8, 5},
                //{7, 0, 6}
                {2, 1, 6},
                {4, 0, 8},
                {7, 5, 3}
            }
        };

        var finalField = new byte[3, 3]
        {
            //{1, 2, 3},
            //{4, 5, 6},
            //{7, 8, 0}
            {1, 2, 3},
            {8, 0, 4},
            {7, 6, 5}
        };

        var barleyBreakRules = new BarleyBreakRules(finalField);
        var comparer = new BarleyBreakStateEqualityComparer();

        var gameTree = Solver.Solve(startState, barleyBreakRules, comparer);

        var finalNode = gameTree.Leafs.Peek();
        var kpd = (finalNode.GameState.Depth + 1f) / gameTree.Cache.Count * 100;

        var graph = new Graph("BarleyBreak");
        var currentDepthNodes = new List<GameNode<BarleyBreakState, BarleyBreakTraceData>> { gameTree.Root };
        while (currentDepthNodes.Count > 0)
        {
            var nextDepthAllChilds = new List<GameNode<BarleyBreakState, BarleyBreakTraceData>>(4);
            foreach (var current in currentDepthNodes)
            {
                var source = MatrixToString(current.GameState.Field);
                var sourceZeroPos = current.GameState.Field.GetPosition(0);
                foreach (var currentChild in current.Childs)
                {
                    var target = MatrixToString(currentChild.GameState.Field);
                    var targetZeroPos = currentChild.GameState.Field.GetPosition(0);
                    var direction = DirectionMap[(targetZeroPos.Row - sourceZeroPos.Row, targetZeroPos.Col - sourceZeroPos.Col)];
                    graph.AddEdge(source, direction, target);
                    nextDepthAllChilds.Add(currentChild);
                }
            }
            currentDepthNodes = nextDepthAllChilds;
        }

        var currentNode = finalNode;
        while (currentNode != null)
        {
            if (currentNode.Parent != null)
            {
                var nodeId = MatrixToString(currentNode.GameState.Field);
                var parentEdge = graph.FindNode(nodeId).InEdges.First();
                parentEdge.Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                parentEdge.Label.FontColor = Microsoft.Msagl.Drawing.Color.Green;
            }
            currentNode = currentNode.Parent;
        }

        /// FUCKING

        var mainForm = new Form();

        mainForm.Controls.Add(new System.Windows.Forms.Label()
        {
            Text = $"КПД = {kpd}%\nГлубина = {finalNode.GameState.Depth + 1}\nРассмотренно ходов = {gameTree.Cache.Count}",
            AutoSize = true,
            Top = 20,
        });

        mainForm.Controls.Add(new GViewer
        {
            Graph = graph,
            Dock = DockStyle.Fill,
        });

        /// PLOTTING

        var missplacedTilesTrace = new Series();
        missplacedTilesTrace.BorderWidth = 3;
        missplacedTilesTrace.ChartType = SeriesChartType.Line;
        missplacedTilesTrace.MarkerStyle = MarkerStyle.Circle;
        missplacedTilesTrace.MarkerSize = 5;
        for (var i = 0; i < gameTree.Trace.Count; ++i)
        {
            var x = i + 1;
            var y = gameTree.Trace[i].MissplacedTilesCount; // G
            missplacedTilesTrace.Points.AddXY(x, y);
        }

        var distanceTrace = new Series();
        distanceTrace.BorderWidth = 3;
        distanceTrace.ChartType = SeriesChartType.Line;
        distanceTrace.MarkerStyle = MarkerStyle.Circle;
        distanceTrace.MarkerSize = 5;
        distanceTrace.Color = System.Drawing.Color.Magenta;
        var maxDistance = 0;
        for (var i = 0; i < gameTree.Trace.Count; ++i)
        {
            var x = i + 1;
            var y = gameTree.Trace[i].Distance; // G
            distanceTrace.Points.AddXY(x, y);
            maxDistance = (int)Math.Max(maxDistance, y);
        }

        var plotArea = new ChartArea();
        plotArea.AxisX.Minimum = 1;
        plotArea.AxisX.Maximum = gameTree.Trace.Count;
        plotArea.AxisX.Interval = gameTree.Trace.Count > 100 ? 10 : 1;
        plotArea.AxisX.Title = "Номер хода";
        plotArea.AxisY.Maximum = maxDistance;
        plotArea.AxisY.Interval = 1;
        plotArea.AxisY.Title = "Ошибка";

        plotArea.CursorX.IsUserEnabled = true;
        plotArea.CursorX.IsUserSelectionEnabled = true;
        plotArea.CursorY.IsUserEnabled = true;
        plotArea.CursorY.IsUserSelectionEnabled = true;

        plotArea.AxisX.ScaleView.Zoomable = true;
        plotArea.AxisY.ScaleView.Zoomable = true;
        plotArea.AxisX.ScrollBar.Enabled = true;
        plotArea.AxisY.ScrollBar.Enabled = true;

        var plot = new Chart();
        plot.ChartAreas.Add(plotArea);
        plot.Series.Add(missplacedTilesTrace);
        plot.Series.Add(distanceTrace);
        plot.Dock = DockStyle.Fill;

        var plotForm = new Form();
        plotForm.Controls.Add(plot);

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