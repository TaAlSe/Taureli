// Tests/Core/Moves/MoveTests.cs

using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class MoveTests
{
    /// <summary>
    /// Проверяет, что конструктор Move сохраняет переданные позиции From и To,
    /// а поле PromotionType по умолчанию равно null.
    /// </summary>
    [Fact]
    public void Move_StoresCorrectPositions()
    {
        var from = new Position(1, 0);
        var to = new Position(3, 0);
        var move = new Move(from, to);

        Assert.Equal(from, move.From);
        Assert.Equal(to, move.To);
        Assert.Null(move.PromotionType);
    }

    /// <summary>
    /// Проверяет, что при создании хода с указанием PromotionType это значение сохраняется.
    /// </summary>
    [Fact]
    public void Move_WithPromotion_StoresPromotionType()
    {
        var move = new Move(new Position(1, 0), new Position(0, 0), PieceType.Helgi);
        Assert.Equal(PieceType.Helgi, move.PromotionType);
    }
}