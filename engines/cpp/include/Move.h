#ifndef MOVE_H
#define MOVE_H

#include <string>

#include "Enums.h"
#include "Position.h"

namespace SampleEngine
{
struct Move
{
    SampleEngine::PieceName PieceName;
    Position Source;
    Position Destination;
};

static const Move PassMove{PieceName::INVALID, NullPosition, NullPosition};

bool operator==(Move const &lhs, Move const &rhs);
bool operator!=(Move const &lhs, Move const &rhs);

size_t hash(Move const &move);

std::string BuildMoveString(bool &isPass, PieceName &startPiece, char &beforeSeperator, PieceName &endPiece,
                            char &afterSeperator);

bool TryNormalizeMoveString(std::string const &moveString, std::string &result);
bool TryNormalizeMoveString(std::string const &moveString, bool &isPass, PieceName &startPiece, char &beforeSeperator,
                            PieceName &endPiece, char &afterSeperator);
} // namespace SampleEngine

#endif
