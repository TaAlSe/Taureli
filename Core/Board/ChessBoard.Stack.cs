// Core/Board/ChessBoard.Stack.cs

using Core.Moves;
using Core.Pieces;

namespace Core.Board;

public partial class ChessBoard
{
    // --- Стек-методы ---

    /// <summary>Возвращает верхнюю фигуру стека на позиции (или null).</summary>
    public Piece? GetTopPiece(Position pos)
    {
        if (!pos.IsValid) return null;
        var stack = _stacks[pos.Row, pos.Col];
        return stack?.LastOrDefault();
    }

    /// <summary>Возвращает весь стек на позиции (или null).</summary>
    public List<Piece>? GetStack(Position pos)
    {
        if (!pos.IsValid) return null;
        return _stacks[pos.Row, pos.Col];
    }

    /// <summary>Заменяет стек на позиции новым (может быть null).</summary>
    public void SetStack(Position pos, List<Piece>? stack)
    {
        if (!pos.IsValid) return;
        _stacks[pos.Row, pos.Col] = stack;
    }

    /// <summary>Добавляет фигуру сверху стека (слияние).</summary>
    public void PushPiece(Position pos, Piece piece)
    {
        if (!pos.IsValid) return;
        var stack = _stacks[pos.Row, pos.Col];
        if (stack == null)
        {
            stack = [];
            _stacks[pos.Row, pos.Col] = stack;
        }
        stack.Add(piece);
    }

    /// <summary>Удаляет верхнюю фигуру стека и возвращает её (или null).</summary>
    public Piece? PopPiece(Position pos)
    {
        if (!pos.IsValid) return null;
        var stack = _stacks[pos.Row, pos.Col];
        if (stack == null || stack.Count == 0) return null;
        var piece = stack[^1];
        stack.RemoveAt(stack.Count - 1);
        if (stack.Count == 0)
            _stacks[pos.Row, pos.Col] = null;
        return piece;
    }

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    public void SetPiece(Position pos, Piece? piece)
    {
        if (!pos.IsValid)
            throw new ArgumentException($"Некорректная позиция: {pos}", nameof(pos));
        _stacks[pos.Row, pos.Col] = piece != null ? [piece] : null;
    }

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    public void SetPiece(int row, int col, Piece? piece) => SetPiece(new Position(row, col), piece);

    // --- Вспомогательные методы для башен ---

    /// <summary>Соединяет две башни (top ставится поверх bottom).</summary>
    public static List<Piece>? ConcatStacks(List<Piece>? bottom, List<Piece>? top)
    {
        if (top == null) return bottom;
        if (bottom == null) return top;
        bottom.AddRange(top);
        return bottom;
    }

    /// <summary>Разделяет башню на две части: верхние count фигур и нижние.</summary>
    public static (List<Piece>? upper, List<Piece>? lower) SplitStack(List<Piece> stack, int count)
    {
        if (stack == null || count <= 0 || count >= stack.Count)
            return (null, stack);
        // Верхние count фигур — это последние count элементов
        var upper = stack.Skip(stack.Count - count).ToList();
        var lower = stack.Take(stack.Count - count).ToList();
        return (upper, lower);
    }
}
