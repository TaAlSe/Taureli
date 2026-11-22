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
        LastMove = move;
        return true;
    }

    /// <summary>
    /// Выполняет ход без проверки легальности.
    /// Автоматически превращает ратника, достигшего последней горизонтали.
    /// </summary>
    /// <param name="move">Ход.</param>
    /// <exception cref="InvalidOperationException">Если на исходной клетке нет фигуры.</exception>
    public void ExecuteMoveInternal(Move move)
    {
        var currentColor = GetTopPiece(move.From)?.Color
            ?? throw new InvalidOperationException("Нет фигуры на исходной клетке");

        // Обновление позиции короля будет сделано после перемещения

        if (move.IsCastling)
        {
            ExecuteCastling(move, currentColor);
            // после рокировки обновим позиции короля
            UpdateKingPosition(currentColor, move.To);
            return;
        }

        // Обычный ход (с возможным разделением башни).
        List<Piece>? movingStack;
        if (move.SplitCount is > 0)
        {
            // Разделяем башню
            var stack = GetStack(move.From);
            if (stack == null || move.SplitCount.Value >= stack.Count)
            {
                // splitCount >= размеру стека — перемещаем всю башню.
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

        if (movingStack is { Count: > 0 })
        {
            // Добавляем перемещаемую часть на целевую клетку (слияние)
            var targetStack = GetStack(move.To);

            // Откат превращённой фигуры, если она оказалась под новой башней.
            // Правило: превращённая фигура, на которую встаёт любая башня
            // (своя или вражеская), переворачивается обратно в ратника.
            if (targetStack is { Count: > 0 } && targetStack[^1].IsPromoted)
                targetStack[^1] = RevertToRatnik(targetStack[^1]);

            var newStack = ConcatStacks(targetStack, movingStack);
            SetStack(move.To, newStack);

            // Помечаем перемещённые фигуры как ходившие.
            MarkMoved(move.To, movingStack.Count);

            // Автоматическое превращение верхней фигуры (если ратник на последней горизонтали).
            TryPromote(move.To);

            // Если перемещался волхв — обновляем его позицию.
            if (movingStack.Any(p => p.Type == PieceType.Volkhv))
                UpdateKingPosition(currentColor, move.To);
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
            PushPiece(move.To, king);

        // Перемещаем ладью
        int rookFromCol = move.CastlingSide == CastlingSide.KingSide ? 7 : 0;
        int rookToCol = move.CastlingSide == CastlingSide.KingSide ? 5 : 3;
        var rook = PopPiece(new Position(row, rookFromCol));
        if (rook != null)
            PushPiece(new Position(row, rookToCol), rook);
    }
}