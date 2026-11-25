// Core/Pieces/Piece.cs

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
    public PieceType? PromotionTarget { get; init; } = PromotionTarget;

    /// <summary>
    /// Указывает, была ли эта фигура получена путём превращения ратника.
    /// <para>
    /// При попадании такой фигуры под другую башню (свою или вражескую) она
    /// переворачивается обратно в ратника — правило обратного превращения.
    /// </para>
    /// <para>
    /// Для обычных фигур, не полученных превращением, значение <c>false</c>.
    /// </para>
    /// </summary>
    public bool IsPromoted { get; init; }

    /// <summary>
    /// Указывает, совершала ли эта фигура хотя бы один ход — включая перемещение
    /// в составе башни, взятие, превращение и обратное превращение.
    /// <para>
    /// Используется для правил, зависящих от истории фигуры:
    /// </para>
    /// <list type="bullet">
    /// <item>Ратник, который уже ходил, теряет право на двойной ход вперёд,
    /// даже если в процессе игры вернулся на исходную горизонталь
    /// (2-ю для белых, 7-ю для чёрных).</item>
    /// </list>
    /// <para>
    /// Значение <c>false</c> только у фигур, не совершавших ходов с момента
    /// создания (например, в начальной расстановке).
    /// </para>
    /// </summary>
    public bool HasMoved { get; init; }

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
        _ => "??"
    };

    /// <summary>
    /// Возвращает короткий код фигуры для отладки/консоли.
    /// <para>Схема: <c>P</c> + цель превращения для Ratnik, одна буква для остальных.
    /// Заглавные — белые, строчные — чёрные. Суффикс <c>*</c> — фигура получена превращением.</para>
    /// </summary>
    public string GetDisplayCode()
    {
        var sb = new System.Text.StringBuilder(3);

        // Основной код: P + цель для Ratnik, одна буква для остальных.
        if (Type == PieceType.Ratnik)
        {
            sb.Append('P');
            sb.Append(PromotionTarget switch
            {
                PieceType.Ratoborets => 'R',
                PieceType.Archer => 'A',
                PieceType.Horseman => 'H',
                PieceType.Knyaz => 'K',
                PieceType.Helgi => 'X',
                _ => '?'
            });
        }
        else
        {
            sb.Append(Type switch
            {
                PieceType.Ratoborets => 'R',
                PieceType.Archer => 'A',
                PieceType.Horseman => 'H',
                PieceType.Knyaz => 'K',
                PieceType.Volkhv => 'V',
                PieceType.Helgi => 'X',
                _ => '?'
            });
        }

        // Индикатор превращения.
        if (IsPromoted)
            sb.Append('*');

        // Цвет: строчные для чёрных.
        if (Color == PieceColor.Black)
        {
            for (int i = 0; i < sb.Length; i++)
                if (char.IsUpper(sb[i]))
                    sb[i] = char.ToLower(sb[i]);
        }

        return sb.ToString();
    }

    // TODO: Реализовать метод GetFenChar() для совместимости с FEN.
    //       В таврелях своя нотация, но для обмена партиями нужен стандарт.
    //       Возможно, использовать расширение .tav или свой формат.
}