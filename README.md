# Taureli — ядро для русских шахмат (таврелей)

MVP десктопного приложения для игры в таврели. Реализована полная логика ходов всех фигур, включая башни (стеки), рокировку, проверку шаха и валидацию легальности. Ведётся работа над особыми правилами пешки.

## Архитектура

- **Core** — ядро (фигуры, доска, генерация ходов, валидация, атака).
- **Data** — (пусто, для хранения партий).
- **UI** — (будет WPF-интерфейс).
- **Tests** — юнит-тесты (xUnit).
- **Demo** — консольная демонстрация.

## Структура Core

### Board (partial class ChessBoard)
- `ChessBoard.Core.cs` — поля, конструкторы, свойства, `ToString`, `FindKing`.
- `ChessBoard.Stack.cs` — работа со стеками (`GetTopPiece`, `GetStack`, `SetStack`, `PushPiece`, `PopPiece`, `SetPiece`, `ConcatStacks`, `SplitStack`).
- `ChessBoard.Castling.cs` — права рокировки, `CheckRookPosition`, `UpdateCastlingRights`.
- `ChessBoard.Moves.cs` — `MakeMove`, `ExecuteMoveInternal`, `ExecuteCastling`.
- `ChessBoard.King.cs` — `UpdateKingPosition`.

### Pieces
- `PieceColor.cs` — enum (White, Black).
- `PieceType.cs` — типы фигур (Volkhv, Horseman, Helgi, Knyaz, Ratoborets, Archer, Ratnik).
- `Piece.cs` — record (Type, Color, PromotionTarget, IsPromoted, HasMoved).
- `PieceFactory.cs` — начальная расстановка.

### Moves
- `Position.cs` — координаты (Row, Col).
- `Move.cs` — класс хода (From, To, PromotionType, IsCastling, CastlingSide, SplitCount).
- `Directions.cs` — направления.
- `LinearMoveGenerator.cs` — линейные ходы.
- `PawnMoveGenerator.cs` — ходы пешки.
- `CastlingMoveGenerator.cs` — рокировка.
- `MoveGenerator.cs` — единая точка входа.

### Attack
- `AttackDetector.cs` — проверка атаки клетки.

### Validation
- `MoveValidator.cs` — `IsLegalMove`, `GetLegalMoves`.

## Дорожная карта MVP

### Сделано

- [x] 1.1–1.9.2 — базовые сущности, генераторы, валидация, рокировка.
- [x] 2.1.0–2.1.4 — башни (стеки), разделение, слияние, тесты.
- [x] 2.2.1 — хранение `PromotionTarget` в пешке, обновление `PieceFactory` и `PawnMoveGenerator`.
- [x] 2.2.1а — `IsPromoted` и `HasMoved` в `Piece` с документацией.
- [x] 2.2.1б — `PawnMoveGenerator`: запрет двойного хода для ратника, который уже ходил (`!piece.HasMoved`).
- [x] 2.2.1в — `PawnMoveGenerator`: разрешён ход на свою башню (вперёд на 1 или 2 клетки при пустой промежуточной).

### 2.2.2. Превращение ратника

- [ ] 2.2.2.1. `Core/Board/ChessBoard.Promotion.cs`: `TryPromote` и `RevertToRatnik`.
- [ ] 2.2.2.2. `Core/Board/ChessBoard.Stack.cs`: `MarkMoved(Position, int)` — помечает `HasMoved = true` для верхних `movedCount` фигур через `with`.
- [ ] 2.2.2.3. `Core/Board/ChessBoard.Moves.cs`:
  - [ ] удалить ветку `else if (move.PromotionType.HasValue)`;
  - [ ] откат превращённой фигуры под башней (`targetStack[^1].IsPromoted`) — **до** `ConcatStacks`;
  - [ ] вызвать `MarkMoved(move.To, movingStack.Count)`;
  - [ ] вызвать `TryPromote(move.To)`.
