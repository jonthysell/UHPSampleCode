using System;
using System.Collections.Generic;
using System.Text;

namespace SampleEngine
{
    class Board
    {
        public BoardState BoardState { get; private set; } = BoardState.NotStarted;

        public bool GameInProgress => BoardState == BoardState.NotStarted || BoardState == BoardState.InProgress;

        public bool GameIsOver => BoardState == BoardState.WhiteWins || BoardState == BoardState.BlackWins || BoardState == BoardState.Draw;

        public int CurrentTurn { get; private set; } = 0;

        public int CurrentPlayerTurn => 1 + CurrentTurn / 2;

        public Color CurrentColor => (Color)(CurrentTurn % (int)Color.NumColors);

        public bool CurrentTurnQueenInPlay => PieceInPlay(CurrentColor == Color.White ? PieceName.wQ : PieceName.bQ);

        private Position[] m_piecePositions = new Position[(int)PieceName.NumPieceNames];

        private List<Move> m_moveHistory = new List<Move>();
        private List<string> m_moveHistoryStr = new List<string>();

        private MoveSet? m_cachedValidMoves = null;
        private PositionSet? m_cachedValidPlacements = null;

        public Board()
        {
            for (int pn = 0; pn < (int)PieceName.NumPieceNames; pn++)
            {
                m_piecePositions[pn] = new Position()
                {
                    Q = 0,
                    R = 0,
                    Stack = -1,
                };
            }
        }

        public string GetGameString()
        {
            var sb = new StringBuilder();

            sb.Append($"Base;{BoardState};{CurrentColor}[{CurrentPlayerTurn}]");

            foreach (var moveStr in m_moveHistoryStr)
            {
                sb.Append($";{moveStr}");
            }

            return sb.ToString();
        }

        public MoveSet GetValidMoves()
        {
            if (m_cachedValidMoves is null)
            {
                m_cachedValidMoves = new MoveSet();

                if (GameInProgress)
                {
                    for (int pn = 0; pn < (int)PieceName.NumPieceNames; pn++)
                    {
                        GetValidMoves((PieceName)pn, m_cachedValidMoves);
                    }

                    if (m_cachedValidMoves.Count == 0)
                    {
                        m_cachedValidMoves.Add(Move.PassMove);
                    }
                }
            }

            return m_cachedValidMoves;
        }

        public bool TryPlayMove(Move move, string moveString)
        {
            var validMoves = GetValidMoves();

            if (validMoves.Contains(move))
            {
                m_moveHistoryStr.Add(moveString);
                TrustedPlay(move);
                return true;
            }

            return false;
        }

        public bool TryUndoLastMove()
        {
            if (m_moveHistory.Count > 0)
            {
                var lastMove = m_moveHistory[m_moveHistory.Count - 1];

                if (lastMove != Move.PassMove)
                {
                    m_piecePositions[(int)lastMove.PieceName] = lastMove.Source;
                }

                m_moveHistory.RemoveAt(m_moveHistory.Count - 1);
                m_moveHistoryStr.RemoveAt(m_moveHistoryStr.Count - 1);

                CurrentTurn--;

                ResetState();
                ResetCaches();

                return true;
            }

            return false;
        }

