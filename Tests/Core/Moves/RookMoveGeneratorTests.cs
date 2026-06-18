using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class RookMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что ладья на пустой доске может пройти все клетки по вертикали и горизонтали.
    /// </summary>
    [Fact]
    public void Rook_OnEmptyBoard_HasCorrectMoves()
    {
        var board = new ChessBoard();
        var rook = new Piece(PieceType.Ratoborets, PieceColor.White);
        board.SetPiece(3, 3, rook); // d4

        var moves = RookMoveGenerator.GetMoves(board, new Position(3, 3));

        // Ожидаем 14 ходов: 4 вверх + 3 вниз + 4 вправо + 3 влево
        Assert.Equal(14, moves.Count);
        // Проверим наличие некоторых: например, (3,7) и (3,0)
        Assert.Contains(new Position(3, 7), moves);
        Assert.Contains(new Position(3, 0), moves);
        Assert.Contains(new Position(0, 3), moves);
        Assert.Contains(new Position(7, 3), moves);
    }

    /// <summary>
    /// Проверяет, что ладья не может перепрыгнуть через свою фигуру.
    /// </summary>
    [Fact]
    public void Rook_BlockedByOwnPiece_StopsBeforeIt()
    {
        var board = new ChessBoard();
        var rook = new Piece(PieceType.Ratoborets, PieceColor.White);
        board.SetPiece(3, 3, rook); // d4
        var ownPiece = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(3, 5, ownPiece); // f4 — своя пешка

        var moves = RookMoveGenerator.GetMoves(board, new Position(3, 3));

        // Вправо должны дойти до col=4 (e4), но не дальше
        Assert.Contains(new Position(3, 4), moves);
        Assert.DoesNotContain(new Position(3, 5), moves);
        Assert.DoesNotContain(new Position(3, 6), moves);
        Assert.DoesNotContain(new Position(3, 7), moves);
        // Влево, вверх, вниз — без изменений
        Assert.Contains(new Position(3, 2), moves);
        Assert.Contains(new Position(3, 1), moves);
        Assert.Contains(new Position(3, 0), moves);
        Assert.Contains(new Position(2, 3), moves);
        Assert.Contains(new Position(1, 3), moves);
        Assert.Contains(new Position(0, 3), moves);
        Assert.Contains(new Position(4, 3), moves);
        Assert.Contains(new Position(5, 3), moves);
        Assert.Contains(new Position(6, 3), moves);
        Assert.Contains(new Position(7, 3), moves);
    }

    /// <summary>
    /// Проверяет, что ладья может взять вражескую фигуру, но не может пройти дальше.
    /// </summary>
    [Fact]
    public void Rook_CanCaptureEnemyPiece_ButStopsAfterCapture()
    {
        var board = new ChessBoard();
        var rook = new Piece(PieceType.Ratoborets, PieceColor.White);
        board.SetPiece(3, 3, rook); // d4
        var enemyPiece = new Piece(PieceType.Horseman, PieceColor.Black);
        board.SetPiece(3, 5, enemyPiece); // f4 — вражеский конь

        var moves = RookMoveGenerator.GetMoves(board, new Position(3, 3));

        // Должен быть ход на f4 (взять коня)
        Assert.Contains(new Position(3, 5), moves);
        // Но не дальше
        Assert.DoesNotContain(new Position(3, 6), moves);
        Assert.DoesNotContain(new Position(3, 7), moves);
    }

    /// <summary>
    /// Проверяет, что ладья не генерирует ходы, если на начальной клетке нет фигуры.
    /// </summary>
    [Fact]
    public void Rook_FromEmptySquare_ReturnsEmptyList()
    {
        var board = new ChessBoard();
        var moves = RookMoveGenerator.GetMoves(board, new Position(3, 3));
        Assert.Empty(moves);
    }
}