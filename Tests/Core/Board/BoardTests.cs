using Core.Pieces;
using BoardClass = Core.Board.Board;

namespace Tests.Core.Board;

public class BoardTests
{
    [Fact]
    public void StartingPosition_HasCorrectPieces()
    {
        var board = new BoardClass(PieceFactory.StartingPosition());

        // Проверяем углы
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(0, 0)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(0, 0)?.Color);
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(0, 7)?.Type);
        Assert.Equal(PieceType.Ratoborets, board.GetPiece(7, 0)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(7, 0)?.Color);

        // Проверяем, что в центре пусто
        Assert.Null(board.GetPiece(3, 3));

        // Проверяем пешек
        Assert.Equal(PieceType.Ratnik, board.GetPiece(1, 4)?.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(1, 4)?.Color);
        Assert.Equal(PieceType.Ratnik, board.GetPiece(6, 4)?.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(6, 4)?.Color);
    }

    [Fact]
    public void SetPiece_UpdatesBoard()
    {
        var board = new BoardClass();
        var piece = new Piece(PieceType.Helgi, PieceColor.White);
        board.SetPiece(4, 4, piece);

        Assert.Same(piece, board.GetPiece(4, 4));
    }

    [Fact]
    public void Clear_RemovesAllPieces()
    {
        var board = new BoardClass(PieceFactory.StartingPosition());
        board.Clear();

        for (int row = 0; row < BoardClass.Size; row++)
            for (int col = 0; col < BoardClass.Size; col++)
                Assert.Null(board.GetPiece(row, col));
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var board = new BoardClass(PieceFactory.StartingPosition());
        string output = board.ToString();

        // Проверяем наличие ожидаемых двухбуквенных кодов
        Assert.Contains("Ra", output); // белая ладья
        Assert.Contains("ra", output); // чёрная ладья
        Assert.Contains("Rt", output); // белая пешка
        Assert.Contains("rt", output); // чёрная пешка
        Assert.Contains("a  b  c  d  e  f  g  h", output);
    }
}
