using Core.Pieces;

namespace Core.Moves;

/// <summary>
/// Представляет ход из одной позиции в другую, включая возможное превращение.
/// </summary>
public class Move(Position from, Position to, PieceType? promotionType = null)
{
    /// <summary>Начальная позиция хода.</summary>
    public Position From { get; } = from;
    /// <summary>Конечная позиция хода.</summary>
    public Position To { get; } = to;
    /// <summary>Тип фигуры для превращения (если применимо).</summary>
    public PieceType? PromotionType { get; } = promotionType;
    /// <summary>Флаг, указывающий, является ли ход рокировкой.</summary>
    public bool IsCastling { get; init; } = false;
    /// <summary>Сторона рокировки (короткая/длинная), если IsCastling == true.</summary>
    public CastlingSide? CastlingSide { get; init; } = null;

    public override bool Equals(object? obj)
    {
        if (obj is Move other)
        {
            return From.Equals(other.From) && To.Equals(other.To)
                && PromotionType == other.PromotionType
                && IsCastling == other.IsCastling
                && CastlingSide == other.CastlingSide;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(From, To, PromotionType, IsCastling, CastlingSide);
    }

    public override string ToString()
    {
        var promotion = PromotionType.HasValue ? $" ({PromotionType})" : "";
        return $"{From} -> {To}{promotion}";
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