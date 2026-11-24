

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Board;

public class PromotionTests
{
    /// <summary>
    /// Проверяет, что пешка на последней горизонтали генерирует ход на неё.
    /// Само превращение выполняется при исполнении хода (см. Ratnik_ReachesLastRank_PromotesToTarget).
    /// </summary>
    [Fact]
    public void Pawn_OnLastRank_GeneratesMoveToLastRank()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(6, 4), new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi));

        var moves = PawnMoveGenerator.GetMoves(board, new Position(6, 4));

        Assert.Contains(moves, m => m.To == new Position(7, 4));
    }
}
