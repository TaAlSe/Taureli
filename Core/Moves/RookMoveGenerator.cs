using Core.Board;

namespace Core.Moves;

public static class RookMoveGenerator
{
    /// <summary>
    /// Возвращает все возможные ходы ладьи (ратоборца) из заданной позиции.
    /// </summary>
    public static List<Position> GetMoves(ChessBoard board, Position from) => 
        LinearMoveGenerator.GetMoves(board, from, Directions.RookDirections);
}