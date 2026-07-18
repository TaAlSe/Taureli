using Core.Pieces;

namespace Core.Moves;

/// <summary>
/// Представляет ход из одной позиции в другую, включая возможное превращение.
/// </summary>
public class Move(Position from, Position to, PieceType? promotionType = null)
{
    public Position From { get; } = from;
    public Position To { get; } = to;
    public PieceType? PromotionType { get; } = promotionType;
    public bool IsCastling { get; init; } = false;
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