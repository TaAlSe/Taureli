using Core.Board;

namespace Core.Moves;

/// <summary>
/// Генератор ходов для всадника (коня). Перепрыгивает через фигуры.
/// </summary>
public static class KnightMoveGenerator
{
    /// <summary>
    /// Возвращает все возможные ходы всадника из заданной позиции.
    /// </summary>
    public static List<Position> GetMoves(ChessBoard board, Position from)
    {
        var piece = board.GetPiece(from.Row, from.Col);
        if (piece == null) return [];

        var moves = new List<Position>();
        foreach (var (dRow, dCol) in Directions.KnightDirections)
        {
            int newRow = from.Row + dRow;
            int newCol = from.Col + dCol;
            var target = new Position(newRow, newCol);
            if (!target.IsValid) continue;

            var targetPiece = board.GetPiece(newRow, newCol);
            if (targetPiece == null || targetPiece.Color != piece.Color)
                moves.Add(target);
        }
        return moves;
    }
}