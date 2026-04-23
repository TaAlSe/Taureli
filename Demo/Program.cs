using Core.Board;
using Core.Pieces;

var board = new Board(PieceFactory.StartingPosition());
Console.WriteLine(board.ToString());

// Дополнительно: очистим и поставим одну фигуру для теста
board.Clear();
board.SetPiece(4, 4, new Piece(PieceType.Helgi, PieceColor.White));
Console.WriteLine("\nПосле очистки и установки Хелги в e5:\n");
Console.WriteLine(board.ToString());