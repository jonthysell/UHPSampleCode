#ifndef BOARD_H
#define BOARD_H

#include <memory>
#include <string>
#include <vector>

#include "Constants.h"
#include "Enums.h"
#include "Move.h"
#include "MoveSet.h"
#include "Position.h"
#include "PositionSet.h"

namespace SampleEngine
{
class Board
{
  public:
    Board();

    BoardState GetBoardState();
    int GetCurrentTurn();

    std::string GetGameString();
    std::shared_ptr<MoveSet> GetValidMoves();

    bool TryPlayMove(Move const &move, std::string moveString);
    bool TryUndoLastMove();

    bool TryGetMoveString(Move const &move, std::string &result);
    bool TryParseMove(std::string moveString, Move &result, std::string &resultString);

  private:
    void GetValidMoves(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet);
    std::shared_ptr<PositionSet> GetValidPlacements();

    void GetValidQueenBeeMoves(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet);
    void GetValidSpiderMoves(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet);
    void GetValidBeetleMoves(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet);
    void GetValidGrasshopperMoves(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet);
    void GetValidSoldierAntMoves(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet);

    void GetValidSlides(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet, int fixedRange);
    void GetValidSlides(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet, Position const &startingPosition,
                        Position const &lastPosition, Position const &currentPosition);
    void GetValidSlides(PieceName const &pieceName, std::shared_ptr<MoveSet> moveSet, Position const &startingPosition,
                        Position const &lastPosition, Position const &currentPosition, int remainingSlides);

    void TrustedPlay(Move const &move);

    bool PlacingPieceInOrder(PieceName const &pieceName);

    PieceName GetPieceAt(Position const &position);
    PieceName GetPieceOnTopAt(Position const &position);
    bool HasPieceAt(Position const &position);

    bool PieceInHand(PieceName const &pieceName);
    bool PieceInPlay(PieceName const &pieceName);

    bool PieceIsOnTop(PieceName const &pieceName);

    bool CanMoveWithoutBreakingHive(PieceName const &pieceName);

    bool IsOneHive();

    int CountNeighbors(PieceName const &pieceName);

    void ResetState();
    void ResetCaches();

    BoardState m_boardState = BoardState::NotStarted;
    Color m_currentColor = Color::White;
    int m_currentTurn = 0;

    Position m_piecePositions[(int)PieceName::NumPieceNames];

    std::vector<Move> m_moveHistory;
    std::vector<std::string> m_moveHistoryStr;

    std::shared_ptr<MoveSet> m_cachedValidMoves = nullptr;
    std::shared_ptr<PositionSet> m_cachedValidPlacements = nullptr;
};
} // namespace SampleEngine

#endif
