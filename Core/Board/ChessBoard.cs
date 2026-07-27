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
    private readonly LinkedList<Piece>?[,] _stacks;

    // Права на рокировку
    /// <summary>Может ли белый сделать короткую рокировку (с королевской стороны).</summary>
    public bool WhiteCanCastleKingSide { get; private set; }
    /// <summary>Может ли белый сделать длинную рокировку (с ферзевой стороны).</summary>
    public bool WhiteCanCastleQueenSide { get; private set; }
    /// <summary>Может ли чёрный сделать короткую рокировку.</summary>
    public bool BlackCanCastleKingSide { get; private set; }
    /// <summary>Может ли чёрный сделать длинную рокировку.</summary>
    public bool BlackCanCastleQueenSide { get; private set; }

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
        _stacks = new LinkedList<Piece>?[Size, Size];

        WhiteCanCastleKingSide = whiteCanCastleKingSide;
        WhiteCanCastleQueenSide = whiteCanCastleQueenSide;
        BlackCanCastleKingSide = blackCanCastleKingSide;
        BlackCanCastleQueenSide = blackCanCastleQueenSide;

        if (initialPosition != null)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    var piece = initialPosition[row, col];
                    if (piece != null)
                    {
                        var stack = new LinkedList<Piece>();
                        stack.AddFirst(piece);
                        _stacks[row, col] = stack;
                    }
                    else
                    {
                        _stacks[row, col] = null;
                    }
                }
            }
        }
    }

    // --- Старые методы (остаются без изменений) ---

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    public void SetPiece(int row, int col, Piece? piece) => SetPiece(new Position(row, col), piece);

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    public void SetPiece(Position pos, Piece? piece)
    {
        if (!pos.IsValid)
            throw new ArgumentException($"Некорректная позиция: {pos}", nameof(pos));
        // Также обновляем стек: если piece != null, создаём стек с одной фигурой, иначе null
        if (piece != null)
        {
            var stack = new LinkedList<Piece>();
            stack.AddFirst(piece);
            _stacks[pos.Row, pos.Col] = stack;
        }
        else
        {
            _stacks[pos.Row, pos.Col] = null;
        }
    }

    public override string ToString()
    {
        // Пока используем старые методы для отображения
        var sb = new System.Text.StringBuilder();
        for (int row = Size - 1; row >= 0; row--)
        {
            sb.Append(row + 1).Append("  ");
            for (int col = 0; col < Size; col++)
            {
                var piece = GetTopPiece(new Position(row, col));
                sb.Append(piece == null ? ".  " : piece.GetDebugCode() + " ");
            }
            sb.AppendLine();
        }
        sb.AppendLine("   a  b  c  d  e  f  g  h");
        return sb.ToString();
    }

    // --- Новые методы для работы со стеками ---

    /// <summary>Возвращает верхнюю фигуру стека на позиции (или null).</summary>
    public Piece? GetTopPiece(Position pos)
    {
        if (!pos.IsValid) return null;
        var stack = _stacks[pos.Row, pos.Col];
        return stack?.First?.Value;
    }

    /// <summary>Возвращает весь стек на позиции (или null).</summary>
    public LinkedList<Piece>? GetStack(Position pos)
    {
        if (!pos.IsValid) return null;
        return _stacks[pos.Row, pos.Col];
    }

    /// <summary>Заменяет стек на позиции новым (может быть null).</summary>
    public void SetStack(Position pos, LinkedList<Piece>? stack)
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
            stack = new LinkedList<Piece>();
            _stacks[pos.Row, pos.Col] = stack;
        }
        stack.AddFirst(piece);
    }

    /// <summary>Удаляет верхнюю фигуру стека и возвращает её (или null).</summary>
    public Piece? PopPiece(Position pos)
    {
        if (!pos.IsValid) return null;
        var stack = _stacks[pos.Row, pos.Col];
        if (stack == null || stack.First == null) return null;
        var piece = stack.First.Value;
        stack.RemoveFirst();
        if (stack.Count == 0)
        {
            _stacks[pos.Row, pos.Col] = null;
        }
        return piece;
    }

    /// <summary>Соединяет две башни (top ставится поверх bottom).</summary>
    public static LinkedList<Piece>? ConcatStacks(LinkedList<Piece>? bottom, LinkedList<Piece>? top)
    {
        if (top == null) return bottom;
        if (bottom == null) return top;
        // Копируем элементы top в bottom (это O(n), но для MVP приемлемо)
        foreach (var piece in top)
            bottom.AddLast(piece);
        return bottom;
    }

    /// <summary>Разделяет башню на две части: верхние count фигур и нижние.</summary>
    public static (LinkedList<Piece>? upper, LinkedList<Piece>? lower) SplitStack(LinkedList<Piece> stack, int count)
    {
        if (stack == null || count <= 0 || count >= stack.Count)
            return (null, stack);
        var upper = new LinkedList<Piece>();
        var lower = new LinkedList<Piece>();
        int i = 0;
        foreach (var piece in stack)
        {
            if (i < count)
                upper.AddLast(piece);
            else
                lower.AddLast(piece);
            i++;
        }
        return (upper, lower);
    }

    // --- Остальные методы (UpdateCastlingRights, MakeMove, ExecuteCastling) ---
    // Они пока используют старые методы, но в микрошаге 3 мы их перепишем.
    // Пока оставляем их без изменений.

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
    /// <param name="splitCount">Количество верхних фигур, которые нужно переместить (если null, перемещается вся башня).</param>
    /// <returns>true, если ход выполнен успешно.</returns>
    public bool MakeMove(Move move, int? splitCount = null)
    {
        // 1. Получаем верхнюю фигуру на начальной клетке
        var movingPiece = GetTopPiece(move.From);
        if (movingPiece == null)
            return false;

        // 2. Проверяем легальность (используем старый валидатор, который пока работает с GetPiece)
        if (!MoveValidator.IsLegalMove(this, move))
            return false;

        var currentColor = movingPiece.Color;

        // 3. Выполняем ход в зависимости от типа
        if (move.IsCastling)
        {
            ExecuteCastling(move, currentColor);
        }
        else if (move.PromotionType.HasValue)
        {
            // Превращение: удаляем пешку, ставим новую фигуру
            PopPiece(move.From);
            PushPiece(move.To, new Piece(move.PromotionType.Value, currentColor));
        }
        else
        {
            // Обычный ход: либо вся башня, либо часть
            LinkedList<Piece>? movingStack;
            if (splitCount.HasValue && splitCount.Value > 0)
            {
                // Разделяем башню
                var stack = GetStack(move.From);
                if (stack == null || splitCount.Value >= stack.Count)
                {
                    // Если splitCount >= размеру стека, перемещаем всю башню (как обычный ход)
                    movingStack = stack;
                    SetStack(move.From, null);
                }
                else
                {
                    // Разделяем: верхние splitCount фигур уходят, нижние остаются
                    var (upper, lower) = SplitStack(stack, splitCount.Value);
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
        }

        // 4. Обновляем права рокировки (они не зависят от стеков, работают с позициями)
        UpdateCastlingRights(move);

        // todo: обновить состояние для взятия на проходе и превращения в хелги

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
}

public interface IBoard
{
    Piece? GetTopPiece(Position pos);
    LinkedList<Piece>? GetStack(Position pos);
    void SetStack(Position pos, LinkedList<Piece>? stack);
    void PushPiece(Position pos, Piece piece);
    Piece? PopPiece(Position pos);
    void SetPiece(Position pos, Piece? piece); // оставляем для удобства тестов
    bool MakeMove(Move move, int? splitCount = null);
    void UpdateCastlingRights(Move move);
    bool WhiteCanCastleKingSide { get; }
    bool WhiteCanCastleQueenSide { get; }
    bool BlackCanCastleKingSide { get; }
    bool BlackCanCastleQueenSide { get; }
}