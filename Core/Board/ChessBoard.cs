using Core.Moves;
using Core.Pieces;

namespace Core.Board;

/// <summary>
/// Представляет шахматную доску 8x8 для таврелей.
/// </summary>
public partial class ChessBoard : IBoard
{
}

public interface IBoard
{
    Piece? GetTopPiece(Position pos);
    List<Piece>? GetStack(Position pos);
    void SetStack(Position pos, List<Piece>? stack);
    void PushPiece(Position pos, Piece piece);
    Piece? PopPiece(Position pos);
    void SetPiece(Position pos, Piece? piece); // оставляем для удобства тестов
    bool MakeMove(Move move);
    void UpdateCastlingRights(Move move);
    bool WhiteCanCastleKingSide { get; }
    bool WhiteCanCastleQueenSide { get; }
    bool BlackCanCastleKingSide { get; }
    bool BlackCanCastleQueenSide { get; }
}