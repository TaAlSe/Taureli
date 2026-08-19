using Core.Moves;
using Core.Pieces;

namespace Core.Board;

public partial class ChessBoard
{
    /// <summary>
    /// Обновляет позицию короля указанного цвета.
    /// </summary>
    /// <param name="color">Цвет короля.</param>
    /// <param name="newPosition">Новая позиция (может быть null, если король снят с доски).</param>
    public void UpdateKingPosition(PieceColor color, Position? newPosition)
    {
        if (color == PieceColor.White)
            WhiteKingPosition = newPosition;
        else
            BlackKingPosition = newPosition;
    }
}