- [ ] 2.2.2.4. `Core/Board/ChessBoard.Core.cs`: поле `public Move? LastMove { get; private set; }`; в `MakeMove` — `LastMove = move;` в конце.
- [ ] 2.2.2.5. `Core/Moves/PawnMoveGenerator.cs`: убрать `promotionRow` и `promotionType`.
- [ ] 2.2.2.6. `Core/Moves/MoveGenerator.cs`: в блоке split убрать проверку `move.PromotionType.HasValue` и третий аргумент `new Move(from, to)`.
- [ ] 2.2.2.7. `Core/Moves/Move.cs`: удалить `PromotionType` (поле, параметр, `Equals`/`GetHashCode`/`ToString`); `Shift+F12` → поправить `Demo/Program.cs`, `MoveTests.cs`, `PawnMoveGeneratorTests.cs`; `Ctrl+Shift+B`.
- [ ] 2.2.2.8. Тесты:
  - [ ] переписать `PawnMoveGeneratorTests`;
  - [ ] новый `Tests/Core/Board/PromotionTests.cs`:
    - [ ] `PieceFactory_AllPawnsHavePromotionTarget`;
    - [ ] `Ratnik_ReachesLastRank_PromotesToTarget`;
    - [ ] `Ratnik_OnTopOfStack_Promotes_StackBelowUnchanged`;
    - [ ] `Ratnik_SplitFromStack_Promotes`;
    - [ ] `PromotedPiece_CoveredByFriendlyStack_RevertsToRatnik`;
    - [ ] `PromotedPiece_CoveredByEnemyStack_RevertsToRatnik`;
    - [ ] `RevertToRatnik_PreservesPromotionTarget`;
    - [ ] `Ratnik_ReturnsToStartRank_LosesDoubleMove`.
- [ ] 2.2.2.9. README: отметить 2.2.2 как выполненный.

> **Примечание:** 2.2.4 «Превращение при разделении башни» из старого плана поглощается 2.2.2 — автоматическое превращение верхней фигуры работает и при split, отдельный пункт не нужен.

### 2.2.3. Взятие на проходе (пленение)

- [ ] 2.2.3.1. `Core/Moves/Move.cs`: `public bool IsEnPassant { get; init; } = false;`, учесть в `Equals`/`GetHashCode`/`ToString`.
- [ ] 2.2.3.2. `Core/Moves/PawnMoveGenerator.cs`: генерация e.p. (проверка `board.LastMove`, двойного хода Ratnik’а, горизонтали, соседней вертикали, пустой целевой клетки).
- [ ] 2.2.3.3. `Core/Board/ChessBoard.Moves.cs`: ветка `if (move.IsEnPassant)` **до** общей ветки (снять нашу пешку с `From`, снять вражескую башню с `enemyPos`, откатить `IsPromoted` при необходимости, собрать новую башню на `To`).
- [ ] 2.2.3.4. `Core/Moves/MoveGenerator.cs`: в split пробрасывать `IsEnPassant = move.IsEnPassant`.
- [ ] 2.2.3.5. `Core/Validation/MoveValidator.cs`: при `move.IsEnPassant` сохранять/восстанавливать стек `enemyPos`.
- [ ] 2.2.3.6. Тесты `Tests/Core/Board/EnPassantTests.cs`:
  - [ ] `BasicEnPassant_CapturesWholeStack`;
  - [ ] `EnPassant_WhenEnemyPawnOnTopOfStack_CapturesWholeStack`;
  - [ ] `EnPassant_NotAvailableAfterAnotherMove`;
  - [ ] `EnPassant_NotAvailableIfLastMoveNotDoublePawn`;
  - [ ] `EnPassant_NotAvailableIfTargetOccupied`;
  - [ ] `EnPassant_NotAvailableIfPawnNotOnCorrectRank`;
  - [ ] `EnPassant_PinnedToKing_IsIllegal`;
  - [ ] `EnPassant_CapturingPawnMarkedHasMoved`.
- [ ] 2.2.3.7. README: отметить 2.2.3 как выполненный.

### 3.x. MVP2 и далее

- [ ] 3.1 — WPF UI.
- [ ] 3.2 — сохранение/загрузка PGN (или собственного формата `.tav`).
- [ ] Рефакторинг: заменить флаги `WhiteCanCastle*`/`BlackCanCastle*` на `HasMoved` + поиск короля/ладьи (после стабилизации `HasMoved`).
- [ ] Рефакторинг `PieceFactory`: массив `pawnPromotions` вместо `backRowTypes` + `if (col == 4)`.
- [ ] Полный снапшот доски в `MoveValidator` (если появятся правила, трогающие >2 квадратов).

## Планы на будущее

- **Магистерская работа**: «Разработка и исследование алгоритмов поиска и оценки позиций для игры таврели». После MVP — оптимизация под движки (битовые доски, alpha-beta).
- **MVP2**: оптимизация логики, функциональные требования к тестам.
- **Функциональные требования**: после стабилизации MVP составить матрицу «требование → тест».

## Как запустить

1. Собрать решение.
2. Назначить `Demo` запускаемым.
3. Запустить — выводятся примеры ходов для всех фигур.