        public bool TryGetMoveString(Move move, out string? result)
        {
            if (move == Move.PassMove)
            {
                result = Constants.PassMoveString;
                return true;
            }

            var startPiece = move.PieceName.ToString();

            if (CurrentTurn == 0 && move.Destination == Position.OriginPosition)
            {
                result = startPiece;
                return true;
            }

            var endPiece = "";

            if (move.Destination.Stack > 0)
            {
                PieceName pieceBelow = GetPieceAt(move.Destination.GetBelow());
                endPiece = pieceBelow.ToString();
            }
            else
            {
                for (int dir = 0; dir < (int)Direction.NumDirections; dir++)
                {
                    Position neighborPosition = move.Destination.GetNeighborAt((Direction)dir);
                    PieceName neighbor = GetPieceOnTopAt(neighborPosition);

                    if (neighbor != PieceName.INVALID && neighbor != move.PieceName)
                    {
                        endPiece = neighbor.ToString();
                        switch (dir)
                        {
                            case 0: // Up
                                endPiece = endPiece + "\\";
                                break;
                            case 1: // UpRight
                                endPiece = "/" + endPiece;
                                break;
                            case 2: // DownRight
                                endPiece = "-" + endPiece;
                                break;
                            case 3: // Down
                                endPiece = "\\" + endPiece;
                                break;
                            case 4: // DownLeft
                                endPiece = endPiece + "/";
                                break;
                            case 5: // UpLeft
                                endPiece = endPiece + "-";
                                break;
                        }
                        break;
                    }
                }
            }

            if (endPiece != "")
            {
                result = $"{startPiece} {endPiece}";
                return true;
            }

            result = default;
            return false;
        }

        public bool TryParseMove(string moveString, out Move? result, out string? resultString)
        {
            if (Move.TryNormalizeMoveString(moveString, out bool isPass, out PieceName startPiece, out char beforeSeperator, out PieceName endPiece, out char afterSeperator))
            {
                resultString = Move.BuildMoveString(isPass, startPiece, beforeSeperator, endPiece, afterSeperator);

                if (isPass)
                {
                    result = Move.PassMove;
                    return true;
                }

                Position source = m_piecePositions[(int)startPiece];

                Position destination = Position.OriginPosition;

                if (endPiece != PieceName.INVALID)
                {
                    Position targetPosition = m_piecePositions[(int)endPiece];

                    if (beforeSeperator != '\0')
                    {
                        // Moving piece on the left-hand side of the target piece
                        switch (beforeSeperator)
                        {
                            case '-':
                                destination = targetPosition.GetNeighborAt(Direction.UpLeft).GetBottom();
                                break;
                            case '/':
                                destination = targetPosition.GetNeighborAt(Direction.DownLeft).GetBottom();
                                break;
                            case '\\':
                                destination = targetPosition.GetNeighborAt(Direction.Up).GetBottom();
                                break;
                        }
                    }
                    else if (afterSeperator != '\0')
                    {
                        // Moving piece on the right-hand side of the target piece
                        switch (afterSeperator)
                        {
                            case '-':
                                destination = targetPosition.GetNeighborAt(Direction.DownRight).GetBottom();
                                break;
                            case '/':
                                destination = targetPosition.GetNeighborAt(Direction.UpRight).GetBottom();
                                break;
                            case '\\':
                                destination = targetPosition.GetNeighborAt(Direction.Down).GetBottom();
                                break;
                        }
                    }
                    else
                    {
                        destination = targetPosition.GetAbove();
                    }
                }

                result = new Move()
                {
                    PieceName = startPiece,
                    Source = source,
                    Destination = destination
                };
                return true;
            }

            result = default;
            resultString = default;
            return false;
        }

