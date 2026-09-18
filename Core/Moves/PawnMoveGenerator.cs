using Core.Board;
using Core.Pieces;

namespace Core.Moves;

/// <summary>
/// Генератор ходов для ратника (пешки) без учёта взятия на проходе и особого превращения в хелги.
/// </summary>
public static class PawnMoveGenerator
{
    /// <summary>
    /// Возвращает все возможные ходы для пешки из заданной позиции.
    /// </summary>
    /// <param name="board">Текущая доска.</param>
    /// <param name="from">Позиция пешки.</param>
    /// <returns>Список ходов (Move) с учётом превращения на последней горизонтали.</returns>
    public static List<Move> GetMoves(ChessBoard board, Position from)
    {
        // Проверяем, что на указанной клетке действительно стоит пешка.
        var piece = board.GetTopPiece(from);
        if (piece == null || piece.Type != PieceType.Ratnik)
            return [];

        // Список ходов (Move).
        var moves = new List<Move>();

        // Направление движения: +1 для белых (вверх), -1 для чёрных (вниз).
        int direction = piece.Color == PieceColor.White ? 1 : -1;

        // Начальный ряд для двойного хода: для белых — 1 (вторая горизонталь), для чёрных — 6 (седьмая).
        int startRow = piece.Color == PieceColor.White ? 1 : 6;
        int promotionRow = piece.Color == PieceColor.White ? 7 : 0;

        // 1. Ход на 1 клетку вперёд
        int newRow = from.Row + direction;
        if (newRow >= 0 && newRow < 8)
        {
            // Позиция для хода.
            var targetPos = new Position(newRow, from.Col);
            if (board.GetTopPiece(targetPos) == null)
            {
                // Ход с превращением, если достигли последней горизонтали (пока null).
                PieceType? promotionType = (newRow == promotionRow) ? piece.PromotionTarget : null;
                moves.Add(new Move(from, targetPos, promotionType));
            }
        }

        // 2. Ход на 2 клетки с начальной позиции
        if (from.Row == startRow)
        {
            // Целевая строка для двойного хода.
            int twoStepRow = from.Row + 2 * direction;
            if (twoStepRow >= 0 && twoStepRow < 8)
            {
                // Промежуточная клетка.
                var intermediatePos = new Position(from.Row + direction, from.Col);
                // Позиция для хода.
                var targetPos = new Position(twoStepRow, from.Col);
                if (board.GetTopPiece(intermediatePos) == null && board.GetTopPiece(targetPos) == null)
                {
                    moves.Add(new Move(from, targetPos));
                }
            }
        }

        // 3. Взятие по диагонали
        foreach (int colOffset in new[] { -1, 1 })
        {
            int targetCol = from.Col + colOffset;
            if (targetCol < 0 || targetCol >= 8) continue;

            int targetRow = from.Row + direction;
            if (targetRow < 0 || targetRow >= 8) continue;

            var targetPos = new Position(targetRow, targetCol);
            var targetPiece = board.GetTopPiece(targetPos);

            // Взятие возможно только если на целевой клетке стоит вражеская фигура.
            if (targetPiece != null && targetPiece.Color != piece.Color)
            {
                // Если пешка достигает последней горизонтали — превращение (пока null).
                PieceType? promotionType = (targetRow == promotionRow) ? piece.PromotionTarget : null;
                moves.Add(new Move(from, targetPos, promotionType));
            }
        }

        return moves;
    }
}