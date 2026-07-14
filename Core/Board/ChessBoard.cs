using Core.Moves;
using Core.Pieces;  // чтобы использовать Piece, PieceColor, PieceType

namespace Core.Board;

/// <summary>
/// Представляет шахматную доску 8x8 для таврелей.
/// </summary>
public class ChessBoard
{
    public const int Size = 8;
    private readonly Piece?[,] _squares;

    // Права на рокировку (доступны только для чтения, изменяются через методы)
    public bool WhiteCanCastleKingSide { get; private set; }
    public bool WhiteCanCastleQueenSide { get; private set; }
    public bool BlackCanCastleKingSide { get; private set; }
    public bool BlackCanCastleQueenSide { get; private set; }

    // Конструкторы по умолчанию устанавливают все права в true
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
            // копирование массива
        }
    }

    /// <summary>
    /// Обновляет права на рокировку после выполнения хода.
    /// </summary>
    public void UpdateCastlingRights(Move move)
    {
        // Если ход начинается с позиции короля (e1 или e8)
        if (move.From == new Position(0, 4)) // белый король
        {
            WhiteCanCastleKingSide = false;
            WhiteCanCastleQueenSide = false;
        }
        else if (move.From == new Position(7, 4)) // чёрный король
        {
            BlackCanCastleKingSide = false;
            BlackCanCastleQueenSide = false;
        }

        // Если ход начинается с ладьи
        CheckRookMove(move.From);
        // Если ход заканчивается на клетке, где стояла ладья (взятие)
        CheckRookCapture(move.To);
    }

    private void CheckRookMove(Position pos)
    {
        if (pos == new Position(0, 0)) WhiteCanCastleQueenSide = false;
        else if (pos == new Position(0, 7)) WhiteCanCastleKingSide = false;
        else if (pos == new Position(7, 0)) BlackCanCastleQueenSide = false;
        else if (pos == new Position(7, 7)) BlackCanCastleKingSide = false;
    }

    private void CheckRookCapture(Position pos)
    {
        // Если ход заканчивается на начальной клетке ладьи, то ладья была взята
        // (это возможно, только если на той клетке стояла ладья противника)
        if (pos == new Position(0, 0)) WhiteCanCastleQueenSide = false;
        else if (pos == new Position(0, 7)) WhiteCanCastleKingSide = false;
        else if (pos == new Position(7, 0)) BlackCanCastleQueenSide = false;
        else if (pos == new Position(7, 7)) BlackCanCastleKingSide = false;
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
}