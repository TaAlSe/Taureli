// Tests/Core/Moves/CastlingMoveGeneratorTests.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class CastlingMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что белые могут сделать короткую рокировку при соблюдении всех условий.
    /// </summary>
    [Fact]
    public void WhiteKingSideCastling_Allowed_WhenAllConditionsMet()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(0, 7), new Piece(PieceType.Ratoborets, PieceColor.White));
        // Клетки f1, g1 свободны

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.White);
        Assert.Single(moves);
        var castling = moves[0];
        Assert.True(castling.IsCastling);
        Assert.Equal(CastlingSide.KingSide, castling.CastlingSide);
        Assert.Equal(new Position(0, 4), castling.From);
        Assert.Equal(new Position(0, 6), castling.To);
    }

    /// <summary>
    /// Проверяет, что рокировка невозможна, если король уже двигался.
    /// </summary>
    [Fact]
    public void WhiteKingSideCastling_NotAllowed_WhenKingMoved()
    {
        var board = new ChessBoard(
            whiteCanCastleKingSide: false,
            whiteCanCastleQueenSide: false
        );
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(0, 7), new Piece(PieceType.Ratoborets, PieceColor.White));

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.White);
        Assert.Empty(moves);
    }

    /// <summary>
    /// Проверяет, что рокировка невозможна, если ладья уже двигалась.
    /// </summary>
    [Fact]
    public void WhiteKingSideCastling_NotAllowed_WhenRookMoved()
    {
        var board = new ChessBoard(
            whiteCanCastleKingSide: false // только короткая сброшена
        );
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(0, 7), new Piece(PieceType.Ratoborets, PieceColor.White));

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.White);
        Assert.Empty(moves);
    }

    /// <summary>
    /// Проверяет, что рокировка невозможна, если поле, которое король пересекает, атаковано.
    /// </summary>
    [Fact]
    public void WhiteKingSideCastling_NotAllowed_WhenSquareAttacked()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(0, 7), new Piece(PieceType.Ratoborets, PieceColor.White));
        // Чёрный слон на g3 атакует g1 (0,6)
        board.SetPiece(new Position(2, 6), new Piece(PieceType.Archer, PieceColor.Black));

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.White);
        Assert.Empty(moves);
    }

    /// <summary>
    /// Проверяет, что белые могут сделать длинную рокировку при соблюдении условий.
    /// </summary>
    [Fact]
    public void WhiteQueenSideCastling_Allowed_WhenAllConditionsMet()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(0, 0), new Piece(PieceType.Ratoborets, PieceColor.White));
        // Клетки b1, c1, d1 свободны

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.White);
        Assert.Single(moves);
        var castling = moves[0];
        Assert.True(castling.IsCastling);
        Assert.Equal(CastlingSide.QueenSide, castling.CastlingSide);
        Assert.Equal(new Position(0, 4), castling.From);
        Assert.Equal(new Position(0, 2), castling.To);
    }

    /// <summary>
    /// Проверяет, что чёрные могут сделать короткую рокировку.
    /// </summary>
    [Fact]
    public void BlackKingSideCastling_Allowed_WhenAllConditionsMet()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(7, 4), new Piece(PieceType.Volkhv, PieceColor.Black));
        board.SetPiece(new Position(7, 7), new Piece(PieceType.Ratoborets, PieceColor.Black));

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.Black);
        Assert.Single(moves);
        var castling = moves[0];
        Assert.Equal(CastlingSide.KingSide, castling.CastlingSide);
        Assert.Equal(new Position(7, 4), castling.From);
        Assert.Equal(new Position(7, 6), castling.To);
    }

    /// <summary>
    /// Проверяет, что рокировка не генерируется, если нет короля на начальной позиции.
    /// </summary>
    [Fact]
    public void NoCastling_WhenKingNotAtHome()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), null); // убрали короля
        board.SetPiece(new Position(0, 7), new Piece(PieceType.Ratoborets, PieceColor.White));

        var moves = CastlingMoveGenerator.GetCastlingMoves(board, PieceColor.White);
        Assert.Empty(moves);
    }
}