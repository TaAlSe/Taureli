using Core.Moves;
using Core.Pieces;
using Core.Validation;

namespace Core.Board;

/// <summary>
/// Представляет шахматную доску 8x8 для таврелей.
/// </summary>
public class ChessBoard
{
    public const int Size = 8;
    private readonly Piece?[,] _squares;

    // Права на рокировку (доступны только для чтения, изменяются через методы)
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
        _squares = new Piece?[Size, Size];
        WhiteCanCastleKingSide = whiteCanCastleKingSide;
        WhiteCanCastleQueenSide = whiteCanCastleQueenSide;
        BlackCanCastleKingSide = blackCanCastleKingSide;
        BlackCanCastleQueenSide = blackCanCastleQueenSide;

        if (initialPosition != null)
        {
            if (initialPosition.GetLength(0) != Size || initialPosition.GetLength(1) != Size)
                throw new ArgumentException($"Массив должен быть размером {Size}x{Size}", nameof(initialPosition));
            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                    _squares[row, col] = initialPosition[row, col];
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
    /// Создаёт доску, заполняя её переданной расстановкой.
    /// </summary>
    /// <param name="initialPosition">Двумерный массив 8x8 с фигурами (может содержать null).</param>
    /// <exception cref="ArgumentNullException">Если initialPosition == null.</exception>
    /// <exception cref="ArgumentException">Если размеры массива не 8x8.</exception>
    public ChessBoard(Piece?[,] initialPosition) : this()
    {
        ArgumentNullException.ThrowIfNull(initialPosition);
        if (initialPosition.GetLength(0) != Size || initialPosition.GetLength(1) != Size)
            throw new ArgumentException("Массив должен быть размером 8x8", nameof(initialPosition));

        for (int row = 0; row < _squares.GetLength(0); row++)
            for (int col = 0; col < _squares.GetLength(1); col++)
                _squares[row, col] = initialPosition[row, col];
    }

    /// <summary>
    /// Возвращает фигуру в заданной клетке.
    /// </summary>
    /// <param name="row">Строка от 0 до 7 (0 – чёрные, 7 – белые).</param>
    /// <param name="col">Столбец от 0 до 7 (a=0, h=7).</param>
    /// <returns>Фигура или null, если клетка пуста или координаты за пределами доски.</returns>
    public Piece? GetPiece(int row, int col)
    {
        return new Position(row, col).IsValid ? _squares[row, col] : null;
    }

    /// <summary>
    /// Возвращает фигуру по позиции (прямой доступ с проверкой IsValid).
    /// </summary>
    public Piece? GetPiece(Position pos) => pos.IsValid ? _squares[pos.Row, pos.Col] : null;

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    public void SetPiece(int row, int col, Piece? piece)
    {
        SetPiece(new Position(row, col), piece);
    }

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    /// <exception cref="ArgumentException">Если позиция выходит за пределы доски.</exception>
    public void SetPiece(Position pos, Piece? piece)
    {
        if (!pos.IsValid)
            throw new ArgumentException($"Некорректная позиция: {pos}", nameof(pos));
        _squares[pos.Row, pos.Col] = piece;
    }

    /// <summary>
    /// Очищает всю доску (все клетки становятся null).
    /// </summary>
    public void Clear()
    {
        Array.Clear(_squares, 0, _squares.Length);
    }

    /// <summary>
    /// Возвращает строковое представление доски для отладки (использует GetDebugCode у фигур).
    /// </summary>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (int row = 7; row >= 0; row--)  // печатаем с 8-й горизонтали по 1-ю
        {
            sb.Append(row + 1).Append("  ");
            for (int col = 0; col < Size; col++)
            {
                var piece = _squares[row, col];
                sb.Append(piece == null ? ".  " : piece.GetDebugCode() + " ");
            }
            sb.AppendLine();
        }
        sb.AppendLine("   a  b  c  d  e  f  g  h");
        return sb.ToString();
    }

    /// <summary>
    /// Выполняет ход на доске (постоянное изменение состояния).
    /// </summary>
    /// <param name="move">Ход, который нужно выполнить.</param>
    /// <returns>true, если ход был выполнен успешно.</returns>
    public bool MakeMove(Move move)
    {
        // 2. Получаем фигуру, которая ходит
        var movingPiece = GetPiece(move.From);
        if (movingPiece == null)
            return false;

        // 1. Проверяем легальность
        if (!MoveValidator.IsLegalMove(this, move))
            return false;

        var currentColor = movingPiece.Color;

        // 4. Выполняем ход в зависимости от типа
        if (move.IsCastling)
        {
            // Рокировка
            ExecuteCastling(move, currentColor);
        }
        else if (move.PromotionType.HasValue)
        {
            // Превращение пешки
            SetPiece(move.To, new Piece(move.PromotionType.Value, currentColor));
            SetPiece(move.From, null);
        }
        else
        {
            // Обычный ход
            SetPiece(move.To, movingPiece);
            SetPiece(move.From, null);
        }

        // 5. Обновляем права рокировки
        UpdateCastlingRights(move);
        // todo: обновить состояние для взятия на проходе

        // 6. (Позже) Обновляем состояние для взятия на проходе и т.д.
        return true;
    }

    /// <summary>
    /// Выполняет рокировку (перемещает короля и ладью).
    /// </summary>
    private void ExecuteCastling(Move move, PieceColor color)
    {
        int row = color == PieceColor.White ? 0 : 7;
        // Перемещаем короля
        var king = GetPiece(move.From);
        if (king != null)
        {
            SetPiece(move.To, king);
            SetPiece(move.From, null);
        }
        // Перемещаем ладью
        int rookFromCol = move.CastlingSide == CastlingSide.KingSide ? 7 : 0;
        int rookToCol = move.CastlingSide == CastlingSide.KingSide ? 5 : 3;
        var rook = GetPiece(new Position(row, rookFromCol));
        if (rook != null)
        {
            SetPiece(new Position(row, rookToCol), rook);
            SetPiece(new Position(row, rookFromCol), null);
        }
    }
}

public interface IBoard
{
    Piece? GetTopPiece(Position pos);
    void SetTopPiece(Position pos, Piece? piece);
    bool MakeMove(Move move, PieceColor currentColor);
    // Позже могут добавиться методы для работы со стеками
}