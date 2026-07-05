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
    public static bool IsLegalMove(ChessBoard board, Move move, PieceColor currentColor)
    {
        // 1. Получаем фигуру на начальной клетке
        var movingPiece = board.GetPiece(move.From);
        if (movingPiece == null || movingPiece.Color != currentColor)
            return false; // нет фигуры или не та цвет

        // 2. Сохраняем состояние для отката
        var capturedPiece = board.GetPiece(move.To);
        PieceType? promotionType = move.PromotionType;

        // 3. Выполняем ход временно
        // Если есть превращение, ставим новую фигуру, иначе перемещаем
        if (promotionType.HasValue)
        {
            // Пешка превращается в указанную фигуру
            board.SetPiece(move.To, new Piece(promotionType.Value, currentColor));
            board.SetPiece(move.From, null);
        }
        else
        {
            // Обычный ход
            board.SetPiece(move.To, movingPiece);
            board.SetPiece(move.From, null);
        }

        // 4. Находим позицию короля текущего игрока
        Position? kingPos = FindKing(board, currentColor);
        bool isLegal = false;
        if (kingPos.HasValue)
        {
            // Проверяем, атакован ли король фигурами противника
            PieceColor opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            if (!AttackDetector.IsSquareAttacked(board, kingPos.Value, opponentColor))
                isLegal = true;
        }

        // 5. Откатываем ход
        if (promotionType.HasValue)
        {
            // Восстанавливаем пешку на From
            board.SetPiece(move.From, movingPiece);
            // Восстанавливаем фигуру на To (если была)
            board.SetPiece(move.To, capturedPiece);
        }
        else
        {
            board.SetPiece(move.From, movingPiece);
            board.SetPiece(move.To, capturedPiece);
        }

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
                var piece = board.GetPiece(pos);
                if (piece != null && piece.Type == PieceType.Volkhv && piece.Color == color)
                    return pos;
            }
        }
        return null;
    }

    /// <summary>
    /// Возвращает все легальные ходы для фигуры на заданной позиции.
    /// </summary>
    public static List<Move> GetLegalMoves(ChessBoard board, Position from, PieceColor currentColor)
    {
        var allMoves = MoveGenerator.GetMovesForPiece(board, from);
        var legalMoves = new List<Move>();
        foreach (var move in allMoves)
        {
            if (IsLegalMove(board, move, currentColor))
                legalMoves.Add(move);
        }
        return legalMoves;
    }
}