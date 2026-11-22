// Core/Board/ChessBoard.Promotion.cs

using Core.Moves;
using Core.Pieces;

namespace Core.Board;

public partial class ChessBoard
{
    /// <summary>
    /// Автоматически превращает ратника на последней горизонтали в фигуру
    /// из его <see cref="Piece.PromotionTarget"/>.
    /// </summary>
    /// <remarks>
    /// Меняется только верхняя фигура башни; всё, что под ней, остаётся на месте.
    /// Новая фигура сохраняет <see cref="Piece.PromotionTarget"/> исходного ратника,
    /// чтобы при обратном превращении можно было восстановить его «родную» линию.
    /// </remarks>
    private void TryPromote(Position pos)
    {
        var top = GetTopPiece(pos);
        if (top is null || top.Type != PieceType.Ratnik || !top.PromotionTarget.HasValue)
            return;

        int lastRow = top.Color == PieceColor.White ? 7 : 0;
        if (pos.Row != lastRow)
            return;

        PopPiece(pos);
        PushPiece(pos, new Piece(top.PromotionTarget.Value, top.Color, top.PromotionTarget)
        {
            IsPromoted = true,
            HasMoved = true
        });
    }

    /// <summary>
    /// Откатывает превращённую фигуру обратно в ратника, сохраняя исходный
    /// <see cref="Piece.PromotionTarget"/> и помечая её как ходившую.
    /// </summary>
    /// <remarks>
    /// Вызывается, когда на превращённую фигуру встаёт своя или вражеская башня
    /// (правило обратного превращения). <see cref="Piece.PromotionTarget"/>
    /// передаётся через конструктор, а не через инициализатор, — так надёжнее
    /// и явно.
    /// </remarks>
    private static Piece RevertToRatnik(Piece promoted) =>
        new(PieceType.Ratnik, promoted.Color, promoted.PromotionTarget)
        {
            IsPromoted = false,
            HasMoved = true
        };
}