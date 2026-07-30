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
        var fromStackCopy = board.GetStack(move.From)?.ToList();
        var toStackCopy = board.GetStack(move.To)?.ToList();

        // Выполняем ход через внутренний метод (без проверок)
        board.ExecuteMoveInternal(move, move.SplitCount);

        var movingPiece = board.GetTopPiece(move.From) ?? board.GetTopPiece(move.To);
        var currentColor = movingPiece?.Color ?? throw new InvalidOperationException("No piece after move");

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
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var pos = new Position(row, col);
                var piece = board.GetTopPiece(pos);
                if (piece != null && piece.Type == PieceType.Volkhv && piece.Color == color)
                    return pos;
            }
        }
        return null;
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