namespace Core.Pieces;

/// <summary>
/// Метод расширения для получения противоположного цвета.
/// </summary>
public static class PieceColorExtensions
{
    public static PieceColor Opposite(this PieceColor color) =>
        color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}