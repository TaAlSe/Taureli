// Core/Board/ChessBoard.Castling.cs

using Core.Moves;

namespace Core.Board;

public partial class ChessBoard
{
    /// <summary>
    /// Обновляет права на рокировку в зависимости от того, на какой клетке находится ладья.
    /// Если позиция совпадает с начальной клеткой ладьи, соответствующее право сбрасывается.
    /// </summary>
    /// <param name="pos">Позиция, которую нужно проверить (начальная или конечная клетка хода).</param>
    private void CheckRookPosition(Position pos)
    {
        (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, BlackCanCastleQueenSide, BlackCanCastleKingSide) = pos switch
        {
            (0, 0) => (false, WhiteCanCastleKingSide, BlackCanCastleQueenSide, BlackCanCastleKingSide),
            (0, 7) => (WhiteCanCastleQueenSide, false, BlackCanCastleQueenSide, BlackCanCastleKingSide),
            (7, 0) => (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, false, BlackCanCastleKingSide),
            (7, 7) => (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, BlackCanCastleQueenSide, false),
            _ => (WhiteCanCastleQueenSide, WhiteCanCastleKingSide, BlackCanCastleQueenSide, BlackCanCastleKingSide)
        };
    }

    /// <summary>
    /// Обновляет права на рокировку после выполнения хода.
    /// </summary>
    public void UpdateCastlingRights(Move move)
    {
        // Король
        if (move.From == new Position(0, 4))
        {
            WhiteCanCastleKingSide = false;
            WhiteCanCastleQueenSide = false;
        }
        else if (move.From == new Position(7, 4))
        {
            BlackCanCastleKingSide = false;
            BlackCanCastleQueenSide = false;
        }

        // Ладья на начальной клетке (двигалась)
        CheckRookPosition(move.From);
        // Ладья на конечной клетке (была взята)
        CheckRookPosition(move.To);
    }
}