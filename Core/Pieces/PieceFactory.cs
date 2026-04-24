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

        // Белые (ряд 0, он же 1-я горизонталь)
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
            board[1, col] = new Piece(PieceType.Ratnik, PieceColor.White);

        // Чёрные пешки (ряд 6)
        for (int col = 0; col < 8; col++)
            board[6, col] = new Piece(PieceType.Ratnik, PieceColor.Black);

        // Чёрные (ряд 7)
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