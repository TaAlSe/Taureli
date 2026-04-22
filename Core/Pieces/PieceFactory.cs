namespace Core.Pieces;

/// <summary>
/// Вспомогательный класс для создания начальной позиции.
/// </summary>
public static class PieceFactory
{
    /// <summary>
    /// Возвращает двумерный массив 8x8 с фигурами в начальной расстановке.
    /// Индексы: [строка, столбец], где 0 — верх (чёрные), 7 — низ (белые).
    /// </summary>
    public static Piece?[,] StartingPosition()
    {
        var board = new Piece?[8, 8];

        // Чёрные (верх)
        board[0, 0] = new Piece(PieceType.Ratoborets, PieceColor.Black);
        board[0, 1] = new Piece(PieceType.Horseman, PieceColor.Black);
        board[0, 2] = new Piece(PieceType.Archer, PieceColor.Black);
        board[0, 3] = new Piece(PieceType.Knyaz, PieceColor.Black);
        board[0, 4] = new Piece(PieceType.Volkhv, PieceColor.Black);
        board[0, 5] = new Piece(PieceType.Archer, PieceColor.Black);
        board[0, 6] = new Piece(PieceType.Horseman, PieceColor.Black);
        board[0, 7] = new Piece(PieceType.Ratoborets, PieceColor.Black);

        // Чёрные ратники (пешки)
        for (int col = 0; col < 8; col++)
            board[1, col] = new Piece(PieceType.Ratnik, PieceColor.Black);

        // Белые ратники
        for (int col = 0; col < 8; col++)
            board[6, col] = new Piece(PieceType.Ratnik, PieceColor.White);

        // Белые (низ)
        board[7, 0] = new Piece(PieceType.Ratoborets, PieceColor.White);
        board[7, 1] = new Piece(PieceType.Horseman, PieceColor.White);
        board[7, 2] = new Piece(PieceType.Archer, PieceColor.White);
        board[7, 3] = new Piece(PieceType.Knyaz, PieceColor.White);
        board[7, 4] = new Piece(PieceType.Volkhv, PieceColor.White);
        board[7, 5] = new Piece(PieceType.Archer, PieceColor.White);
        board[7, 6] = new Piece(PieceType.Horseman, PieceColor.White);
        board[7, 7] = new Piece(PieceType.Ratoborets, PieceColor.White);

        return board;
    }
}