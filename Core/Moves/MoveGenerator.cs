// Core/Moves/MoveGenerator.cs

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
    /// <returns>Список доступных ходов.</returns>
    public static List<Move> GetMovesForPiece(ChessBoard board, Position from)
    {
        var piece = board.GetTopPiece(from);
        if (piece == null) return [];

        var moves = new List<Move>();

        // Генерация ходов в зависимости от типа фигуры
        switch (piece.Type)
        {
            case PieceType.Horseman:
                {
                    var positions = LinearMoveGenerator.GetMoves(board, from, Directions.KnightDirections, 1);
                    moves.AddRange(positions.Select(pos => new Move(from, pos)));
                    break;
                }
            case PieceType.Helgi:
                {
                    var queenPositions = LinearMoveGenerator.GetMoves(board, from, Directions.QueenDirections);
                    var knightPositions = LinearMoveGenerator.GetMoves(board, from, Directions.KnightDirections, 1);
                    var allPositions = queenPositions.Concat(knightPositions).ToList();
                    moves.AddRange(allPositions.Select(pos => new Move(from, pos)));
                    break;
                }
            case PieceType.Ratnik:
                {
                    moves.AddRange(PawnMoveGenerator.GetMoves(board, from));
                    break;
                }
            case PieceType.Volkhv:
                {
                    var positions = LinearMoveGenerator.GetMoves(board, from, Directions.QueenDirections, 1);
                    moves.AddRange(positions.Select(pos => new Move(from, pos)));
                    moves.AddRange(CastlingMoveGenerator.GetCastlingMoves(board, piece.Color));
                    break;
                }
            default:
                {
                    // Линейные фигуры (ладья, слон, князь)
                    if (_directionsMap.TryGetValue(piece.Type, out var directions))
                    {
                        var positions = LinearMoveGenerator.GetMoves(board, from, directions);
                        moves.AddRange(positions.Select(pos => new Move(from, pos)));
                    }
                    break;
                }
        }

        // Добавляем ходы с разделением для башен (если на клетке больше одной фигуры)
        var stack = board.GetStack(from);
        if (stack != null && stack.Count > 1)
        {
            var additionalMoves = new List<Move>();
            foreach (var move in moves)
            {
                // Пропускаем рокировку и превращение (они не могут быть разделены)
                if (move.IsCastling || move.PromotionType.HasValue)
                    continue;

                for (int k = 1; k < stack.Count; k++)
                {
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