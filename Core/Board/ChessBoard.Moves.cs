// Core/Board/ChessBoard.Moves.cs

using Core.Moves;
using Core.Pieces;
using Core.Validation;

namespace Core.Board;

public partial class ChessBoard
{
    /// <summary>
    /// Выполняет ход на доске.
    /// </summary>
    /// <param name="move">Ход, который нужно выполнить.</param>
    /// <returns>true, если ход выполнен успешно.</returns>
    public bool MakeMove(Move move)
    {
        if (!MoveValidator.IsLegalMove(this, move))
            return false;

        ExecuteMoveInternal(move);
        UpdateCastlingRights(move);
        return true;
    }

    /// <summary>
    /// 3. Выполняем ход в зависимости от типа
    /// </summary>
    /// <param name="move"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ExecuteMoveInternal(Move move)
    {
        var currentColor = GetTopPiece(move.From)?.Color ?? throw new InvalidOperationException("No piece at from");

        // Обновление позиции короля будет сделано после перемещения

        if (move.IsCastling)
        {
            ExecuteCastling(move, currentColor);
            // после рокировки обновим позиции короля
            UpdateKingPosition(currentColor, move.To);
        }
        else if (move.PromotionType.HasValue)
        {
            // Превращение: удаляем пешку, ставим новую фигуру
            PopPiece(move.From);
            PushPiece(move.To, new Piece(move.PromotionType.Value, currentColor));
            // Пешка превращается, но если это была пешка, то позиция короля не меняется
        }
        else
        {
            // обычный ход
            List<Piece>? movingStack;
            if (move.SplitCount.HasValue && move.SplitCount.Value > 0)
            {
                // Разделяем башню
                var stack = GetStack(move.From);
                if (stack == null || move.SplitCount.Value >= stack.Count)
                {
                    // Если splitCount >= размеру стека, перемещаем всю башню (как обычный ход)
                    movingStack = stack;
                    SetStack(move.From, null);
                }
                else
                {
                    // Разделяем: верхние splitCount фигур уходят, нижние остаются
                    var (upper, lower) = SplitStack(stack, move.SplitCount.Value);
                    movingStack = upper;
                    SetStack(move.From, lower);
                }
            }
            else
            {
                // Вся башня
                movingStack = GetStack(move.From);
                SetStack(move.From, null);
            }

            if (movingStack != null && movingStack.Count > 0)
            {
                // Добавляем перемещаемую часть на целевую клетку (слияние)
                var targetStack = GetStack(move.To);
                var newStack = ConcatStacks(targetStack, movingStack);
                SetStack(move.To, newStack);
            }

            // Если перемещалась башня, содержащая короля, то обновляем его позицию
            if (movingStack != null && movingStack.Any(p => p.Type == PieceType.Volkhv))
            {
                UpdateKingPosition(currentColor, move.To);
            }
        }
    }

    /// <summary>
    /// Выполняет рокировку (перемещает короля и ладью).
    /// </summary>
    private void ExecuteCastling(Move move, PieceColor color)
    {
        int row = color == PieceColor.White ? 0 : 7;
        // Перемещаем короля
        var king = PopPiece(move.From);
        if (king != null)
        {
            PushPiece(move.To, king);
        }
        // Перемещаем ладью
        int rookFromCol = move.CastlingSide == CastlingSide.KingSide ? 7 : 0;
        int rookToCol = move.CastlingSide == CastlingSide.KingSide ? 5 : 3;
        var rook = PopPiece(new Position(row, rookFromCol));
        if (rook != null)
        {
            PushPiece(new Position(row, rookToCol), rook);
        }
    }
}
