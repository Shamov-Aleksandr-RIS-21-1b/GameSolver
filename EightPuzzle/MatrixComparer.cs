using System.Diagnostics.CodeAnalysis;

namespace GameSolver.EightPuzzle;

public class GameFieldComparer : IEqualityComparer<byte[,]>
{
    public static readonly GameFieldComparer Instance = new();

    public bool Equals(byte[,]? x, byte[,]? y)
    {
        for (var i = 0; i < x.GetLength(0); ++i)
        {
            for (var j = 0; j < x.GetLength(1); ++j)
            {
                if (x[i, j] != y[i, j])
                    return false;
            }
        }
        return true;
    }

    public int GetHashCode([DisallowNull] byte[,] obj)
    {
        int hash = 0;
        for (var i = 0; i < obj.GetLength(0); ++i)
        {
            for (var j = 0; j < obj.GetLength(1); ++j)
            {
                hash += obj[i, j] * (obj.GetLength(0) * i + j + 1);
            }
        }
        return hash;
    }
}
