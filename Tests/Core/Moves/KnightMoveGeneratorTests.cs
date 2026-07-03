using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class KnightMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что всадник (конь) на пустой доске имеет ровно 8 возможных ходов
    /// и все они соответствуют правильному смещению (буквой Г).
    /// </summary>
    [Fact]
    public void Knight_OnEmptyBoard_HasCorrectMoves()
    {
        var board = new ChessBoard();
        var knight = new Piece(PieceType.Horseman, PieceColor.White);
        board.SetPiece(3, 3, knight); // d4

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));
        // На d4 конь имеет 8 ходов: b3, b5, c2, c6, e2, e6, f3, f5
        Assert.Equal(8, moves.Count);
        var expected = new[]
        {
            new Position(1, 2), new Position(1, 4),
            new Position(2, 1), new Position(2, 5),
            new Position(4, 1), new Position(4, 5),
            new Position(5, 2), new Position(5, 4)
        };
        foreach (var pos in expected)
            Assert.Contains(moves, m => m.To == pos);
    }

    /// <summary>
    /// Проверяет, что всадник может взять вражескую фигуру, но не может пойти на клетку,
    /// занятую своей фигурой.
    /// </summary>
    [Fact]
    public void Knight_CanCaptureEnemyPiece_ButNotOwn()
    {
        var board = new ChessBoard();
        var knight = new Piece(PieceType.Horseman, PieceColor.White);
        board.SetPiece(3, 3, knight);
        var enemy = new Piece(PieceType.Ratnik, PieceColor.Black);
        board.SetPiece(5, 4, enemy); // f5
        var own = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(1, 4, own); // b5 — своя

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));
        Assert.Contains(moves, m => m.To == new Position(5, 4)); // может взять врага
        Assert.DoesNotContain(moves, m => m.To == new Position(1, 4)); // не может на свою
    }

    /// <summary>
    /// Проверяет, что всадник в углу доски имеет только 2 возможных хода.
    /// </summary>
    [Fact]
    public void Knight_FromCorner_HasFewerMoves()
    {
        var board = new ChessBoard();
        board.SetPiece(0, 0, new Piece(PieceType.Horseman, PieceColor.White));
        var moves = MoveGenerator.GetMovesForPiece(board, new Position(0, 0));
        Assert.Equal(2, moves.Count); // только (1,2) и (2,1)
    }
}