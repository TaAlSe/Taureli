using Core.Board;
using Core.Pieces;

namespace Core.Moves;

/// <summary>
/// Единый генератор ходов для всех фигур, использующий словарь направлений.
/// </summary>
public static class MoveGenerator
{
    private static readonly Dictionary<PieceType, (int dRow, int dCol)[]> _directionsMap = new()
    {
        [PieceType.Ratoborets] = Directions.RookDirections,
        [PieceType.Archer] = Directions.BishopDirections,
        [PieceType.Knyaz] = Directions.QueenDirections,
        // В будущем: Horseman, Helgi, Volkhv, Ratnik
    };

    /// <summary>
    /// Возвращает все возможные ходы для фигуры, стоящей на указанной позиции.
    /// </summary>
    /// <param name="board">Текущая доска.</param>
    /// <param name="from">Позиция фигуры.</param>
    /// <returns>Список доступных позиций для хода.</returns>
    public static List<Position> GetMovesForPiece(ChessBoard board, Position from)
    {
        var piece = board.GetPiece(from.Row, from.Col);
        if (piece == null) return [];

        if (_directionsMap.TryGetValue(piece.Type, out var directions))
            return LinearMoveGenerator.GetMoves(board, from, directions);

        return []; // Для фигур без направлений (пока)
    }
}