        private void GetValidMoves(PieceName pieceName, MoveSet moveSet)
        {
            if (pieceName != PieceName.INVALID && GameInProgress && CurrentColor == Enums.GetColor(pieceName) && PlacingPieceInOrder(pieceName))
            {
                int pieceIndex = (int)pieceName;

                if (CurrentTurn == 0)
                {
                    // First turn by white
                    if (pieceName != PieceName.wQ)
                    {
                        moveSet.Add(new Move()
                        {
                            PieceName = pieceName,
                            Source = m_piecePositions[pieceIndex],
                            Destination = Position.OriginPosition
                        });
                    }
                }
                else if (CurrentTurn == 1)
                {
                    // First turn by black
                    if (pieceName != PieceName.bQ)
                    {
                        var validPlacements = GetValidPlacements();
                        foreach (var placement in validPlacements)
                        {
                            moveSet.Add(new Move()
                            {
                                PieceName = pieceName,
                                Source = m_piecePositions[pieceIndex],
                                Destination = placement
                            });
                        }
                    }
                }
                else if (PieceInHand(pieceName))
                {
                    // Piece is in hand
                    if ((CurrentPlayerTurn != 4 ||
                         (CurrentPlayerTurn == 4 &&
                          (CurrentTurnQueenInPlay || (!CurrentTurnQueenInPlay && Enums.GetBugType(pieceName) == BugType.QueenBee)))))
                    {
                        var validPlacements = GetValidPlacements();
                        foreach (var placement in validPlacements)
                        {
                            moveSet.Add(new Move()
                            {
                                PieceName = pieceName,
                                Source = m_piecePositions[pieceIndex],
                                Destination = placement
                            });
                        }
                    }
                    else if (CurrentTurnQueenInPlay && CanMoveWithoutBreakingHive(pieceName))
                    {
                        // Piece is in play and movement is allowed
                        switch (Enums.GetBugType(pieceName))
                        {
                            case BugType.QueenBee:
                                GetValidQueenBeeMoves(pieceName, moveSet);
                                break;
                            case BugType.Spider:
                                GetValidSpiderMoves(pieceName, moveSet);
                                break;
                            case BugType.Beetle:
                                GetValidBeetleMoves(pieceName, moveSet);
                                break;
                            case BugType.Grasshopper:
                                GetValidGrasshopperMoves(pieceName, moveSet);
                                break;
                            case BugType.SoldierAnt:
                                GetValidSoldierAntMoves(pieceName, moveSet);
                                break;
                        }
                    }
                }
            }
        }

