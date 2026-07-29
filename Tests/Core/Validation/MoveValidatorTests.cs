using Core.Board;
using Core.Pieces;
using Core.Moves;
using Core.Validation;

namespace Tests.Core.Validation;

public class MoveValidatorTests
{
    /// <summary>
    /// Проверяет, что обычный ход пешки (e2-e3) легален, если не оставляет короля под шахом.
    /// </summary>
    [Fact]
    public void SimpleMove_NotLeavingKingInCheck_IsLegal()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var move = new Move(new Position(1, 4), new Position(2, 4));
        Assert.True(MoveValidator.IsLegalMove(board, move));
    }

    /// <summary>
    /// Проверяет, что ход пешки, который открывает диагональ для атаки на короля, считается нелегальным.
    /// Белый король на e1, белая пешка на f2, чёрный слон на h4.
    /// Ход f2-f3 убирает блокировку → нелегально.
    /// </summary>
    [Fact]
    public void MoveThatExposesKingToCheck_IsIllegal()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(1, 5), new Piece(PieceType.Ratnik, PieceColor.White));
        board.SetPiece(new Position(3, 7), new Piece(PieceType.Archer, PieceColor.Black));

        var move = new Move(new Position(1, 5), new Position(2, 5));
        Assert.False(MoveValidator.IsLegalMove(board, move));
    }

    /// <summary>
    /// Проверяет, что GetLegalMoves возвращает только легальные ходы.
    /// В позиции с пешкой f2, блокирующей слона h4, все ходы пешки нелегальны.
    /// </summary>
    [Fact]
    public void GetLegalMoves_FiltersIllegalMoves()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(1, 5), new Piece(PieceType.Ratnik, PieceColor.White));
        board.SetPiece(new Position(3, 7), new Piece(PieceType.Archer, PieceColor.Black));

        var legalMoves = MoveValidator.GetLegalMoves(board, new Position(1, 5));
        Assert.Empty(legalMoves);
    }

    /// <summary>
    /// Проверяет, что нелегальный ход (пешка горизонтально) не проходит валидацию.
    /// </summary>
    [Fact(Skip = "Временно пропущен, пока не адаптированы генераторы для башен")]
    public void IllegalPawnMove_Horizontal_IsNotLegal()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var illegalMove = new Move(new Position(1, 4), new Position(1, 5)); // e2-f2
        Assert.False(MoveValidator.IsLegalMove(board, illegalMove));
    }
}