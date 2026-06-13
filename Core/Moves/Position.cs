namespace Core.Moves;

/// <summary>
/// Неизменяемая структура, представляющая координаты клетки (строка, столбец).
/// </summary>
/// <param name="Row">Строка от 0 (1-я горизонталь, белые) до 7 (8-я горизонталь, чёрные).</param>
/// <param name="Col">Столбец от 0 (a) до 7 (h).</param>
public readonly record struct Position(int Row, int Col)
{
    /// <summary>Проверяет, находятся ли координаты в пределах доски 0..7.</summary>
    /// <returns>true, если координаты корректны, иначе false.</returns>
    public bool IsValid => Row is >= 0 and < 8 && Col is >= 0 and < 8;

    /// <summary>Преобразует шахматную нотацию (например, "e2") в позицию.</summary>
    /// <param name="algebraic">Нотация из двух символов: буква a-h и цифра 1-8.</param>
    /// <returns>Соответствующая позиция.</returns>
    /// <exception cref="ArgumentException">Если нотация некорректна.</exception>
    public static Position FromAlgebraic(string algebraic)
    {
        if (string.IsNullOrEmpty(algebraic) || algebraic.Length != 2)
            throw new ArgumentException("Нотация должна быть из двух символов, например 'e2'", nameof(algebraic));

        int col = algebraic[0] - 'a';
        int row = algebraic[1] - '1';
        if (row < 0 || row > 7 || col < 0 || col > 7)
            throw new ArgumentException($"Некорректная нотация: {algebraic}");
        return new Position(row, col);
    }

    /// <summary>Преобразует позицию в шахматную нотацию, например "e2".</summary>
    /// <returns>Строка вида {буква}{цифра}.</returns>
    public string ToAlgebraic() => $"{(char)('a' + Col)}{Row + 1}";

    /// <inheritdoc cref="ToAlgebraic"/>
    public override string ToString() => ToAlgebraic();
}