using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Board;

public class BoardTests
{
    /// <summary>
    /// Проверяет корректность начальной расстановки фигур через PieceFactory.
    /// Углы должны быть Ratoborets, центр — пуст, пешки расставлены на 2-й и 7-й горизонталях.
    /// </summary>
    [Fact]
    public void StartingPosition_HasCorrectPieces()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());

        // Углы
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(0, 0)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(0, 0)?.Color);
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(0, 7)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(0, 7)?.Color);   // h1
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(7, 0)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(7, 0)?.Color);   // a8
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(7, 7)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(7, 7)?.Color);   // h8

        // Центр пуст
        Assert.Null(board.GetPiece(3, 3));

        // Пешки: белая на e2? (e2 это row=1, col=4)
        Assert.Equal(PieceType.Ratnik, board.GetPiece(1, 4)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(1, 4)?.Color);
        // Чёрная пешка на e7 (row=6, col=4)
        Assert.Equal(PieceType.Ratnik, board.GetPiece(6, 4)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(6, 4)?.Color);
    }

    /// <summary>
    /// Проверяет, что установка фигуры через SetPiece корректно обновляет доску,
    /// а GetPiece возвращает ту же ссылку.
    /// </summary>
    [Fact]
    public void SetPiece_UpdatesBoard()
    {
        var board = new ChessBoard();
        var piece = new Piece(PieceType.Helgi, PieceColor.White);
        board.SetPiece(4, 4, piece);

        Assert.Same(piece, board.GetPiece(4, 4));
    }

    /// <summary>
    /// Проверяет, что метод Clear удаляет все фигуры с доски (все клетки становятся null).
    /// </summary>
    [Fact]
    public void Clear_RemovesAllPieces()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        board.Clear();

        for (int row = 0; row < ChessBoard.Size; row++)
            for (int col = 0; col < ChessBoard.Size; col++)
                Assert.Null(board.GetPiece(row, col));
    }

    /// <summary>
    /// Проверяет, что ToString() возвращает строку, содержащую ожидаемые двухбуквенные коды фигур
    /// (Ra, ra, Rt, rt) и шахматную нотацию координат (a b c d e f g h).
    /// </summary>
    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        string output = board.ToString();

        Assert.Contains("Ra", output);
        Assert.Contains("ra", output);
        Assert.Contains("Rt", output);
        Assert.Contains("rt", output);
        Assert.Contains("a  b  c  d  e  f  g  h", output);
    }

    /// <summary>
    /// Проверяет, что попытка установить фигуру за пределами доски выбрасывает ArgumentException.
    /// </summary>
    [Fact]
    public void SetPiece_InvalidPosition_ThrowsArgumentException()
    {
        var board = new ChessBoard();
        Assert.Throws<ArgumentException>(() => board.SetPiece(8, 4, new Piece(PieceType.Ratnik, PieceColor.White)));
        Assert.Throws<ArgumentException>(() => board.SetPiece(new Position(8, 4), new Piece(PieceType.Ratnik, PieceColor.White)));
    }
}