namespace GameSolver.EightPuzzle;

public static class MatrixExtensions
{
    public static Position GetPosition(this byte[,] matrix, byte toFind)
    {
        for (var i = 0; i < matrix.GetLength(0); ++i)
        {
            for (var j = 0; j < matrix.GetLength(1); ++j)
            {
                if (matrix[i, j] == toFind)
                    return new Position
                    {
                        Row = i,
                        Col = j
                    };
            }
        }

        return new Position
        {
            Row = -1,
            Col = -1
        };
    }

    public static byte[,] Swap(this byte[,] matrix, Position pos1, Position pos2)
    {
        var result = matrix.CopyMatrix();

        (result[pos1.Row, pos1.Col], result[pos2.Row, pos2.Col]) =
        (result[pos2.Row, pos2.Col], result[pos1.Row, pos1.Col]);

        return result;
    }

    private static byte[,] CopyMatrix(this byte[,] matrix)
    {
        var copy = new byte[matrix.GetLength(0), matrix.GetLength(1)];
        for (var i = 0; i < matrix.GetLength(0); ++i)
        {
            for (var j = 0; j < matrix.GetLength(1); ++j)
            {
                copy[i, j] = matrix[i, j];
            }
        }
        return copy;
    }
}