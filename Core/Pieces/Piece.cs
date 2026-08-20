namespace Core.Pieces;

/// <summary>
/// Неизменяемая таврель (фигура), содержащая тип, цвет и тип превращения для пешек.
/// </summary>
/// <param name="Type">Тип таврели.</param>
/// <param name="Color">Цвет.</param>
/// <param name="PromotionTarget">Тип фигуры, в которую превращается пешка (только для пешек, иначе null).</param>
public record Piece(PieceType Type, PieceColor Color, PieceType? PromotionTarget = null)
{
    /// <summary>
    /// Тип таврели.
    /// </summary>
    public PieceType Type { get; } = Type;

    /// <summary>
    /// Цвет.
    /// </summary>
    public PieceColor Color { get; } = Color;

    /// <summary>
    /// Тип фигуры, в которую превращается пешка (только для пешек, иначе null).
    /// </summary>
    public PieceType? PromotionTarget { get; } = PromotionTarget;

    /// <summary>
    /// Возвращает двухбуквенный код для отладки/консоли (например, "Vo", "ho").
    /// Временное решение — используем первые буквы (позже заменим на изображения).
    /// </summary>
    public string GetDebugCode() => (Color, Type) switch
    {
        (PieceColor.White, PieceType.Volkhv) => "Vo",
        (PieceColor.White, PieceType.Horseman) => "Ho",
        (PieceColor.White, PieceType.Helgi) => "He",
        (PieceColor.White, PieceType.Knyaz) => "Kn",
        (PieceColor.White, PieceType.Ratoborets) => "Ra",
        (PieceColor.White, PieceType.Archer) => "Ar",
        (PieceColor.White, PieceType.Ratnik) => "Rt",
        (PieceColor.Black, PieceType.Volkhv) => "vo",
        (PieceColor.Black, PieceType.Horseman) => "ho",
        (PieceColor.Black, PieceType.Helgi) => "he",
        (PieceColor.Black, PieceType.Knyaz) => "kn",
        (PieceColor.Black, PieceType.Ratoborets) => "ra",
        (PieceColor.Black, PieceType.Archer) => "ar",
        (PieceColor.Black, PieceType.Ratnik) => "rt",
        // аналогично для чёрных, можно нижний регистр: "vo", "ho" и т.д.
        _ => "??"
    };

    // TODO: Реализовать метод GetFenChar() для совместимости с FEN.
    //       В таврелях своя нотация, но для обмена партиями нужен стандарт.
    //       Возможно, использовать расширение .tav или свой формат.
}