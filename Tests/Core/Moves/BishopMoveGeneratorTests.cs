// Tests/Core/Moves/BishopMoveGeneratorTests.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class BishopMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что слон на пустой доске ходит по всем диагоналям.
    /// </summary>
    [Fact]
    public void Bishop_OnEmptyBoard_HasCorrectMoves()
    {
        var board = new ChessBoard();
        var bishop = new Piece(PieceType.Archer, PieceColor.White);
        board.SetPiece(3, 3, bishop); // d4

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));

        // На d4: диагонали дают: вверх-вправо: (4,4),(5,5),(6,6),(7,7) — 4
        // вверх-влево: (4,2),(5,1),(6,0) — 3
        // вниз-вправо: (2,4),(1,5),(0,6) — 3
        // вниз-влево: (2,2),(1,1),(0,0) — 3
        // Итого: 4+3+3+3 = 13
        Assert.Equal(13, moves.Count);

        // Проверим крайние точки
        Assert.Contains(moves, m => m.To == new Position(7, 7));
        Assert.Contains(moves, m => m.To == new Position(0, 0));
        Assert.Contains(moves, m => m.To == new Position(6, 0));
        Assert.Contains(moves, m => m.To == new Position(0, 6));
    }

    /// <summary>
    /// Проверяет, что слон останавливается перед своей фигурой.
    /// </summary>
    [Fact]
    public void Bishop_BlockedByOwnPiece_StopsBeforeIt()
    {
        var board = new ChessBoard();
        var bishop = new Piece(PieceType.Archer, PieceColor.White);
        board.SetPiece(3, 3, bishop); // d4
        var ownPiece = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(5, 5, ownPiece); // f6 — своя пешка на диагонали

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));

        // Должен дойти до (4,4), но не до (5,5) и дальше
        Assert.Contains(moves, m => m.To == new Position(4, 4));
        Assert.DoesNotContain(moves, m => m.To == new Position(5, 5));
        Assert.DoesNotContain(moves, m => m.To == new Position(6, 6));
        Assert.DoesNotContain(moves, m => m.To == new Position(7, 7));
    }

    /// <summary>
    /// Проверяет, что слон может взять вражескую фигуру, но дальше не идёт.
    /// </summary>
    [Fact]
    public void Bishop_CanCaptureEnemyPiece_ButStopsAfterCapture()
    {
        var board = new ChessBoard();
        var bishop = new Piece(PieceType.Archer, PieceColor.White);
        board.SetPiece(3, 3, bishop); // d4
        var enemyPiece = new Piece(PieceType.Horseman, PieceColor.Black);
        board.SetPiece(5, 5, enemyPiece); // f6

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));

        // Может взять на (5,5)
        Assert.Contains(moves, m => m.To == new Position(5, 5));
        // Но не дальше
        Assert.DoesNotContain(moves, m => m.To == new Position(6, 6));
        Assert.DoesNotContain(moves, m => m.To == new Position(7, 7));
    }

    /// <summary>
    /// Проверяет, что слон не генерирует ходы, если на клетке нет фигуры.
    /// </summary>
    [Fact]
    public void Bishop_FromEmptySquare_ReturnsEmptyList()
    {
        var board = new ChessBoard();
        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));
        Assert.Empty(moves);
    }
}