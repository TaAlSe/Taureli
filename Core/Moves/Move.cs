using Core.Pieces;

namespace Core.Moves;

/// <summary>
/// Представляет ход из одной позиции в другую.
/// </summary>
/// <param name="From">Начальная позиция.</param>
/// <param name="To">Конечная позиция.</param>
/// <param name="PromotionType">Тип фигуры, в которую превращается пешка (только для ходов пешки на последнюю горизонталь).</param>
public record Move(Position From, Position To, PieceType? PromotionType = null);