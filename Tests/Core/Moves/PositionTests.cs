// Tests/Core/Moves/PositionTests.cs

using Core.Moves;

namespace Tests.Core.Moves;

public class PositionTests
{
    /// <summary>
    /// Проверяет, что метод FromAlgebraic корректно преобразует шахматную нотацию
    /// в объект Position с правильными Row и Col, а ToAlgebraic возвращает исходную строку.
    /// Тестируются: e2 -> (1,4), a1 -> (0,0), h8 -> (7,7).
    /// </summary>
    [Fact]
    public void FromAlgebraic_ValidInput_ReturnsCorrectPosition()
    {
        var pos = Position.FromAlgebraic("e2");
        Assert.Equal(1, pos.Row);
        Assert.Equal(4, pos.Col);
        Assert.Equal("e2", pos.ToAlgebraic());

        var pos2 = Position.FromAlgebraic("a1");
        Assert.Equal(0, pos2.Row);
        Assert.Equal(0, pos2.Col);
        Assert.Equal("a1", pos2.ToAlgebraic());

        var pos3 = Position.FromAlgebraic("h8");
        Assert.Equal(7, pos3.Row);
        Assert.Equal(7, pos3.Col);
        Assert.Equal("h8", pos3.ToAlgebraic());
    }

    /// <summary>
    /// Проверяет, что метод FromAlgebraic выбрасывает ArgumentException
    /// при передаче некорректных строк: неверная буква, неверная цифра, короткая строка.
    /// </summary>
    [Fact]
    public void FromAlgebraic_InvalidInput_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => Position.FromAlgebraic("z9"));
        Assert.Throws<ArgumentException>(() => Position.FromAlgebraic("a0"));
        Assert.Throws<ArgumentException>(() => Position.FromAlgebraic("e"));
    }

    /// <summary>
    /// Проверяет, что свойство IsValid возвращает true для координат внутри доски (0..7).
    /// </summary>
    [Fact]
    public void IsValid_ValidCoordinates_ReturnsTrue()
    {
        Assert.True(new Position(0, 0).IsValid);
        Assert.True(new Position(7, 7).IsValid);
    }

    /// <summary>
    /// Проверяет, что свойство IsValid возвращает false для координат вне доски (отрицательные или >=8).
    /// </summary>
    [Fact]
    public void IsValid_InvalidCoordinates_ReturnsFalse()
    {
        Assert.False(new Position(8, 0).IsValid);
        Assert.False(new Position(-1, 4).IsValid);
    }
}