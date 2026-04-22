namespace Core.Pieces;

/// <summary>
/// Неизменяемая таврель (фигура), содержащая тип и цвет.
/// В будущем может хранить также "башню" (стек фигур), но для MVP — простая.
/// </summary>
/// <param name="Type">Тип таврели (Volkhv, Horseman, ...)</param>
/// <param name="Color">Цвет (белый/чёрный)</param>
public record Piece(PieceType Type, PieceColor Color)
{
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