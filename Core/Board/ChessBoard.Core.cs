// Core/Board/ChessBoard.Core.cs

using Core.Moves;
using Core.Pieces;

namespace Core.Board;

public partial class ChessBoard
{
    public const int Size = 8;

    // Новое хранилище для башен (стеки)
    private readonly List<Piece>?[,] _stacks;

    // Конструкторы по умолчанию устанавливают все права в true
    /// <summary>
    /// Создаёт доску, заполняя её переданной расстановкой.
    /// </summary>
    /// <param name="initialPosition">Двумерный массив 8x8 с фигурами (может содержать null).</param>
    /// <exception cref="ArgumentException">Если размеры массива не 8x8.</exception>
    public ChessBoard(
        Piece?[,]? initialPosition = null,
        bool whiteCanCastleKingSide = true,
        bool whiteCanCastleQueenSide = true,
        bool blackCanCastleKingSide = true,
        bool blackCanCastleQueenSide = true)
    {
        _stacks = new List<Piece>?[Size, Size];

        WhiteCanCastleKingSide = whiteCanCastleKingSide;
        WhiteCanCastleQueenSide = whiteCanCastleQueenSide;
        BlackCanCastleKingSide = blackCanCastleKingSide;
        BlackCanCastleQueenSide = blackCanCastleQueenSide;

        if (initialPosition == null)
            return;

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                var piece = initialPosition[row, col];

                if (piece == null)
                    continue;

                _stacks[row, col] = [piece];

                if (piece.Type != PieceType.Volkhv)
                    continue;

                var pos = new Position(row, col);
                if (piece.Color == PieceColor.White)
                    WhiteKingPosition = pos;
                else
                    BlackKingPosition = pos;
            }
        }
    }

    /// <summary>Текущая позиция белого короля (null, если король отсутствует на доске).</summary>
    public Position? WhiteKingPosition { get; private set; }
    /// <summary>Текущая позиция чёрного короля (null, если король отсутствует на доске).</summary>
    public Position? BlackKingPosition { get; private set; }

    /// <summary>
    /// Последний успешно выполненный ход (null, если ходов ещё не было).
    /// Используется для правил, зависящих от предыдущего хода (взятие на проходе).
    /// </summary>
    public Move? LastMove { get; private set; }

    // Права на рокировку
    /// <summary>Может ли белый сделать короткую рокировку (с королевской стороны).</summary>
    public bool WhiteCanCastleKingSide { get; private set; }
    /// <summary>Может ли белый сделать длинную рокировку (с ферзевой стороны).</summary>
    public bool WhiteCanCastleQueenSide { get; private set; }
    /// <summary>Может ли чёрный сделать короткую рокировку.</summary>
    public bool BlackCanCastleKingSide { get; private set; }
    /// <summary>Может ли чёрный сделать длинную рокировку.</summary>
    public bool BlackCanCastleQueenSide { get; private set; }

    public override string ToString()
    {
        const int cellWidth = 5; // место под "PX2* " — макс. 3 символа + суффикс + пробел
        var sb = new System.Text.StringBuilder();

        for (int row = Size - 1; row >= 0; row--)
        {
            sb.Append(row + 1).Append("  ");
            for (int col = 0; col < Size; col++)
            {
                var pos = new Position(row, col);
                var stack = GetStack(pos);

                if (stack == null || stack.Count == 0)
                {
                    sb.Append('.', 1).Append(' ', cellWidth - 1);
                    continue;
                }

                var top = stack[^1];
                var code = top.GetDisplayCode();              // "PX" / "R*" / "v" ...
                var height = stack.Count > 1
                    ? stack.Count.ToString()
                    : "";
                var cell = code + height;                      // "PX2", "R*", "v"
                sb.Append(cell);
                sb.Append(' ', Math.Max(1, cellWidth - cell.Length));
            }
            sb.AppendLine();
        }

        sb.Append("   ");
        for (int col = 0; col < Size; col++)
            sb.Append((char)('a' + col)).Append("    ");
        sb.AppendLine();
        return sb.ToString();
    }

    /// <summary>
    /// Возвращает пошаговый дамп всех непустых стеков — для отладки.
    /// Формат: «клетка: [снизу → вверху]».
    /// </summary>
    public string DumpStacks()
    {
        var sb = new System.Text.StringBuilder();
        for (int row = Size - 1; row >= 0; row--)
            for (int col = 0; col < Size; col++)
            {
                var pos = new Position(row, col);
                var stack = GetStack(pos);
                if (stack is null or { Count: 0 }) continue;

                var pieces = string.Join(", ", stack.Select(p => p.GetDisplayCode()));
                sb.AppendLine($"{pos.ToAlgebraic()}: [{pieces}]");
            }
        return sb.ToString();
    }

    /// <summary>
    /// Находит позицию короля указанного цвета на доске (использует кэшированные позиции).
    /// </summary>
    public Position? FindKing(PieceColor color)
    {
        return color == PieceColor.White ? WhiteKingPosition : BlackKingPosition;
    }
}