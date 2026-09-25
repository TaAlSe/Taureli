// Tests/Core/Moves/QueenMoveGeneratorTests.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class QueenMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что князь (ферзь) ходит как ладья + слон.
    /// </summary>
    [Fact]
    public void Queen_OnEmptyBoard_HasCorrectMoves()
    {
        var board = new ChessBoard();
        var queen = new Piece(PieceType.Knyaz, PieceColor.White);
        board.SetPiece(3, 3, queen);

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));
        // На пустой доске: ладья даёт 14 ходов, слон — 13, итого 27
        Assert.Equal(27, moves.Count);
        // Проверим крайние точки
        Assert.Contains(moves, m => m.To == new Position(7, 7)); // диагональ
        Assert.Contains(moves, m => m.To == new Position(7, 3)); // вертикаль
        Assert.Contains(moves, m => m.To == new Position(3, 7)); // горизонталь
    }
}