        private PositionSet GetValidPlacements()
        {
            if (m_cachedValidPlacements is null)
            {
                m_cachedValidPlacements = new PositionSet();

                if (CurrentTurn == 0)
                {
                    m_cachedValidPlacements.Add(Position.OriginPosition);
                }
                else if (CurrentTurn == 1)
                {
                    for (int dir = 0; dir < (int)Direction.NumDirections; dir++)
                    {
                        m_cachedValidPlacements.Add(Position.OriginPosition.GetNeighborAt((Direction)dir));
                    }
                }
                else
                {
                    var visitedPlacements = new PositionSet();

                    for (int pn = 0; pn < (int)PieceName.NumPieceNames; pn++)
                    {
                        var pieceName = (PieceName)pn;

                        if (PieceIsOnTop(pieceName) && CurrentColor == Enums.GetColor(pieceName))
                        {
                            var bottomPosition = m_piecePositions[pn].GetBottom();
                            visitedPlacements.Add(bottomPosition);

                            for (int dir = 0; dir < (int)Direction.NumDirections; dir++)
                            {
                                var neighbor = bottomPosition.GetNeighborAt((Direction)dir);

                                if (!visitedPlacements.Contains(neighbor) && !HasPieceAt(neighbor))
                                {
                                    visitedPlacements.Add(neighbor);

                                    // Neighboring position is a potential, verify its neighbors are empty or same color

                                    bool validPlacement = true;
                                    for (int dir2 = 0; dir2 < (int)Direction.NumDirections; dir2++)
                                    {
                                        var surroundingPosition = neighbor.GetNeighborAt((Direction)dir2);
                                        var surroundingPiece = GetPieceOnTopAt(surroundingPosition);
                                        if (surroundingPiece != PieceName.INVALID && Enums.GetColor(surroundingPiece) != CurrentColor)
                                        {
                                            validPlacement = false;
                                            break;
                                        }
                                    }

                                    if (validPlacement)
                                    {
                                        m_cachedValidPlacements.Add(neighbor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return m_cachedValidPlacements;
        }

        private void GetValidQueenBeeMoves(PieceName pieceName, MoveSet moveSet)
        {
            GetValidSlides(pieceName, moveSet, 1);
        }

        private void GetValidSpiderMoves(PieceName pieceName, MoveSet moveSet)
        {
            GetValidSlides(pieceName, moveSet, 3);

            var upToTwo = new MoveSet();
            GetValidSlides(pieceName, upToTwo, 2);

            foreach (var move in upToTwo)
            {
                moveSet.Remove(move);
            }
        }

        private void GetValidBeetleMoves(PieceName pieceName, MoveSet moveSet)
        {
            // Look in all directions
            for (int direction = 0; direction < (int)Direction.NumDirections; direction++)
            {
                var newPosition = m_piecePositions[(int)pieceName].GetNeighborAt((Direction)direction);

                var topNeighbor = GetPieceOnTopAt(newPosition);

                // Get positions to left and right or direction we're heading
                var leftOfTarget = Enums.LeftOf((Direction)direction);
                var rightOfTarget = Enums.RightOf((Direction)direction);
                var leftNeighborPosition = m_piecePositions[(int)pieceName].GetNeighborAt(leftOfTarget);
                var rightNeighborPosition = m_piecePositions[(int)pieceName].GetNeighborAt(rightOfTarget);

                var topLeftNeighbor = GetPieceOnTopAt(leftNeighborPosition);
                var topRightNeighbor = GetPieceOnTopAt(rightNeighborPosition);

                // At least one neighbor is present
                uint currentHeight = (uint)(m_piecePositions[(int)pieceName].Stack + 1);
                uint destinationHeight = (uint)(topNeighbor != PieceName.INVALID ? m_piecePositions[(int)topNeighbor].Stack + 1 : 0);

                uint topLeftNeighborHeight = (uint)(topLeftNeighbor != PieceName.INVALID ? m_piecePositions[(int)topLeftNeighbor].Stack + 1 : 0);
                uint topRightNeighborHeight = (uint)(topRightNeighbor != PieceName.INVALID ? m_piecePositions[(int)topRightNeighbor].Stack + 1 : 0);

                // "Take-off" beetle
                currentHeight--;

                if (!(currentHeight == 0 && destinationHeight == 0 && topLeftNeighborHeight == 0 && topRightNeighborHeight == 0))
                {
                    // Logic from http://boardgamegeek.com/wiki/page/Hive_FAQ#toc9
                    if (!(destinationHeight < topLeftNeighborHeight && destinationHeight < topRightNeighborHeight && currentHeight < topLeftNeighborHeight && currentHeight < topRightNeighborHeight))
                    {
                        var targetMove = new Move()
                        {
                            PieceName = pieceName,
                            Source = m_piecePositions[(int)pieceName],
                            Destination = new Position()
                            {
                                Q = newPosition.Q,
                                R = newPosition.R,
                                Stack = (int)destinationHeight
                            }
                        };
                        moveSet.Add(targetMove);
                    }
                }
            }
        }

        private void GetValidGrasshopperMoves(PieceName pieceName, MoveSet moveSet)
        {
            var startingPosition = m_piecePositions[(int)pieceName];

            for (int dir = 0; dir < (int)Direction.NumDirections; dir++)
            {
                var landingPosition = startingPosition.GetNeighborAt((Direction)dir);

                int distance = 0;
                while (HasPieceAt(landingPosition))
                {
                    // Jump one more in the same direction
                    landingPosition = landingPosition.GetNeighborAt((Direction)dir);
                    distance++;
                }

                if (distance > 0)
                {
                    // Can only move if there's at least one piece in the way
                    moveSet.Add(new Move()
                    {
                        PieceName = pieceName,
                        Source = startingPosition,
                        Destination = landingPosition
                    });
                }
            }
        }

        private void GetValidSoldierAntMoves(PieceName pieceName, MoveSet moveSet)
        {
            GetValidSlides(pieceName, moveSet, -1);
        }

        private void GetValidSlides(PieceName pieceName, MoveSet moveSet, int maxRange)
        {
            var startingPosition = m_piecePositions[(int)pieceName];

            var visitedPositions = new PositionSet();
            visitedPositions.Add(startingPosition);

            m_piecePositions[(int)pieceName].Stack = -1;
            GetValidSlides(pieceName, moveSet, startingPosition, startingPosition, visitedPositions, 0, maxRange);
            m_piecePositions[(int)pieceName].Stack = startingPosition.Stack;
        }

        private void GetValidSlides(PieceName pieceName, MoveSet moveSet, Position startingPosition, Position currentPosition, PositionSet visitedPositions, int currentRange, int maxRange)
        {
            if (maxRange < 0 || currentRange < maxRange)
            {
                for (int slideDirection = 0; slideDirection < (int)Direction.NumDirections; slideDirection++)
                {
                    var slidePosition = currentPosition.GetNeighborAt((Direction)slideDirection);

                    if (!visitedPositions.Contains(slidePosition) && !HasPieceAt(slidePosition))
                    {
                        // Slide position is open

                        var left = Enums.LeftOf((Direction)slideDirection);
                        var right = Enums.RightOf((Direction)slideDirection);

                        if (HasPieceAt(currentPosition.GetNeighborAt(right)) != HasPieceAt(currentPosition.GetNeighborAt(left)))
                        {
                            // Can slide into slide position
                            var move = new Move()
                            {
                                PieceName = pieceName,
                                Source = startingPosition,
                                Destination = slidePosition
                            };

                            if (moveSet.Add(move))
                            {
                                visitedPositions.Add(slidePosition);
                                GetValidSlides(pieceName, moveSet, startingPosition, slidePosition, visitedPositions, currentRange + 1, maxRange);
                            }
                        }
                    }
                }
            }
        }

        private void TrustedPlay(Move move)
        {
            m_moveHistory.Add(move);

            if (move != Move.PassMove)
            {
                m_piecePositions[(int)move.PieceName] = move.Destination;
            }

            CurrentTurn++;

            ResetState();
            ResetCaches();
        }

        private bool PlacingPieceInOrder(PieceName pieceName)
        {
            if (PieceInHand(pieceName))
            {
                switch (pieceName)
                {
                    case PieceName.wS2:
                        return PieceInPlay(PieceName.wS1);
                    case PieceName.wB2:
                        return PieceInPlay(PieceName.wB1);
                    case PieceName.wG2:
                        return PieceInPlay(PieceName.wG1);
                    case PieceName.wG3:
                        return PieceInPlay(PieceName.wG2);
                    case PieceName.wA2:
                        return PieceInPlay(PieceName.wA1);
                    case PieceName.wA3:
                        return PieceInPlay(PieceName.wA2);
                    case PieceName.bS2:
                        return PieceInPlay(PieceName.bS1);
                    case PieceName.bB2:
                        return PieceInPlay(PieceName.bB1);
                    case PieceName.bG2:
                        return PieceInPlay(PieceName.bG1);
                    case PieceName.bG3:
                        return PieceInPlay(PieceName.bG2);
                    case PieceName.bA2:
                        return PieceInPlay(PieceName.bA1);
                    case PieceName.bA3:
                        return PieceInPlay(PieceName.bA2);
                }
            }

            return true;
        }

        private PieceName GetPieceAt(Position position)
        {
            for (int pn = 0; pn < (int)PieceName.NumPieceNames; pn++)
            {
                if (m_piecePositions[pn] == position)
                {
                    return (PieceName)pn;
                }
            }

            return PieceName.INVALID;
        }

        private PieceName GetPieceOnTopAt(Position position)
        {
            var currentPosition = position.GetBottom();

            var topPiece = GetPieceAt(currentPosition);

            if (topPiece != PieceName.INVALID)
            {
                while (true)
                {
                    currentPosition = currentPosition.GetAbove();
                    var nextPiece = GetPieceAt(currentPosition);
                    if (nextPiece == PieceName.INVALID)
                    {
                        break;
                    }
                    topPiece = nextPiece;
                }
            }

            return topPiece;
        }

        private bool HasPieceAt(Position position)
        {
            return GetPieceAt(position) != PieceName.INVALID;
        }

        private bool PieceInHand(PieceName pieceName)
        {
            return (m_piecePositions[(int)pieceName].Stack < 0);
        }

        private bool PieceInPlay(PieceName pieceName)
        {
            return (m_piecePositions[(int)pieceName].Stack >= 0);
        }

        private bool PieceIsOnTop(PieceName pieceName)
        {
            return PieceInPlay(pieceName) && !HasPieceAt(m_piecePositions[(int)pieceName].GetAbove());
        }

        private bool CanMoveWithoutBreakingHive(PieceName pieceName)
        {
            int pieceIndex = (int)pieceName;
            if (m_piecePositions[pieceIndex].Stack == 0)
            {
                // Temporarily remove piece from board
                m_piecePositions[pieceIndex].Stack = -1;

                // Determine if the hive is broken
                bool isOneHive = IsOneHive();

                // Return piece to the board
                m_piecePositions[pieceIndex].Stack = 0;

                return isOneHive;
            }
            return true;
        }

        private bool IsOneHive()
        {
            var partOfHive = new bool[(int)PieceName.NumPieceNames];
            int piecesVisited = 0;

            // Find a piece on the board to start checking
            var startingPiece = PieceName.INVALID;
            for (int pn = 0; pn < (int)PieceName.NumPieceNames; pn++)
            {
                if (PieceInHand((PieceName)pn))
                {
                    partOfHive[pn] = true;
                    piecesVisited++;
                }
                else
                {
                    partOfHive[pn] = false;
                    if (startingPiece == PieceName.INVALID && m_piecePositions[pn].Stack == 0)
                    {
                        // Save off a starting piece on the bottom
                        startingPiece = (PieceName)pn;
                        partOfHive[pn] = true;
                        piecesVisited++;
                    }
                }
            }

            // There is at least one piece on the board
            if (startingPiece != PieceName.INVALID && piecesVisited < (int)PieceName.NumPieceNames)
            {
                var piecesToLookAt = new Queue<PieceName>();
                piecesToLookAt.Enqueue(startingPiece);

                while (piecesToLookAt.Count > 0)
                {
                    var currentPiece = piecesToLookAt.Dequeue();

                    // Check all pieces at this stack level
                    for (int dir = 0; dir < (int)Direction.NumDirections; dir++)
                    {
                        var neighbor = m_piecePositions[(int)currentPiece].GetNeighborAt((Direction)dir);
                        var neighborPiece = GetPieceAt(neighbor);
                        if (neighborPiece != PieceName.INVALID && !partOfHive[(int)neighborPiece])
                        {
                            piecesToLookAt.Enqueue(neighborPiece);
                            partOfHive[(int)neighborPiece] = true;
                            piecesVisited++;
                        }
                    }

                    // Check for all pieces above this one
                    var pieceAbove = GetPieceAt(m_piecePositions[(int)currentPiece].GetAbove());
                    while (PieceName.INVALID != pieceAbove)
                    {
                        partOfHive[(int)pieceAbove] = true;
                        piecesVisited++;
                        pieceAbove = GetPieceAt(m_piecePositions[(int)pieceAbove].GetAbove());
                    }
                }
            }

            return piecesVisited == (int)PieceName.NumPieceNames;
        }

        private int CountNeighbors(PieceName pieceName)
        {
            int count = 0;
            if (PieceInPlay(pieceName))
            {
                for (int dir = 0; dir < (int)Direction.NumDirections; dir++)
                {
                    var neighbor = GetPieceAt(m_piecePositions[(int)pieceName].GetNeighborAt((Direction)dir));
                    if (neighbor != PieceName.INVALID)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void ResetState()
        {
            bool whiteQueenSurrounded = (CountNeighbors(PieceName.wQ) == 6);
            bool blackQueenSurrounded = (CountNeighbors(PieceName.bQ) == 6);

            if (whiteQueenSurrounded && blackQueenSurrounded)
            {
                BoardState = BoardState.Draw;
            }
            else if (whiteQueenSurrounded)
            {
                BoardState = BoardState.BlackWins;
            }
            else if (blackQueenSurrounded)
            {
                BoardState = BoardState.WhiteWins;
            }
            else
            {
                BoardState = CurrentTurn == 0 ? BoardState.NotStarted : BoardState.InProgress;
            }
        }

        private void ResetCaches()
        {
            m_cachedValidPlacements = null;
            m_cachedValidMoves = null;
        }
    }
}