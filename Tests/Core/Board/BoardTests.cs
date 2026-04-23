using Core.Pieces;
using BoardClass = Core.Board.Board;

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
        var board = new BoardClass(PieceFactory.StartingPosition());

        Assert.Equal(PieceType.Ratoborets, board.GetPiece(0, 0)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(0, 0)?.Color);
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(0, 7)?.Type);
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(7, 0)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(7, 0)?.Color);

        Assert.Null(board.GetPiece(3, 3));

        Assert.Equal(PieceType.Ratnik, board.GetPiece(1, 4)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(1, 4)?.Color);
        Assert.Equal(PieceType.Ratnik, board.GetPiece(6, 4)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(6, 4)?.Color);
    }

    /// <summary>
    /// Проверяет, что установка фигуры через SetPiece корректно обновляет доску,
    /// а GetPiece возвращает ту же ссылку.
    /// </summary>
    [Fact]
    public void SetPiece_UpdatesBoard()
    {
        var board = new BoardClass();
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
        var board = new BoardClass(PieceFactory.StartingPosition());
        board.Clear();

        for (int row = 0; row < BoardClass.Size; row++)
            for (int col = 0; col < BoardClass.Size; col++)
                Assert.Null(board.GetPiece(row, col));
    }

    /// <summary>
    /// Проверяет, что ToString() возвращает строку, содержащую ожидаемые двухбуквенные коды фигур
    /// (Ra, ra, Rt, rt) и шахматную нотацию координат (a b c d e f g h).
    /// </summary>
    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var board = new BoardClass(PieceFactory.StartingPosition());
        string output = board.ToString();

        Assert.Contains("Ra", output);
        Assert.Contains("ra", output);
        Assert.Contains("Rt", output);
        Assert.Contains("rt", output);
        Assert.Contains("a  b  c  d  e  f  g  h", output);
    }
}