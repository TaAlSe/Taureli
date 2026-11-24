using Core.Moves;

namespace Tests.Core.Moves;

public class MoveTests
{
    /// <summary>
    /// Конструктор Move сохраняет переданные позиции From и To.
    /// </summary>
    [Fact]
    public void Move_StoresCorrectPositions()
    {
        var from = new Position(1, 0);
        var to = new Position(3, 0);
        var move = new Move(from, to);

        Assert.Equal(from, move.From);
        Assert.Equal(to, move.To);
    }

    // Move_WithPromotion_StoresPromotionType — удалить.
    // Проверка превращения теперь в PromotionTests.Ratnik_ReachesLastRank_PromotesToTarget
    // и PromotionTests.Ratnik_PromotesToCorrectTargetForEachFile.
}