using Core.Board;

namespace Core.Moves;

public static class LinearMoveGenerator
{
    /// <summary>
    /// Генерирует все возможные ходы по заданным направлениям, пока не встретится преграда.
    /// </summary>
    /// <param name="board">Текущая доска.</param>
    /// <param name="from">Начальная позиция.</param>
    /// <param name="directions">Массив векторов направлений (dRow, dCol).</param>
    /// <param name="maxDistance">Максимальное количество клеток, которые можно пройти (по умолчанию 7).</param>
    /// <returns>Список позиций, куда можно пойти.</returns>
    public static List<Position> GetMoves(ChessBoard board, Position from, (int dRow, int dCol)[] directions, int maxDistance = 7)
    {
        var moves = new List<Position>();
        var piece = board.GetPiece(from.Row, from.Col);
        if (piece == null) return moves; // нет фигуры — нет ходов

        foreach (var (dRow, dCol) in directions)
        {
            for (int step = 1; step <= maxDistance; step++)
            {
                int newRow = from.Row + dRow * step;
                int newCol = from.Col + dCol * step;
                var targetPos = new Position(newRow, newCol);
                if (!targetPos.IsValid) break; // вышли за доску

                var targetPiece = board.GetPiece(newRow, newCol);
                if (targetPiece == null)
                {
                    // пустая клетка — можно идти
                    moves.Add(targetPos);
                }
                else
                {
                    // клетка занята: если фигура противника — можно взять, иначе остановка
                    if (targetPiece.Color != piece.Color)
                        moves.Add(targetPos);
                    break; // в любом случае дальше не идём
                }
            }
        }
        return moves;
    }
}