// Tests/Core/Board/PromotionTests.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Board;

public class PromotionTests
{
    /// <summary>
    /// Хелпер: создаёт доску с двумя королями (чтобы IsLegalMove работал).
    /// </summary>
    private static ChessBoard BoardWithKings()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.UpdateKingPosition(PieceColor.White, new Position(0, 4));
        board.SetPiece(new Position(7, 4), new Piece(PieceType.Volkhv, PieceColor.Black));
        board.UpdateKingPosition(PieceColor.Black, new Position(7, 4));
        return board;
    }

    /// <summary>
    /// Каждая пешка в начальной расстановке имеет PromotionTarget.
    /// </summary>
    [Fact]
    public void PieceFactory_AllPawnsHavePromotionTarget()
    {
        var position = PieceFactory.StartingPosition();
        for (int row = 0; row < 8; row++)
            for (int col = 0; col < 8; col++)
            {
                var piece = position[row, col];
                if (piece is { Type: PieceType.Ratnik })
                    Assert.True(piece.PromotionTarget.HasValue,
                        $"Ратник на ({row},{col}) без PromotionTarget");
            }
    }

    /// <summary>
    /// Пешка на последней горизонтали генерирует ход на неё (само превращение — при исполнении).
    /// </summary>
    [Fact]
    public void Pawn_OnLastRank_GeneratesMoveToLastRank()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(6, 4), new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi));

        var moves = PawnMoveGenerator.GetMoves(board, new Position(6, 4));

        Assert.Contains(moves, m => m.To == new Position(7, 4));
    }

    /// <summary>
    /// Ратник на последней горизонтали превращается в свой PromotionTarget.
    /// </summary>
    [Fact]
    public void Ratnik_ReachesLastRank_PromotesToTarget()
    {
        var board = BoardWithKings();
        board.SetPiece(new Position(6, 4), new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi));

        Assert.True(board.MakeMove(new Move(new Position(6, 4), new Position(7, 4))));

        var top = board.GetTopPiece(new Position(7, 4));
        Assert.NotNull(top);
        Assert.Equal(PieceType.Helgi, top.Type);
        Assert.True(top.IsPromoted);
        Assert.Equal(PieceType.Helgi, top.PromotionTarget);
    }

    /// <summary>
    /// Превращение сохраняет исходную линию: пешка с каждой вертикали даёт свою фигуру.
    /// </summary>
    [Fact]
    public void Ratnik_PromotesToCorrectTargetForEachFile()
    {
        // Ожидаемая цель для каждой вертикали (см. PieceFactory).
        var expected = new[]
        {
            PieceType.Ratoborets, PieceType.Horseman, PieceType.Archer, PieceType.Knyaz,
            PieceType.Helgi,      PieceType.Archer,   PieceType.Horseman, PieceType.Ratoborets
        };

        for (int col = 0; col < 8; col++)
        {
            var board = BoardWithKings();
            board.SetPiece(new Position(6, col),
                new Piece(PieceType.Ratnik, PieceColor.White, expected[col]));

            Assert.True(board.MakeMove(new Move(new Position(6, col), new Position(7, col))),
                $"Ход пешки с вертикали {col} не выполнен");

            var top = board.GetTopPiece(new Position(7, col));
            Assert.NotNull(top);
            Assert.Equal(expected[col], top.Type);
            Assert.True(top.IsPromoted);
        }
    }

    /// <summary>
    /// Пешка на вершине башни превращается, нижние фигуры остаются на месте.
    /// </summary>
    [Fact]
    public void Ratnik_OnTopOfStack_Promotes_StackBelowUnchanged()
    {
        var board = BoardWithKings();
        // На e7: снизу Ratoborets, сверху Ratnik.
        board.SetStack(new Position(6, 4), [
            new Piece(PieceType.Ratoborets, PieceColor.White),
            new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi)
        ]);

        // Ходим верхним Ratnik'ом (SplitCount = 1).
        Assert.True(board.MakeMove(
            new Move(new Position(6, 4), new Position(7, 4)) { SplitCount = 1 }));

        // На e7 остался Ratoborets.
        var fromStack = board.GetStack(new Position(6, 4));
        Assert.NotNull(fromStack);
        Assert.Single(fromStack);
        Assert.Equal(PieceType.Ratoborets, fromStack[0].Type);

        // На e8 — превращённый Helgi.
        var top = board.GetTopPiece(new Position(7, 4));
        Assert.NotNull(top);
        Assert.Equal(PieceType.Helgi, top.Type);
        Assert.True(top.IsPromoted);
    }

    /// <summary>
    /// Ратник, снятый с башни и дошедший до последней горизонтали, превращается.
    /// </summary>
    [Fact]
    public void Ratnik_SplitFromStack_Promotes()
    {
        var board = BoardWithKings();
        board.SetStack(new Position(6, 4), [
            new Piece(PieceType.Ratoborets, PieceColor.White),
            new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi)
        ]);

        Assert.True(board.MakeMove(
            new Move(new Position(6, 4), new Position(7, 4)) { SplitCount = 1 }));

        var top = board.GetTopPiece(new Position(7, 4));
        Assert.NotNull(top);
        Assert.Equal(PieceType.Helgi, top.Type);
        Assert.True(top.IsPromoted);
    }

    /// <summary>
    /// Своя башня встаёт на превращённую фигуру → она откатывается в Ratnik'а.
    /// </summary>
    [Fact]
    public void PromotedPiece_CoveredByFriendlyStack_RevertsToRatnik()
    {
        var board = BoardWithKings();
        // На e8 — превращённый Helgi.
        board.SetPiece(new Position(7, 4), new Piece(PieceType.Helgi, PieceColor.White, PieceType.Helgi)
        {
            IsPromoted = true,
            HasMoved = true
        });
        // На e7 — своя башня, которая на него встанет.
        board.SetPiece(new Position(6, 4), new Piece(PieceType.Ratoborets, PieceColor.White));

        Assert.True(board.MakeMove(new Move(new Position(6, 4), new Position(7, 4))));

        var stack = board.GetStack(new Position(7, 4));
        Assert.NotNull(stack);
        Assert.Equal(2, stack.Count);
        // Снизу — откаченный Ratnik, сверху — Ratoborets.
        Assert.Equal(PieceType.Ratnik, stack[0].Type);
        Assert.False(stack[0].IsPromoted);
        Assert.Equal(PieceType.Helgi, stack[0].PromotionTarget);
        Assert.True(stack[0].HasMoved);
        Assert.Equal(PieceType.Ratoborets, stack[1].Type);
    }

    /// <summary>
    /// Вражеская башня встаёт на превращённую фигуру → она тоже откатывается в Ratnik'а.
    /// </summary>
    [Fact]
    public void PromotedPiece_CoveredByEnemyStack_RevertsToRatnik()
    {
        var board = BoardWithKings();
        // На a1 — превращённый белый Ratnik (например, Archer с вертикали a).
        board.SetPiece(new Position(0, 0), new Piece(PieceType.Archer, PieceColor.White, PieceType.Ratoborets)
        {
            IsPromoted = true,
            HasMoved = true
        });
        // Чёрный Ratnik на b2 бьёт a1 по диагонали вниз-влево.
        board.SetPiece(new Position(1, 1), new Piece(PieceType.Ratnik, PieceColor.Black, PieceType.Ratoborets));

        Assert.True(board.MakeMove(new Move(new Position(1, 1), new Position(0, 0))));

        var stack = board.GetStack(new Position(0, 0));
        Assert.NotNull(stack);
        Assert.Equal(2, stack.Count);
        // Снизу — откаченный белый Ratnik, сверху — чёрный Ratnik.
        Assert.Equal(PieceType.Ratnik, stack[0].Type);
        Assert.Equal(PieceColor.White, stack[0].Color);
        Assert.False(stack[0].IsPromoted);
        Assert.Equal(PieceType.Ratoborets, stack[0].PromotionTarget);
        Assert.True(stack[0].HasMoved);
        Assert.Equal(PieceType.Ratnik, stack[1].Type);
        Assert.Equal(PieceColor.Black, stack[1].Color);
    }

    /// <summary>
    /// Рятник, уже ходивший, не имеет права на двойной ход с начальной горизонтали.
    /// </summary>
    [Fact]
    public void Ratnik_WithHasMoved_DoesNotGenerateDoubleMove()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(1, 4),
            new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi) { HasMoved = true });

        var moves = PawnMoveGenerator.GetMoves(board, new Position(1, 4));

        Assert.DoesNotContain(moves, m => m.To == new Position(3, 4));
        Assert.Contains(moves, m => m.To == new Position(2, 4)); // одиночный ход остаётся
    }
}