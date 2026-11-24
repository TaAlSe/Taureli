// Tests/Core/Board/PromotionTests.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Board;

public class PromotionTests
{
    /// <summary>
    /// Хелпер: доска с двумя королями в безопасных местах.
    /// Чёрный король в центре, чтобы не мешать превращению на 8-й горизонтали.
    /// </summary>
    private static ChessBoard BoardWithKings()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.UpdateKingPosition(PieceColor.White, new Position(0, 4));
        board.SetPiece(new Position(3, 4), new Piece(PieceType.Volkhv, PieceColor.Black));
        board.UpdateKingPosition(PieceColor.Black, new Position(3, 4));
        return board;
    }

    /// <summary>Каждая пешка в начальной расстановке имеет PromotionTarget.</summary>
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

    /// <summary>Ратник на последней горизонтали превращается в свой PromotionTarget.</summary>
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

    /// <summary>Пешка с каждой вертикали превращается в свою фигуру.</summary>
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

    /// <summary>Пешка на вершине башни превращается, нижние фигуры остаются.</summary>
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

    /// <summary>Ратник, снятый с башни и дошедший до последней горизонтали, превращается.</summary>
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
    /// Верхний Ratnik башни идёт вперёд (split) на своего превращённого Helgi.
    /// Покрытый Helgi откатывается в Ratnik'а; пришедший Ratnik превращается.
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
        // На e7 — башня [Ratoborets (bottom), Ratnik (top)].
        board.SetStack(new Position(6, 4), [
            new Piece(PieceType.Ratoborets, PieceColor.White),
            new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi)
        ]);

        // Верхний Ratnik идёт на e8, на своего Helgi.
        Assert.True(board.MakeMove(
            new Move(new Position(6, 4), new Position(7, 4)) { SplitCount = 1 }));

        var stack = board.GetStack(new Position(7, 4));
        Assert.NotNull(stack);
        Assert.Equal(2, stack.Count);
        // Снизу — откаченный Ratnik (бывший Helgi).
        Assert.Equal(PieceType.Ratnik, stack[0].Type);
        Assert.False(stack[0].IsPromoted);
        Assert.Equal(PieceType.Helgi, stack[0].PromotionTarget);
        Assert.True(stack[0].HasMoved);
        // Сверху — Ratnik, только что превращённый в Helgi.
        Assert.Equal(PieceType.Helgi, stack[1].Type);
        Assert.True(stack[1].IsPromoted);

        // На e7 остался Ratoborets.
        var fromStack = board.GetStack(new Position(6, 4));
        Assert.NotNull(fromStack);
        Assert.Single(fromStack);
        Assert.Equal(PieceType.Ratoborets, fromStack[0].Type);
    }

    /// <summary>
    /// Вражеский ратник берёт превращённую фигуру на не-последней горизонтали:
    /// покрытая фигура откатывается в Ratnik'а, обе — ратники в башне.
    /// </summary>
    [Fact]
    public void PromotedPiece_CoveredByEnemyStack_RevertsToRatnik()
    {
        var board = BoardWithKings();
        // На a2 — превращённый White Archer (IsPromoted = true). a2 = (1,0).
        // Строка 1 — не последняя для чёрных (у них последняя — 0), превращения не будет.
        board.SetPiece(new Position(1, 0), new Piece(PieceType.Archer, PieceColor.White, PieceType.Ratoborets)
        {
            IsPromoted = true,
            HasMoved = true
        });
        // Чёрный Ratnik на b3 = (2,1) бьёт a2 по диагонали вниз-влево.
        board.SetPiece(new Position(2, 1),
            new Piece(PieceType.Ratnik, PieceColor.Black, PieceType.Ratoborets));

        Assert.True(board.MakeMove(new Move(new Position(2, 1), new Position(1, 0))));

        var stack = board.GetStack(new Position(1, 0));
        Assert.NotNull(stack);
        Assert.Equal(2, stack.Count);
        // Снизу — откаченный белый Ratnik.
        Assert.Equal(PieceType.Ratnik, stack[0].Type);
        Assert.Equal(PieceColor.White, stack[0].Color);
        Assert.False(stack[0].IsPromoted);
        Assert.Equal(PieceType.Ratoborets, stack[0].PromotionTarget);
        Assert.True(stack[0].HasMoved);
        // Сверху — чёрный Ratnik (не на последней горизонтали, не превращается).
        Assert.Equal(PieceType.Ratnik, stack[1].Type);
        Assert.Equal(PieceColor.Black, stack[1].Color);
    }

    /// <summary>Ратник с HasMoved = true не имеет права на двойной ход.</summary>
    [Fact]
    public void Ratnik_WithHasMoved_DoesNotGenerateDoubleMove()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(1, 4),
            new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi) { HasMoved = true });

        var moves = PawnMoveGenerator.GetMoves(board, new Position(1, 4));

        Assert.DoesNotContain(moves, m => m.To == new Position(3, 4));
        Assert.Contains(moves, m => m.To == new Position(2, 4));
    }
}