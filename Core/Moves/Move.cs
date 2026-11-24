using Core.Pieces;

namespace Core.Moves;

/// <summary>
/// Представляет ход из одной позиции в другую.
/// </summary>
/// <param name="from">Начальная позиция хода.</param>
/// <param name="to">Конечная позиция хода.</param>
public class Move(Position from, Position to)
{
    /// <summary>Начальная позиция хода.</summary>
    public Position From { get; } = from;

    /// <summary>Конечная позиция хода.</summary>
    public Position To { get; } = to;

    /// <summary>
    /// Флаг, указывающий, является ли ход рокировкой.
    /// </summary>
    public bool IsCastling { get; init; } = false;

    /// <summary>
    /// Сторона рокировки (короткая/длинная), если <see cref="IsCastling"/> == true.
    /// </summary>
    public CastlingSide? CastlingSide { get; init; } = null;

    /// <summary>
    /// Количество верхних фигур, которые нужно переместить (если null — перемещается вся башня).
    /// </summary>
    public int? SplitCount { get; init; }

    /// <summary>
    /// Флаг, указывающий, является ли ход взятием на проходе.
    /// </summary>
    /// <remarks>
    /// Пленный — только верхний ратник вражеской башни; остальная часть башни
    /// остаётся на месте.
    /// </remarks>
    public bool IsEnPassant { get; init; } = false;

    /// <summary>
    /// Создаёт объект Move, представляющий рокировку.
    /// </summary>
    /// <param name="from">Позиция короля до рокировки.</param>
    /// <param name="to">Позиция короля после рокировки.</param>
    /// <param name="side">Сторона рокировки.</param>
    /// <returns>Объект Move с установленными флагами <see cref="IsCastling"/> и <see cref="CastlingSide"/>.</returns>
    public static Move CreateCastling(Position from, Position to, CastlingSide side) =>
        new(from, to) { IsCastling = true, CastlingSide = side };

    /// <summary>
    /// Создаёт объект Move, представляющий взятие на проходе.
    /// </summary>
    /// <param name="from">Позиция ратника до взятия.</param>
    /// <param name="to">Целевая позиция — клетка, «перепрыгнутая» вражеским ратником.</param>
    /// <param name="splitCount">Количество верхних фигур для перемещения (если применимо).</param>
    /// <returns>Объект Move с установленным флагом <see cref="IsEnPassant"/>.</returns>
    public static Move CreateEnPassant(Position from, Position to, int? splitCount = null) =>
        new(from, to) { IsEnPassant = true, SplitCount = splitCount };

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is Move other)
        {
            return From.Equals(other.From)
                && To.Equals(other.To)
                && IsCastling == other.IsCastling
                && CastlingSide == other.CastlingSide
                && SplitCount == other.SplitCount
                && IsEnPassant == other.IsEnPassant;
        }
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(From, To, IsCastling, CastlingSide, SplitCount, IsEnPassant);

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($"{From} -> {To}");
        if (IsCastling)
            sb.Append(" (рокировка)");
        if (IsEnPassant)
            sb.Append(" (взятие на проходе)");
        if (SplitCount.HasValue)
            sb.Append($" (разделение: {SplitCount})");
        return sb.ToString();
    }
}

/// <summary>
/// Сторона рокировки.
/// </summary>
public enum CastlingSide
{
    KingSide,
    QueenSide
}