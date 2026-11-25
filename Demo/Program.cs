// Demo/Program.cs

using Core.Board;
using Core.Moves;
using Core.Pieces;

// Демонстрация Position
Console.WriteLine("=== Position ===");
var pos = Position.FromAlgebraic("e4");
Console.WriteLine($"e4 -> Row={pos.Row}, Col={pos.Col}, обратно: {pos.ToAlgebraic()}");

// Демонстрация Move
Console.WriteLine("\n=== Move ===");
var from = new Position(1, 4); // e2
var to = new Position(3, 4);   // e4
var move = new Move(from, to);
Console.WriteLine($"Ход: {move.From.ToAlgebraic()} -> {move.To.ToAlgebraic()}");

// Доска
var board = new ChessBoard(PieceFactory.StartingPosition());
Console.WriteLine("\n=== Доска ===");
Console.WriteLine(board.ToString());

// Генератор ходов (единый)
Console.WriteLine("\n=== Генератор ходов (ладья) ===");
var rookBoard = new ChessBoard();
rookBoard.SetPiece(3, 3, new Piece(PieceType.Ratoborets, PieceColor.White));
var rookMoves = MoveGenerator.GetMovesForPiece(rookBoard, new Position(3, 3));
Console.WriteLine($"Ладья на d4 может пойти на {rookMoves.Count} клеток:");
Console.WriteLine(string.Join(", ", rookMoves.Select(p => p.To.ToAlgebraic())));

Console.WriteLine("\n=== Генератор ходов (слон) ===");
var bishopBoard = new ChessBoard();
bishopBoard.SetPiece(3, 3, new Piece(PieceType.Archer, PieceColor.White));
var bishopMoves = MoveGenerator.GetMovesForPiece(bishopBoard, new Position(3, 3));
Console.WriteLine($"Слон на d4 может пойти на {bishopMoves.Count} клеток:");
Console.WriteLine(string.Join(", ", bishopMoves.Select(p => p.To.ToAlgebraic())));

Console.WriteLine("\n=== Генератор ходов (князь) ===");
var queenBoard = new ChessBoard();
queenBoard.SetPiece(3, 3, new Piece(PieceType.Knyaz, PieceColor.White));
var queenMoves = MoveGenerator.GetMovesForPiece(queenBoard, new Position(3, 3));
Console.WriteLine($"Князь на d4 может пойти на {queenMoves.Count} клеток:");
Console.WriteLine(string.Join(", ", queenMoves.Select(p => p.To.ToAlgebraic())));

Console.WriteLine("\n=== Генератор ходов (всадник) ===");
var knightBoard = new ChessBoard();
knightBoard.SetPiece(3, 3, new Piece(PieceType.Horseman, PieceColor.White));
var knightMoves = MoveGenerator.GetMovesForPiece(knightBoard, new Position(3, 3));
Console.WriteLine($"Всадник на d4 может пойти на {knightMoves.Count} клеток:");
Console.WriteLine(string.Join(", ", knightMoves.Select(p => p.To.ToAlgebraic())));

Console.WriteLine("\n=== Генератор ходов (хелги) ===");
var helgiBoard = new ChessBoard();
helgiBoard.SetPiece(3, 3, new Piece(PieceType.Helgi, PieceColor.White));
var helgiMoves = MoveGenerator.GetMovesForPiece(helgiBoard, new Position(3, 3));
Console.WriteLine($"Хелги на d4 может пойти на {helgiMoves.Count} клеток:");
Console.WriteLine(string.Join(", ", helgiMoves.Select(p => p.To.ToAlgebraic())));

Console.WriteLine("\n=== Генератор ходов (пешка) ===");
var pawnBoard = new ChessBoard();
pawnBoard.SetPiece(1, 4, new Piece(PieceType.Ratnik, PieceColor.White));
var pawnMoves = MoveGenerator.GetMovesForPiece(pawnBoard, new Position(1, 4));
Console.WriteLine($"Белая пешка на e2 может пойти на {pawnMoves.Count} клеток:");
foreach (var pawnMove in pawnMoves)
    Console.WriteLine($"  {pawnMove.From.ToAlgebraic()} -> {pawnMove.To.ToAlgebraic()}");

Console.WriteLine("\n=== Превращение ===");
var promoBoard = new ChessBoard();
promoBoard.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
promoBoard.UpdateKingPosition(PieceColor.White, new Position(0, 4));
promoBoard.SetPiece(new Position(7, 4), new Piece(PieceType.Volkhv, PieceColor.Black));
promoBoard.UpdateKingPosition(PieceColor.Black, new Position(7, 4));
promoBoard.SetPiece(new Position(6, 4),
    new Piece(PieceType.Ratnik, PieceColor.White, PieceType.Helgi));

Console.WriteLine("До: Ratnik на e7 (цель — Helgi)");
promoBoard.MakeMove(new Move(new Position(6, 4), new Position(7, 4)));
var promoted = promoBoard.GetTopPiece(new Position(7, 4));
Console.WriteLine($"После: {promoted?.Type} на e8, IsPromoted={promoted?.IsPromoted}");

Console.WriteLine("\n=== Башня ===");
var stackBoard = new ChessBoard();
stackBoard.SetPiece(new Position(0, 4), new Piece(PieceType.Volkhv, PieceColor.White));
stackBoard.UpdateKingPosition(PieceColor.White, new Position(0, 4));
stackBoard.SetPiece(new Position(7, 4), new Piece(PieceType.Volkhv, PieceColor.Black));
stackBoard.UpdateKingPosition(PieceColor.Black, new Position(7, 4));

// Башня из 3 фигур на d4: снизу Ratoborets, посередине Horseman, сверху Ratnik.
stackBoard.SetStack(new Position(3, 3), [
    new Piece(PieceType.Ratoborets, PieceColor.White),
    new Piece(PieceType.Horseman,   PieceColor.White),
    new Piece(PieceType.Ratnik,     PieceColor.White, PieceType.Helgi)
]);

Console.WriteLine(stackBoard.ToString());

Console.WriteLine("\n=== Дамп стеков ===");
Console.WriteLine(stackBoard.DumpStacks());