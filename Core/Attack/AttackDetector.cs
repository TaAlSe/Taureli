// Core/Attack/AttackDetector.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Core.Attack;

/// <summary>
/// Статический класс для проверки атаки клеток.
/// </summary>
public static class AttackDetector
{
    /// <summary>
    /// Проверяет, атакована ли заданная клетка фигурами указанного цвета.
    /// </summary>
    /// <param name="board">Доска.</param>
    /// <param name="pos">Проверяемая позиция.</param>
    /// <param name="byColor">Цвет атакующих фигур.</param>
    /// <returns>true, если клетка атакована.</returns>
    public static bool IsSquareAttacked(ChessBoard board, Position pos, PieceColor byColor)
    {
        // 1. Линейные фигуры (ладья, слон, ферзь, хелги)
        foreach (var (dRow, dCol) in Directions.QueenDirections)
        {
            for (int step = 1; step < 8; step++)
            {
                int row = pos.Row + dRow * step;
                int col = pos.Col + dCol * step;
                var target = new Position(row, col);
                if (!target.IsValid) break;

                var piece = board.GetTopPiece(target);
                if (piece == null) continue;

                if (piece.Color != byColor)
                    break; // вражеская фигура не подходит, дальше не идём

                if (CanPieceAttackInDirection(piece.Type, dRow, dCol))
                    return true;

                // своя фигура, но не атакует по этому направлению
                break;
            }
        }

        // 2. Конь (всадник) и хелги (как конь)
        foreach (var (dRow, dCol) in Directions.KnightDirections)
        {
            var target = new Position(pos.Row + dRow, pos.Col + dCol);
            if (!target.IsValid) continue;
            var piece = board.GetTopPiece(target);
            if (piece != null && piece.Color == byColor &&
                (piece.Type == PieceType.Horseman || piece.Type == PieceType.Helgi))
                return true;
        }

        // 3. Король (волхв)
        foreach (var (dRow, dCol) in Directions.QueenDirections)
        {
            var target = new Position(pos.Row + dRow, pos.Col + dCol);
            if (!target.IsValid) continue;
            var piece = board.GetTopPiece(target);
            if (piece != null && piece.Color == byColor && piece.Type == PieceType.Volkhv)
                return true;
        }

        // 4. Пешка (ратник) — атакует по диагонали вперёд
        int pawnDirection = byColor == PieceColor.White ? 1 : -1;
        foreach (int colOffset in new[] { -1, 1 })
        {
            var target = new Position(pos.Row - pawnDirection, pos.Col + colOffset);
            if (!target.IsValid) continue;
            var piece = board.GetTopPiece(target);
            if (piece != null && piece.Color == byColor && piece.Type == PieceType.Ratnik)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Проверяет, может ли фигура атаковать по данному направлению.
    /// </summary>
    private static bool CanPieceAttackInDirection(PieceType type, int dRow, int dCol)
    {
        bool isStraight = (dRow == 0 || dCol == 0);
        bool isDiagonal = (dRow != 0 && dCol != 0);

        return type switch
        {
            PieceType.Ratoborets => isStraight,
            PieceType.Archer => isDiagonal,
            PieceType.Knyaz => isStraight || isDiagonal,
            PieceType.Helgi => isStraight || isDiagonal,
            _ => false
        };
    }
}