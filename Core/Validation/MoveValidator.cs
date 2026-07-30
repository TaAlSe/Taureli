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
        // 1. Проверяем, что фигура существует
        var movingPiece = board.GetTopPiece(move.From);
        if (movingPiece == null)
            return false;

        var currentColor = movingPiece.Color;

        // 2. Проверка генератора (пока закомментирована, адаптируем позже)
        // var possibleMoves = MoveGenerator.GetMovesForPiece(board, move.From);
        // if (!possibleMoves.Any(m => m.Equals(move)))
        //     return false;

        // 3. Сохраняем стеки для отката (включая null)
        var fromStack = board.GetStack(move.From);
        var toStack = board.GetStack(move.To);
        PieceType? promotionType = move.PromotionType;

        // 4. Временно выполняем ход
        if (promotionType.HasValue)
        {
            // Превращение: удаляем пешку, ставим новую фигуру
            board.PopPiece(move.From);
            board.PushPiece(move.To, new Piece(promotionType.Value, currentColor));
        }
        else
        {
            // Обычный ход (без разделения): перемещаем верхнюю фигуру
            var piece = board.PopPiece(move.From);
            if (piece != null)
                board.PushPiece(move.To, piece);
        }

        // 5. Проверяем, не остался ли король под шахом
        var kingPos = FindKing(board, currentColor);
        bool isLegal = false;
        if (kingPos.HasValue)
        {
            var opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            isLegal = !AttackDetector.IsSquareAttacked(board, kingPos.Value, opponentColor);
        }

        // 6. Откатываем ход (восстанавливаем стеки)
        board.SetStack(move.From, fromStack);
        board.SetStack(move.To, toStack);

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