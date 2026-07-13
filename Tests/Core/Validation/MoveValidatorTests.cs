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
        Assert.True(MoveValidator.IsLegalMove(board, move, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что ход, открывающий линию для атаки на короля, считается нелегальным.
    /// Белый король на e1, белая пешка на e2, чёрная ладья на e8.
    /// Ход пешки e2-e3 открывает вертикаль, ладья атакует короля → нелегально.
    /// </summary>
    [Fact]
    public void MoveThatExposesKingToCheck_IsIllegal()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(1, 4), new Piece(PieceType.Ratnik, PieceColor.White));
        board.SetPiece(new Position(7, 4), new Piece(PieceType.Ratoborets, PieceColor.Black));

        var move = new Move(new Position(1, 4), new Position(2, 4));
        Assert.False(MoveValidator.IsLegalMove(board, move, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что ход, который не оставляет короля под шахом, считается легальным.
    /// </summary>
    [Fact]
    public void MoveThatTakesAttacker_IsLegal()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var move = new Move(new Position(1, 4), new Position(3, 4));
        Assert.True(MoveValidator.IsLegalMove(board, move, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что GetLegalMoves возвращает только легальные ходы.
    /// В позиции, где пешка прикрывает короля от ладьи, все ходы пешки нелегальны.
    /// </summary>
    [Fact]
    public void GetLegalMoves_FiltersIllegalMoves()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(1, 4), new Piece(PieceType.Ratnik, PieceColor.White));
        board.SetPiece(new Position(7, 4), new Piece(PieceType.Ratoborets, PieceColor.Black));

        var legalMoves = MoveValidator.GetLegalMoves(board, new Position(1, 4), PieceColor.White);
        Assert.Empty(legalMoves);
    }
}