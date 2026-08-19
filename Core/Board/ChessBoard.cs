using Core.Moves;
using Core.Pieces;
using Core.Validation;

namespace Core.Board;

/// <summary>
/// Представляет шахматную доску 8x8 для таврелей.
/// </summary>
public class ChessBoard : IBoard
{
    public const int Size = 8;

    // Новое хранилище для башен (стеки)
    private readonly List<Piece>?[,] _stacks;

    // Права на рокировку
    /// <summary>Может ли белый сделать короткую рокировку (с королевской стороны).</summary>
    public bool WhiteCanCastleKingSide { get; private set; }
    /// <summary>Может ли белый сделать длинную рокировку (с ферзевой стороны).</summary>
    public bool WhiteCanCastleQueenSide { get; private set; }
    /// <summary>Может ли чёрный сделать короткую рокировку.</summary>
    public bool BlackCanCastleKingSide { get; private set; }
    /// <summary>Может ли чёрный сделать длинную рокировку.</summary>
    public bool BlackCanCastleQueenSide { get; private set; }
    /// <summary>Текущая позиция белого короля (null, если король отсутствует на доске).</summary>
    public Position? WhiteKingPosition { get; private set; }
    /// <summary>Текущая позиция чёрного короля (null, если король отсутствует на доске).</summary>
    public Position? BlackKingPosition { get; private set; }

    /// <summary>
    /// Обновляет позицию короля указанного цвета.
    /// </summary>
    /// <param name="color">Цвет короля.</param>
    /// <param name="newPosition">Новая позиция (может быть null, если король снят с доски).</param>
    private void UpdateKingPosition(PieceColor color, Position? newPosition)
    {
        if (color == PieceColor.White)
            WhiteKingPosition = newPosition;
        else
            BlackKingPosition = newPosition;
    }

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

    // --- Остальные методы ---

    public override string ToString()
    {
        // Пока используем старые методы для отображения
        var sb = new System.Text.StringBuilder();
        for (int row = Size - 1; row >= 0; row--)
        {
            sb.Append(row + 1).Append("  ");
            for (int col = 0; col < Size; col++)
            {
                var pos = new Position(row, col);
                var piece = GetTopPiece(pos);
                sb.Append(piece == null ? ".  " : piece.GetDebugCode() + " ");
            }
            sb.AppendLine();
        }
        sb.AppendLine("   a  b  c  d  e  f  g  h");
        return sb.ToString();
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
    /// Обновляет права на рокировку в зависимости от того, на какой клетке находится ладья.
    /// Если позиция совпадает с начальной клеткой ладьи, соответствующее право сбрасывается.
    /// </summary>
    /// <param name="pos">Позиция, которую нужно проверить (начальная или конечная клетка хода).</param>
    private void CheckRookPosition(Position pos)
    {
        (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, BlackCanCastleQueenSide, BlackCanCastleKingSide) = pos switch
        {
            (0, 0) => (false, WhiteCanCastleKingSide, BlackCanCastleQueenSide, BlackCanCastleKingSide),
            (0, 7) => (WhiteCanCastleQueenSide, false, BlackCanCastleQueenSide, BlackCanCastleKingSide),
            (7, 0) => (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, false, BlackCanCastleKingSide),
            (7, 7) => (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, BlackCanCastleQueenSide, false),
            _ => (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, BlackCanCastleQueenSide, BlackCanCastleKingSide)
        };
    }

    /// <summary>
    /// Обновляет права на рокировку после выполнения хода.
    /// </summary>
    public void UpdateCastlingRights(Move move)
    {
        // Король
        if (move.From == new Position(0, 4))
        {
            WhiteCanCastleKingSide = false;
            WhiteCanCastleQueenSide = false;
        }
        else if (move.From == new Position(7, 4))
        {
            BlackCanCastleKingSide = false;
            BlackCanCastleQueenSide = false;
        }

        // Ладья на начальной клетке (двигалась)
        CheckRookPosition(move.From);
        // Ладья на конечной клетке (была взята)
        CheckRookPosition(move.To);
    }

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

    /// <summary>
    /// Находит позицию короля указанного цвета на доске (использует кэшированные позиции).
    /// </summary>
    public Position? FindKing(PieceColor color)
    {
        return color == PieceColor.White ? WhiteKingPosition : BlackKingPosition;
    }
}

public interface IBoard
{
    Piece? GetTopPiece(Position pos);
    List<Piece>? GetStack(Position pos);
    void SetStack(Position pos, List<Piece>? stack);
    void PushPiece(Position pos, Piece piece);
    Piece? PopPiece(Position pos);
    void SetPiece(Position pos, Piece? piece); // оставляем для удобства тестов
    bool MakeMove(Move move);
    void UpdateCastlingRights(Move move);
    bool WhiteCanCastleKingSide { get; }
    bool WhiteCanCastleQueenSide { get; }
    bool BlackCanCastleKingSide { get; }
    bool BlackCanCastleQueenSide { get; }
}