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
var promotionMove = new Move(from, to, PieceType.Helgi);
Console.WriteLine($"Ход с превращением: {promotionMove.From} -> {promotionMove.To}, в {promotionMove.PromotionType}");

// Можно также показать доску (уже есть)
var board = new ChessBoard(PieceFactory.StartingPosition());
Console.WriteLine("\n=== Доска ===");
Console.WriteLine(board.ToString());