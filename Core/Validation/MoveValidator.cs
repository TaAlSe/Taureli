using Core.Board;
using Core.Pieces;
using Core.Moves;
using Core.Attack;

namespace Core.Validation;

/// <summary>
/// Проверяет легальность ходов с учётом шаха.
/// </summary>
public static class MoveValidator
{
    /// <summary>
    /// Проверяет, является ли ход легальным (не оставляет своего короля под шахом).
    /// </summary>
    /// <param name="board">Текущая доска.</param>
    /// <param name="move">Проверяемый ход.</param>
    /// <param name="currentColor">Цвет игрока, совершающего ход.</param>
    /// <returns>true, если ход легален.</returns>
    public static bool IsLegalMove(ChessBoard board, Move move)
    {
        // Проверка, что ход присутствует в генераторе (с учётом SplitCount)
        var possibleMoves = MoveGenerator.GetMovesForPiece(board, move.From);
        if (!possibleMoves.Any(m => m.Equals(move)))
            return false;

        // Сохраняем копии стеков для отката
        var fromStackCopy = board.GetStack(move.From)?.ToList();
        var toStackCopy = board.GetStack(move.To)?.ToList();

        // Выполняем ход через внутренний метод
        board.ExecuteMoveInternal(move);

        // Находим цвет фигуры, которая ходила (она должна быть на доске)
        var movingPiece = board.GetTopPiece(move.From) ?? board.GetTopPiece(move.To);
        if (movingPiece == null)
        {
            // Если фигура исчезла (например, превращение), то ход нелегален
            board.SetStack(move.From, fromStackCopy);
            board.SetStack(move.To, toStackCopy);
            return false;
        }

        var currentColor = movingPiece.Color;
        var kingPos = FindKing(board, currentColor);
        bool isLegal = false;
        if (kingPos.HasValue)
        {
            var opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            isLegal = !AttackDetector.IsSquareAttacked(board, kingPos.Value, opponentColor);
        }

        // Откат
        board.SetStack(move.From, fromStackCopy);
        board.SetStack(move.To, toStackCopy);

        return isLegal;
    }

    /// <summary>
    /// Находит позицию короля указанного цвета.
    /// </summary>
    private static Position? FindKing(ChessBoard board, PieceColor color)
    {
        return color == PieceColor.White ? board.WhiteKingPosition : board.BlackKingPosition;
    }

    /// <summary>
    /// Возвращает все легальные ходы для фигуры на заданной позиции.
    /// </summary>
    public static List<Move> GetLegalMoves(ChessBoard board, Position from)
    {
        var allMoves = MoveGenerator.GetMovesForPiece(board, from);
        var legalMoves = new List<Move>();
        foreach (var move in allMoves)
        {
            if (IsLegalMove(board, move))
                legalMoves.Add(move);
        }
        return legalMoves;
    }
}