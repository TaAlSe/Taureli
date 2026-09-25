// Tests/Core/Moves/HelgiMoveGeneratorTests.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

namespace Tests.Core.Moves;

public class HelgiMoveGeneratorTests
{
    /// <summary>
    /// Проверяет, что хелги на пустой доске имеет ходы как у князя (ферзя) и всадника (коня),
    /// общее количество ходов = 27 (князь) + 8 (конь) = 35.
    /// </summary>
    [Fact]
    public void Helgi_OnEmptyBoard_HasQueenAndKnightMoves()
    {
        var board = new ChessBoard();
        var helgi = new Piece(PieceType.Helgi, PieceColor.White);
        board.SetPiece(3, 3, helgi);

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));
        // На пустой доске: князь даёт 27, конь — 8, итого 35
        Assert.Equal(35, moves.Count);
        // Проверим наличие коневого хода
        Assert.Contains(moves, m => m.To == new Position(5, 4));
        // и княжеского
        Assert.Contains(moves, m => m.To == new Position(7, 7));
    }

    /// <summary>
    /// Проверяет, что для хелги линейная часть (князь) блокируется своими фигурами,
    /// а всадническая часть (конь) перепрыгивает через них.
    /// </summary>
    [Fact]
    public void Helgi_BlockedByOwnPiece_StopsForQueenPart_ButKnightJumps()
    {
        var board = new ChessBoard();
        var helgi = new Piece(PieceType.Helgi, PieceColor.White);
        board.SetPiece(3, 3, helgi);
        var ownBlock = new Piece(PieceType.Ratnik, PieceColor.White);
        board.SetPiece(5, 5, ownBlock); // f6 — блокирует диагональ князя
        // Но конь всё равно может прыгнуть на (5,4) и (5,2) и т.д.

        var moves = MoveGenerator.GetMovesForPiece(board, new Position(3, 3));
        // Проверяем, что ход на (5,5) отсутствует (заблокирован)
        Assert.DoesNotContain(moves, m => m.To == new Position(5, 5));
        // Но конь может прыгнуть на (5,4) — есть
        Assert.Contains(moves, m => m.To == new Position(5, 4));
    }
}