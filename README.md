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
- `Piece.cs` — record (Type, Color, PromotionTarget).
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

## Текущее состояние

- [x] 1.1–1.9.2 — базовые сущности, генераторы, валидация, рокировка.
- [x] 2.1.0–2.1.4 — башни (стеки), разделение, слияние, тесты.
- [x] 2.2.1 — хранение `PromotionTarget` в пешке, обновление `PieceFactory` и `PawnMoveGenerator`.
- [ ] 2.2.2 — реализация превращения в `MakeMove`.
- [ ] 2.2.3 — взятие на проходе.
- [ ] 2.2.4 — превращение при разделении башни (опционально).
- [ ] 3.1 — WPF UI.
- [ ] 3.2 — сохранение/загрузка PGN.

## План на шаг 2.2 – Особые правила пешки

### 2.2.2 – Реализация превращения в `MakeMove` и `ExecuteMoveInternal`
- При выполнении хода, если `PromotionType` задан, заменить пешку на новую фигуру.
- Учесть превращение при разделении башни (если верхняя фигура – пешка и остаётся на последней горизонтали).

### 2.2.3 – Взятие на проходе
- Добавить в `ChessBoard` поле `LastMove`.
- В `PawnMoveGenerator` проверять возможность взятия на проходе.
- В `ExecuteMoveInternal` обрабатывать удаление пешки.

### 2.2.4 – Превращение при разделении башни (опционально)
- Реализовать позже.

## Планы на будущее

- **Магистерская работа**: «Разработка и исследование алгоритмов поиска и оценки позиций для игры таврели». После MVP — оптимизация под движки (битовые доски, alpha-beta).
- **MVP2**: оптимизация логики, функциональные требования к тестам.
- **Функциональные требования**: после стабилизации MVP составить матрицу «требование → тест».

## Как запустить

1. Собрать решение.
2. Назначить `Demo` запускаемым.
3. Запустить — выводятся примеры ходов для всех фигур.

## Контекст для продолжения

Завершён подшаг 2.2.1. Все 57 тестов зелёные.  
Следующий шаг — 2.2.2: реализация превращения в `MakeMove` и `ExecuteInternal`.