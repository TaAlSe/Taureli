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
        // 1. Проверяем, что фигура существует и соответствует цвету
        var movingPiece = board.GetTopPiece(move.From);
        if (movingPiece == null)
            return false;

        var currentColor = movingPiece.Color;

        // 2. Проверяем, что ход допустим по правилам движения (генератор ходов)
        //var possibleMoves = MoveGenerator.GetMovesForPiece(board, move.From);
        //if (!possibleMoves.Any(m => m.Equals(move)))
        //    return false;

        // 3. Сохраняем состояние для отката
        var capturedPiece = board.GetTopPiece(move.To);
        PieceType? promotionType = move.PromotionType;

        // 4. Выполняем ход временно
        if (promotionType.HasValue)
        {
            board.SetPiece(move.To, new Piece(promotionType.Value, currentColor));
        }
        else
        {
            board.SetPiece(move.To, movingPiece);
        }
        board.SetPiece(move.From, null);

        // 5. Проверяем, не остался ли король под шахом
        var kingPos = FindKing(board, currentColor);
        bool isLegal = false;
        if (kingPos.HasValue)
        {
            var opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            isLegal = !AttackDetector.IsSquareAttacked(board, kingPos.Value, opponentColor);
        }

        // 6. Откатываем ход
        board.SetPiece(move.From, movingPiece);
        board.SetPiece(move.To, capturedPiece);

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