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
    /// <summary>Количество верхних фигур, которые нужно переместить (если null – перемещается вся башня).</summary>
    public int? SplitCount { get; init; }

    /// <summary>
    /// Создаёт объект Move, представляющий рокировку.
    /// </summary>
    /// <param name="from">Позиция короля до рокировки.</param>
    /// <param name="row">Горизонталь короля (0 для белых, 7 для чёрных).</param>
    /// <param name="col">Конечная позиция короля (6 для короткой, 2 для длинной).</param>
    /// <param name="side">Сторона рокировки.</param>
    /// <returns>Объект Move с установленными флагами IsCastling и CastlingSide.</returns>
    public static Move CreateCastlingMove(Position from, int row, int col, CastlingSide side)
    {
        return new Move(from, new Position(row, col), null)
        {
            IsCastling = true,
            CastlingSide = side
        };
    }

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