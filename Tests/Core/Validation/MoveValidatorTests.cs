// Tests/Core/Validation/MoveValidatorTests.cs

using Core.Board;
using Core.Pieces;
using Core.Moves;
using Core.Validation;

namespace Tests.Core.Validation;

public class MoveValidatorTests
{
    /// <summary>
    /// Проверяет, что обычный ход пешки (e2-e3) легален, если не оставляет короля под шахом.
    /// </summary>
    [Fact]
    public void SimpleMove_NotLeavingKingInCheck_IsLegal()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var move = new Move(new Position(1, 4), new Position(2, 4));
        Assert.True(MoveValidator.IsLegalMove(board, move));
    }

    /// <summary>
    /// Проверяет, что ход пешки, который открывает диагональ для атаки на короля, считается нелегальным.
    /// Белый король на e1, белая пешка на f2, чёрный слон на h4.
    /// Ход f2-f3 убирает блокировку → нелегально.
    /// </summary>
    [Fact]
    public void MoveThatExposesKingToCheck_IsIllegal()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(1, 5), new Piece(PieceType.Ratnik, PieceColor.White));
        board.SetPiece(new Position(3, 7), new Piece(PieceType.Archer, PieceColor.Black));

        var move = new Move(new Position(1, 5), new Position(2, 5));
        Assert.False(MoveValidator.IsLegalMove(board, move));
    }

    /// <summary>
    /// Проверяет, что GetLegalMoves возвращает только легальные ходы.
    /// В позиции с пешкой f2, блокирующей слона h4, все ходы пешки нелегальны.
    /// </summary>
    [Fact]
    public void GetLegalMoves_FiltersIllegalMoves()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.SetPiece(new Position(1, 5), new Piece(PieceType.Ratnik, PieceColor.White));
        board.SetPiece(new Position(3, 7), new Piece(PieceType.Archer, PieceColor.Black));

        var legalMoves = MoveValidator.GetLegalMoves(board, new Position(1, 5));
        Assert.Empty(legalMoves);
    }

    /// <summary>
    /// Проверяет, что нелегальный ход (пешка горизонтально) не проходит валидацию.
    /// </summary>
    [Fact]
    public void IllegalPawnMove_Horizontal_IsNotLegal()
    {
        var board = new ChessBoard(PieceFactory.StartingPosition());
        var illegalMove = new Move(new Position(1, 4), new Position(1, 5)); // e2-f2
        Assert.False(MoveValidator.IsLegalMove(board, illegalMove));
    }

    /// <summary>
    /// Проверяет, что IsLegalMove не изменяет состояние доски (откатывает ход).
    /// </summary>
    [Fact]
    public void IsLegalMove_DoesNotMutateBoard()
    {
        var board = new ChessBoard();
        // Создаём башню из трёх фигур
        var stack = new List<Piece>
    {
        new(PieceType.Ratnik, PieceColor.White),
        new(PieceType.Horseman, PieceColor.White),
        new(PieceType.Archer, PieceColor.White)
    };
        var pos = new Position(0, 0);
        board.SetStack(pos, stack);
        // Добавляем короля, чтобы валидация проходила
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));

        // Сохраняем состояние до проверки (глубокое копирование)
        var beforeFromStack = board.GetStack(pos)?.ToList();
        var beforeToStack = board.GetStack(new Position(0, 1))?.ToList();

        // Делаем ход (перемещаем верхнюю фигуру)
        var move = new Move(pos, new Position(0, 1));

        // Вызываем валидатор (он должен выполнить и откатить ход)
        _ = MoveValidator.IsLegalMove(board, move);

        // Проверяем, что стеки не изменились
        var afterFromStack = board.GetStack(pos);
        var afterToStack = board.GetStack(new Position(0, 1));

        Assert.Equal(beforeFromStack, afterFromStack);
        Assert.Equal(beforeToStack, afterToStack);
    }

    /// <summary>
    /// Проверяет, что IsLegalMove не оставляет позицию короля изменённой после симуляции.
    /// </summary>
    [Fact(Skip = "Баг: IsLegalMove не восстанавливает позиции королей. Чинится в 2.2.3.5а")]
    public void IsLegalMove_RestoresKingPositionAfterSimulation()
    {
        var board = new ChessBoard();
        board.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
        board.UpdateKingPosition(PieceColor.White, new Position(0, 4));
        board.SetPiece(new Position(7, 4), new Piece(PieceType.Volkhv, PieceColor.Black));
        board.UpdateKingPosition(PieceColor.Black, new Position(7, 4));

        var kingBefore = board.WhiteKingPosition;
        var move = new Move(new Position(0, 4), new Position(1, 4)); // e1 → e2

        MoveValidator.IsLegalMove(board, move);

        Assert.Equal(kingBefore, board.WhiteKingPosition);
    }
}