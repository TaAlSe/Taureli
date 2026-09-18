using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class PawnMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что белая пешка с начальной позиции (e2) может пойти на одну клетку (e3)
    /// и на две клетки (e4), если обе клетки свободны.
    /// </summary>
    [Fact]
    public void WhitePawn_FromStart_CanMoveOneOrTwoSquaresForward()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(1, 4, pawn); // e2

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(1, 4));
        // Ожидаем: e3 и e4
        Assert.Equal(2, moves.Count);
        Assert.Contains(new Move(new Position(1, 4), new Position(2, 4)), moves);
        Assert.Contains(new Move(new Position(1, 4), new Position(3, 4)), moves);
    }

    /// <summary>
    /// Проверяет, что если клетка перед пешкой занята, то пешка не может пойти вперёд,
    /// и двойной ход также невозможен (из-за блокировки промежуточной клетки).
    /// </summary>
    [Fact]
    public void WhitePawn_Blocked_CanOnlyMoveOneOrNone()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(1, 4, pawn);
        board.SetPiece(2, 4, new Piece(PieceType.Ratnik, PieceColor.Black)); // e3 занято

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(1, 4));
        // Должен быть только ход на e4 (двойной), т.к. e3 занято, но e4 свободно?
        // На самом деле, если e3 занято, то двойной ход невозможен, потому что промежуточная клетка занята.
        Assert.Empty(moves);
    }

    /// <summary>
    /// Проверяет, что белая пешка может взять вражескую фигуру по диагонали влево и вправо,
    /// при этом сохраняя возможность обычного хода вперёд и двойного хода.
    /// </summary>
    [Fact]
    public void WhitePawn_CanCaptureDiagonally()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(1, 4, pawn);
        board.SetPiece(2, 3, new Piece(PieceType.Horseman, PieceColor.Black)); // d3
        board.SetPiece(2, 5, new Piece(PieceType.Archer, PieceColor.Black));  // f3

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(1, 4));
        // Ходы: e3 (вперёд), e4 (двойной), d3 (взятие), f3 (взятие) — итого 4
        Assert.Equal(4, moves.Count);
        Assert.Contains(new Move(new Position(1, 4), new Position(2, 3)), moves);
        Assert.Contains(new Move(new Position(1, 4), new Position(2, 5)), moves);
        Assert.Contains(new Move(new Position(1, 4), new Position(2, 4)), moves);
        Assert.Contains(new Move(new Position(1, 4), new Position(3, 4)), moves);
    }

    /// <summary>
    /// Проверяет, что белая пешка на предпоследней горизонтали (e7) при ходе вперёд
    /// создаёт ход с превращением (PromotionType = null, так как выбор фигуры отложен).
    /// </summary>
    [Fact]
    public void WhitePawn_OnPromotionRank_ReturnsMoveWithPromotion()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(6, 4, pawn); // e7

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(6, 4));
        // Ход на e8 (превращение)
        Assert.Single(moves);
        var move = moves[0];
        Assert.Equal(new Position(7, 4), move.To);
        Assert.Null(move.PromotionType); // пока null
    }

    /// <summary>
    /// Проверяет, что чёрная пешка движется вниз (уменьшение row) с начальной позиции e7
    /// и имеет ходы на e6 и e5 (двойной ход).
    /// </summary>
    [Fact]
    public void BlackPawn_MovesDownward()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.Black);
        board.SetPiece(6, 4, pawn); // e7

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(6, 4));
        // Ожидаем: e6 и e5
        Assert.Equal(2, moves.Count);
        Assert.Contains(new Move(new Position(6, 4), new Position(5, 4)), moves);
        Assert.Contains(new Move(new Position(6, 4), new Position(4, 4)), moves);
    }

    /// <summary>
    /// Проверяет, что пешка не на последней горизонтали не генерирует ход с превращением.
    /// </summary>
    [Fact]
    public void Pawn_NotOnPromotionRank_DoesNotGeneratePromotion()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi);
        board.SetPiece(new Position(3, 4), pawn); // e4
        var moves = PawnMoveGenerator.GetMoves(board, new Position(3, 4));
        var move = moves.First(m => m.To == new Position(4, 4)); // e5
        Assert.Null(move.PromotionType);
    }

    /// <summary>
    /// Проверяет, что пешка на последней горизонтали генерирует ход с превращением в указанный тип.
    /// </summary>
    [Fact]
    public void Pawn_OnPromotionRank_GeneratesPromotionWithTarget()
    {
        var board = new ChessBoard();
        var pawn = new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi);
        board.SetPiece(new Position(6, 4), pawn); // e7
        var moves = PawnMoveGenerator.GetMoves(board, new Position(6, 4));
        var move = moves.First(m => m.To == new Position(7, 4)); // e8
        Assert.Equal(PieceType.Helgi, move.PromotionType);
    }
}