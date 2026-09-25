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
    /// Находит позицию короля указанного цвета на доске (использует кэшированные позиции).
    /// </summary>
    public Position? FindKing(PieceColor color)
    {
        return color == PieceColor.White ? WhiteKingPosition : BlackKingPosition;
    }

}
