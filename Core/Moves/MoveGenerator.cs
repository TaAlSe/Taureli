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
    };

    /// <summary>
    /// Возвращает все возможные ходы для фигуры, стоящей на указанной позиции.
    /// </summary>
    /// <param name="board">Текущая доска.</param>
    /// <param name="from">Позиция фигуры.</param>
    /// <returns>Список доступных позиций для хода.</returns>
    public static List<Move> GetMovesForPiece(ChessBoard board, Position from)
    {
        var piece = board.GetPiece(from.Row, from.Col);
        if (piece == null) return [];

        // Специальные фигуры (нелинейные)
        if (piece.Type == PieceType.Horseman)
            return [.. KnightMoveGenerator.GetMoves(board, from).Select(pos => new Move(from, pos))];

        if (piece.Type == PieceType.Helgi)
        {
            var queenMoves = LinearMoveGenerator.GetMoves(board, from, Directions.QueenDirections);
            var knightMoves = KnightMoveGenerator.GetMoves(board, from);
            var allPositions = queenMoves.Concat(knightMoves).ToList();
            return [.. allPositions.Select(pos => new Move(from, pos))];
        }

        if (piece.Type == PieceType.Ratnik)
        {
            // Пешка возвращает Move, а не Position
            return PawnMoveGenerator.GetMoves(board, from);
        }

        // Линейные фигуры (ладья, слон, князь)
        if (_directionsMap.TryGetValue(piece.Type, out var directions))
        {
            var positions = LinearMoveGenerator.GetMoves(board, from, directions);
            return [.. positions.Select(pos => new Move(from, pos))];
        }

        return [];
    }
}