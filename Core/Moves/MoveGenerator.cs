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
        var piece = board.GetTopPiece(from);
        if (piece == null) return [];

        // Специальные фигуры (нелинейные)
        if (piece.Type == PieceType.Horseman)
        {
            var positions = LinearMoveGenerator.GetMoves(board, from, Directions.KnightDirections, 1);
            return [.. positions.Select(pos => new Move(from, pos))];
        }

        if (piece.Type == PieceType.Helgi)
        {
            var queenPositions = LinearMoveGenerator.GetMoves(board, from, Directions.QueenDirections);
            var knightPositions = LinearMoveGenerator.GetMoves(board, from, Directions.KnightDirections, 1);
            var allPositions = queenPositions.Concat(knightPositions).ToList();
            return [.. allPositions.Select(pos => new Move(from, pos))];
        }

        if (piece.Type == PieceType.Ratnik)
        {
            // Пешка возвращает Move, а не Position
            return PawnMoveGenerator.GetMoves(board, from);
        }

        if (piece.Type == PieceType.Volkhv)
        {
            var positions = LinearMoveGenerator.GetMoves(board, from, Directions.QueenDirections, 1);
            var moves = positions.Select(pos => new Move(from, pos)).ToList();
            moves.AddRange(CastlingMoveGenerator.GetCastlingMoves(board, piece.Color));
            return moves;
        }

        // Линейные фигуры (ладья, слон, князь)
        if (_directionsMap.TryGetValue(piece.Type, out var directions))
        {
            var positions = LinearMoveGenerator.GetMoves(board, from, directions);
            return [.. positions.Select(pos => new Move(from, pos))];
        }

        // Если на клетке башня (больше 1 фигуры), добавляем ходы с разделением
        var stack = board.GetStack(from);
        if (stack != null && stack.Count > 1)
        {
            var additionalMoves = new List<Move>();
            foreach (var move in moves)
            {
                // Для каждого хода добавляем варианты с разделением (от 1 до stack.Count-1)
                for (int k = 1; k < stack.Count; k++)
                {
                    // Создаём копию хода с SplitCount = k
                    var splitMove = new Move(move.From, move.To, move.PromotionType)
                    {
                        IsCastling = move.IsCastling,
                        CastlingSide = move.CastlingSide,
                        SplitCount = k
                    };
                    additionalMoves.Add(splitMove);
                }
            }
            moves.AddRange(additionalMoves);
        }

        return moves;
    }
}