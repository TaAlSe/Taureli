using Core.Board;

namespace Core.Moves;

public static class RookMoveGenerator
{
    private static readonly (int dRow, int dCol)[] Directions =
    [
        (1, 0),   // вверх (увеличение row — от белых к чёрным)
        (-1, 0),  // вниз
        (0, 1),   // вправо
        (0, -1)   // влево
    ];

    /// <summary>
    /// Возвращает все возможные ходы ладьи (ратоборца) из заданной позиции.
    /// </summary>
    public static List<Position> GetMoves(ChessBoard board, Position from)
    {
        return LinearMoveGenerator.GetMoves(board, from, Directions);
    }
}