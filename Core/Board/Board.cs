namespace Core.Board;

using Core.Pieces;  // чтобы использовать Piece, PieceColor, PieceType

/// <summary>
/// Представляет шахматную доску 8x8 для таврелей.
/// </summary>
public class Board
{
    private readonly Piece?[,] _squares;

    /// <summary>
    /// Создаёт пустую доску (все клетки null).
    /// </summary>
    public Board()
    {
        _squares = new Piece?[8, 8];
    }

    /// <summary>
    /// Создаёт доску, заполняя её переданной расстановкой.
    /// </summary>
    /// <param name="initialPosition">Двумерный массив 8x8 с фигурами (может содержать null).</param>
    /// <exception cref="ArgumentNullException">Если initialPosition == null.</exception>
    /// <exception cref="ArgumentException">Если размеры массива не 8x8.</exception>
    public Board(Piece?[,] initialPosition) : this()
    {
        if (initialPosition == null)
            throw new ArgumentNullException(nameof(initialPosition));
        if (initialPosition.GetLength(0) != 8 || initialPosition.GetLength(1) != 8)
            throw new ArgumentException("Массив должен быть размером 8x8", nameof(initialPosition));

        for (int row = 0; row < 8; row++)
            for (int col = 0; col < 8; col++)
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
        return IsInside(row, col) ? _squares[row, col] : null;
    }

    /// <summary>
    /// Устанавливает фигуру в заданную клетку (можно null для очистки).
    /// </summary>
    public void SetPiece(int row, int col, Piece? piece)
    {
        if (IsInside(row, col))
            _squares[row, col] = piece;
    }

    /// <summary>
    /// Проверяет, находятся ли координаты в пределах доски.
    /// </summary>
    public static bool IsInside(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    /// <summary>
    /// Очищает всю доску (все клетки становятся null).
    /// </summary>
    public void Clear()
    {
        for (int row = 0; row < 8; row++)
            for (int col = 0; col < 8; col++)
                _squares[row, col] = null;
    }

    /// <summary>
    /// Возвращает строковое представление доски для отладки (использует GetDebugCode у фигур).
    /// </summary>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (int row = 0; row < 8; row++)
        {
            // Номер горизонтали (8 - row, чтобы 8 была внизу)
            sb.Append(8 - row).Append("  ");
            for (int col = 0; col < 8; col++)
            {
                var piece = _squares[row, col];
                if (piece == null)
                    sb.Append(".  ");
                else
                    sb.Append(piece.GetDebugCode()).Append(' ');
            }
            sb.AppendLine();
        }
        sb.AppendLine("    a  b  c  d  e  f  g  h");
        return sb.ToString();
    }
}