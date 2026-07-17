using Core.Board;
using Core.Pieces;
using Core.Attack;

namespace Core.Moves;

/// <summary>
/// Генератор ходов рокировки.
/// </summary>
public static class CastlingMoveGenerator
{
    /// <summary>
    /// Возвращает список возможных рокировок для указанного цвета.
    /// </summary>
    public static List<Move> GetCastlingMoves(ChessBoard board, PieceColor color)
    {
        var moves = new List<Move>();
        int row = color == PieceColor.White ? 0 : 7;
        var kingPos = new Position(row, 4);
        var king = board.GetPiece(kingPos);
        if (king == null || king.Type != PieceType.Volkhv || king.Color != color)
            return moves;

        // Проверяем, не двигался ли король (по флагу)
        bool canCastleKingSide = color == PieceColor.White ? board.WhiteCanCastleKingSide : board.BlackCanCastleKingSide;
        bool canCastleQueenSide = color == PieceColor.White ? board.WhiteCanCastleQueenSide : board.BlackCanCastleQueenSide;

        if (!canCastleKingSide && !canCastleQueenSide)
            return moves;

        var opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;

        // Проверяем, не под шахом ли король
        if (AttackDetector.IsSquareAttacked(board, kingPos, opponentColor))
            return moves;

        // Короткая рокировка
        if (canCastleKingSide && CanCastle(board, row, kingPos, opponentColor, true))
            moves.Add(CreateCastlingMove(kingPos, row, 6, CastlingSide.KingSide));

        // Длинная рокировка
        if (canCastleQueenSide && CanCastle(board, row, kingPos, opponentColor, false))
            moves.Add(CreateCastlingMove(kingPos, row, 2, CastlingSide.QueenSide));

        return moves;
    }

    private static bool CanCastle(ChessBoard board, int row, Position kingPos, PieceColor opponentColor, bool kingSide)
    {
        int rookCol = kingSide ? 7 : 0;
        var rookPos = new Position(row, rookCol);
        var rook = board.GetPiece(rookPos);
        if (rook == null || rook.Type != PieceType.Ratoborets || rook.Color != kingPos.Color)
            return false;

        // Проверяем, свободны ли промежуточные клетки
        int startCol = kingSide ? 5 : 1;
        int endCol = kingSide ? 6 : 3;
        int direction = kingSide ? 1 : -1;

        for (int col = kingPos.Col + direction; col != rookCol; col += direction)
        {
            if (board.GetPiece(new Position(row, col)) != null)
                return false;
        }

        // Проверяем, не атакованы ли клетки, которые король пересекает или занимает
        // (король не должен проходить через битое поле)
        var checkPositions = new List<Position> { kingPos };
        if (kingSide)
        {
            checkPositions.Add(new Position(row, 5));
            checkPositions.Add(new Position(row, 6));
        }
        else
        {
            checkPositions.Add(new Position(row, 3));
            checkPositions.Add(new Position(row, 2));
        }

        foreach (var pos in checkPositions)
        {
            if (AttackDetector.IsSquareAttacked(board, pos, opponentColor))
                return false;
        }

        return true;
    }

    private static Move CreateCastlingMove(Position from, int row, int col, CastlingSide side)
    {
        return new Move(from, new Position(row, col), null)
        {
            IsCastling = true,
            CastlingSide = side
        };
    }
}

/// <summary>
/// Сторона рокировки.
/// </summary>
public enum CastlingSide
{
    KingSide,
    QueenSide
}