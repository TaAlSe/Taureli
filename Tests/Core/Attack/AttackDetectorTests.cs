using Core.Attack;
using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Attack;

public class AttackDetectorTests
{
    /// <summary>
    /// Проверяет, что ладья (ратоборец) атакует клетки по вертикали и горизонтали.
    /// </summary>
    [Fact]
    public void Rook_AttacksAlongRowAndColumn()
    {
        var board = new ChessBoard();
        var rook = new Piece(PieceType.Ratoborets, PieceColor.White);
        board.SetPiece(3, 3, rook); // d4
        var target = new Position(3, 7); // h4
        Assert.True(AttackDetector.IsSquareAttacked(board, target, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что слон (лучник) атакует клетки по диагонали.
    /// </summary>
    [Fact]
    public void Bishop_AttacksAlongDiagonal()
    {
        var board = new ChessBoard();
        var bishop = new Piece(PieceType.Archer, PieceColor.White);
        board.SetPiece(3, 3, bishop);
        var target = new Position(7, 7); // h8
        Assert.True(AttackDetector.IsSquareAttacked(board, target, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что конь (всадник) атакует клетки буквой «Г» (8 возможных позиций).
    /// </summary>
    [Fact]
    public void Knight_AttacksSpecificSquares()
    {
        var board = new ChessBoard();
        var knight = new Piece(PieceType.Horseman, PieceColor.White);
        board.SetPiece(3, 3, knight);
        var target = new Position(5, 4); // f5
        Assert.True(AttackDetector.IsSquareAttacked(board, target, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что пешка (ратник) атакует диагонально вперёд (на одну клетку влево-вверх или вправо-вверх для белых,
    /// и влево-вниз или вправо-вниз для чёрных).
    /// </summary>
    [Fact]
    public void Pawn_AttacksDiagonallyForward()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(2, 3, pawn); // d3
        var target = new Position(3, 4); // e4
        Assert.True(AttackDetector.IsSquareAttacked(board, target, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что король (волхв) атакует все 8 соседних клеток.
    /// </summary>
    [Fact]
    public void King_AttacksAdjacentSquares()
    {
        var board = new ChessBoard();
        var king = new Piece(PieceType.Volkhv, PieceColor.White);
        board.SetPiece(3, 3, king);
        var target = new Position(4, 4); // e5
        Assert.True(AttackDetector.IsSquareAttacked(board, target, PieceColor.White));
    }

    /// <summary>
    /// Проверяет, что клетка, на которой нет фигур, не считается атакованной.
    /// </summary>
    [Fact]
    public void NotAttacked_WhenNoPiece()
    {
        var board = new ChessBoard();
        var target = new Position(4, 4);
        Assert.False(AttackDetector.IsSquareAttacked(board, target, PieceColor.White));
    }
}