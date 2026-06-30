namespace Core.Moves;

/// <summary>
/// Содержит все возможные направления движения фигур.
/// </summary>
public static class Directions
{
    /// <summary>Вверх (увеличение row).</summary>
    public static readonly (int DRow, int DCol) Up = (1, 0);
    /// <summary>Вниз (уменьшение row).</summary>
    public static readonly (int DRow, int DCol) Down = (-1, 0);
    /// <summary>Вправо (увеличение col).</summary>
    public static readonly (int DRow, int DCol) Right = (0, 1);
    /// <summary>Влево (уменьшение col).</summary>
    public static readonly (int DRow, int DCol) Left = (0, -1);
    /// <summary>Вверх-вправо.</summary>
    public static readonly (int DRow, int DCol) UpRight = (1, 1);
    /// <summary>Вверх-влево.</summary>
    public static readonly (int DRow, int DCol) UpLeft = (1, -1);
    /// <summary>Вниз-вправо.</summary>
    public static readonly (int DRow, int DCol) DownRight = (-1, 1);
    /// <summary>Вниз-влево.</summary>
    public static readonly (int DRow, int DCol) DownLeft = (-1, -1);

    /// <summary>Направления для ладьи (ратоборца).</summary>
    public static readonly (int DRow, int DCol)[] RookDirections = [Up, Down, Right, Left];
    /// <summary>Направления для слона (лучника).</summary>
    public static readonly (int DRow, int DCol)[] BishopDirections = [UpRight, UpLeft, DownRight, DownLeft];
    /// <summary>Направления для князя (ферзя) — ладья + слон.</summary>
    public static readonly (int DRow, int DCol)[] QueenDirections = [.. RookDirections, .. BishopDirections];
    /// <summary>Направления для всадника (коня) — все 8 клеток буквой Г.</summary>
    public static readonly (int DRow, int DCol)[] KnightDirections =
    [
        (2, 1), (2, -1), (-2, 1), (-2, -1),
        (1, 2), (1, -2), (-1, 2), (-1, -2)
    ];
}