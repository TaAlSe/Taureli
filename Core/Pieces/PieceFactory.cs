// Core/Pieces/PieceFactory.cs

namespace Core.Pieces;

/// <summary>
/// Вспомогательный класс для создания начальной позиции.
/// </summary>
public static class PieceFactory
{
    /// <summary>
    /// Возвращает двумерный массив 8x8 с фигурами в начальной расстановке.
    /// Индексы: [row, col], где 0 — нижняя горизонталь (белые, ряд 1),
    /// 7 — верхняя горизонталь (чёрные, ряд 8), col: 0=a, 7=h.
    /// </summary>
    public static Piece?[,] StartingPosition()
    {
        var board = new Piece?[8, 8];

        // Определяем, перед какой фигурой стоит пешка
        // Фигуры: 0 - ладья, 1 - конь, 2 - слон, 3 - ферзь, 4 - король, 5 - слон, 6 - конь, 7 - ладья
        PieceType[] backRowTypes =
        [
            PieceType.Ratoborets, PieceType.Horseman, PieceType.Archer, PieceType.Knyaz,
            PieceType.Volkhv, PieceType.Archer, PieceType.Horseman, PieceType.Ratoborets
        ];

        // Белые фигуры (нижний ряд)
        board[0, 0] = new Piece(PieceType.Ratoborets, PieceColor.White);
        board[0, 1] = new Piece(PieceType.Horseman, PieceColor.White);
        board[0, 2] = new Piece(PieceType.Archer, PieceColor.White);
        board[0, 3] = new Piece(PieceType.Knyaz, PieceColor.White);
        board[0, 4] = new Piece(PieceType.Volkhv, PieceColor.White);
        board[0, 5] = new Piece(PieceType.Archer, PieceColor.White);
        board[0, 6] = new Piece(PieceType.Horseman, PieceColor.White);
        board[0, 7] = new Piece(PieceType.Ratoborets, PieceColor.White);

        // Белые пешки (ряд 1)
        for (int col = 0; col < 8; col++)
        {
            var promotionTarget = backRowTypes[col];
            // Для пешки перед королём (col=4) – превращение в Helgi
            if (col == 4)
                promotionTarget = PieceType.Helgi;
            board[1, col] = new Piece(PieceType.Ratnik, PieceColor.White, promotionTarget);
        }

        // Чёрные пешки (ряд 6)
        // Чёрные фигуры на row=7
        for (int col = 0; col < 8; col++)
        {
            var promotionTarget = backRowTypes[col];
            if (col == 4)
                promotionTarget = PieceType.Helgi;
            board[6, col] = new Piece(PieceType.Ratnik, PieceColor.Black, promotionTarget);
        }

        // Чёрные фигуры (верхний ряд)
        board[7, 0] = new Piece(PieceType.Ratoborets, PieceColor.Black);
        board[7, 1] = new Piece(PieceType.Horseman, PieceColor.Black);
        board[7, 2] = new Piece(PieceType.Archer, PieceColor.Black);
        board[7, 3] = new Piece(PieceType.Knyaz, PieceColor.Black);
        board[7, 4] = new Piece(PieceType.Volkhv, PieceColor.Black);
        board[7, 5] = new Piece(PieceType.Archer, PieceColor.Black);
        board[7, 6] = new Piece(PieceType.Horseman, PieceColor.Black);
        board[7, 7] = new Piece(PieceType.Ratoborets, PieceColor.Black);

        return board;
    }
}