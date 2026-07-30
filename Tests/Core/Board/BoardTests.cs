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
        Assert.Equal(PieceType.Ratoborets, board.GetTopPiece(new Position(0, 0))?.Type);
        Assert.Equal(PieceColor.White, board.GetTopPiece(new Position(0, 0))?.Color);
        Assert.Equal(PieceType.Ratoborets, board.GetTopPiece(new Position(0, 7))?.Type);
        Assert.Equal(PieceColor.White, board.GetTopPiece(new Position(0, 7))?.Color);   // h1
        Assert.Equal(PieceType.Ratoborets, board.GetTopPiece(new Position(7, 0))?.Type);
        Assert.Equal(PieceColor.Black, board.GetTopPiece(new Position(7, 0))?.Color);   // a8
        Assert.Equal(PieceType.Ratoborets, board.GetTopPiece(new Position(7, 7))?.Type);
        Assert.Equal(PieceColor.Black, board.GetTopPiece(new Position(7, 7))?.Color);   // h8

        // Центр пуст
        Assert.Null(board.GetTopPiece(new Position(3, 3)));

        // Пешки: белая на e2? (e2 это row=1, col=4)
        Assert.Equal(PieceType.Ratnik, board.GetTopPiece(new Position(1, 4))?.Type);
        Assert.Equal(PieceColor.White, board.GetTopPiece(new Position(1, 4))?.Color);
        // Чёрная пешка на e7 (row=6, col=4)
        Assert.Equal(PieceType.Ratnik, board.GetTopPiece(new Position(6, 4))?.Type);
        Assert.Equal(PieceColor.Black, board.GetTopPiece(new Position(6, 4))?.Color);
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

        Assert.Same(piece, board.GetTopPiece(new Position(4, 4)));
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

    /// <summary>
    /// Проверяет, что метод MakeMove выполняет простой ход пешки (e2-e3) и корректно обновляет доску.
    /// </summary>
    [Fact]
    public void MakeMove_SimpleMove_UpdatesBoard()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var move = new Move(new Position(1, 4), new Position(2, 4)); // e2-e3
        bool result = board.MakeMove(move);
        Assert.True(result);
        Assert.NotNull(board.GetTopPiece(new Position(2, 4)));
        Assert.Null(board.GetTopPiece(new Position(1, 4)));
    }

    /// <summary>
    /// Проверяет, что метод MakeMove возвращает false для нелегального хода (пешка пытается пойти горизонтально).
    /// </summary>
    [Fact(Skip = "Временно пропущен, пока не адаптированы генераторы для башен")]
    public void MakeMove_IllegalMove_ReturnsFalse()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var move = new Move(new Position(1, 4), new Position(1, 5)); // e2-f2? нелегально для пешки
        bool result = board.MakeMove(move);
        Assert.False(result);
    }

    /// <summary>
    /// Проверяет, что метод MakeMove с параметром splitCount перемещает только верхнюю часть башни,
    /// оставляя нижнюю часть на исходной клетке.
    /// </summary>
    [Fact]
    public void SplitStack_MovesOnlyTopPart()
    {
        var board = new ChessBoard();
        // Добавляем короля для валидации
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));

        var stack = new List<Piece>
        {
            new(PieceType.Ratnik, PieceColor.White),
            new(PieceType.Horseman, PieceColor.White),
            new(PieceType.Archer, PieceColor.White)
        };
        board.SetStack(new Position(0, 0), stack);

        // Ходим верхней частью (2 фигуры) на (0,1)
        var move = new Move(new Position(0, 0), new Position(0, 1));
        var result = board.MakeMove(move, 2);
        Assert.True(result);

        // Проверяем, что на (0,0) осталась 1 фигура (Ratnik)
        var remaining = board.GetStack(new Position(0, 0));
        Assert.NotNull(remaining);
        Assert.Single(remaining);
        Assert.Equal(PieceType.Ratnik, remaining[0].Type);

        // Проверяем, что на (0,1) теперь 2 фигуры (Archer + Horseman)
        var moved = board.GetStack(new Position(0, 1));
        Assert.NotNull(moved);
        Assert.Equal(2, moved.Count);
        Assert.Equal(PieceType.Archer, moved[^1].Type); // верхняя фигура — Archer
    }

    /// <summary>
    /// Проверяет, что PushPiece добавляет фигуру в стек на указанной позиции.
    /// </summary>
    [Fact]
    public void PushPiece_AddsToStack()
    {
        var board = new ChessBoard();
        var pos = new Position(0, 0);
        var piece = new Piece(PieceType.Ratnik, PieceColor.White);
        board.PushPiece(pos, piece);
        Assert.Equal(piece, board.GetTopPiece(pos));
        var stack = board.GetStack(pos);
        Assert.NotNull(stack);
        Assert.Single(stack);
    }

    /// <summary>
    /// Проверяет, что PopPiece удаляет и возвращает верхнюю фигуру стека.
    /// </summary>
    [Fact]
    public void PopPiece_RemovesTop()
    {
        var board = new ChessBoard();
        var pos = new Position(0, 0);
        board.PushPiece(pos, new Piece(PieceType.Ratnik, PieceColor.White));
        board.PushPiece(pos, new Piece(PieceType.Horseman, PieceColor.White));
        var popped = board.PopPiece(pos);
        Assert.Equal(PieceType.Horseman, popped?.Type);
        Assert.Equal(PieceType.Ratnik, board.GetTopPiece(pos)?.Type);
    }

    /// <summary>
    /// Проверяет, что SetStack заменяет стек на позиции новым списком.
    /// </summary>
    [Fact]
    public void SetStack_ReplacesStack()
    {
        var board = new ChessBoard();
        var pos = new Position(0, 0);
        var stack = new List<Piece> { new(PieceType.Archer, PieceColor.White) };
        board.SetStack(pos, stack);
        Assert.Equal(stack, board.GetStack(pos));
        Assert.Equal(PieceType.Archer, board.GetTopPiece(pos)?.Type);
    }

    /// <summary>
    /// Проверяет, что ConcatStacks объединяет две башни (верхняя ставится поверх нижней).
    /// </summary>
    [Fact]
    public void ConcatStacks_CombinesTwoStacks()
    {
        var bottom = new List<Piece> { new(PieceType.Ratnik, PieceColor.White) };
        var top = new List<Piece> { new(PieceType.Horseman, PieceColor.White), new(PieceType.Archer, PieceColor.White) };
        var result = ChessBoard.ConcatStacks(bottom, top);
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(PieceType.Ratnik, result[0].Type);
        Assert.Equal(PieceType.Horseman, result[1].Type);
        Assert.Equal(PieceType.Archer, result[2].Type);
    }

    /// <summary>
    /// Проверяет, что SplitStack разделяет башню на верхнюю и нижнюю части заданного размера.
    /// </summary>
    [Fact]
    public void SplitStack_SplitsCorrectly()
    {
        var stack = new List<Piece>
    {
        new(PieceType.Ratnik, PieceColor.White),
        new(PieceType.Horseman, PieceColor.White),
        new(PieceType.Archer, PieceColor.White)
    };
        var (upper, lower) = ChessBoard.SplitStack(stack, 2);
        Assert.NotNull(upper);
        Assert.NotNull(lower);
        Assert.Equal(2, upper.Count);
        Assert.Single(lower);
        Assert.Equal(PieceType.Ratnik, upper[0].Type);
        Assert.Equal(PieceType.Horseman, upper[1].Type);
        Assert.Equal(PieceType.Archer, lower[0].Type);
    }
}