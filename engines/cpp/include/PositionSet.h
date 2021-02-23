#ifndef POSITIONSET_H
#define POSITIONSET_H

#include <unordered_set>

#include "Position.h"

namespace SampleEngine
{
struct PositionHasher
{
  public:
    size_t operator()(Position const &pos) const
    {
        return hash(pos);
    }
};

struct PositionComparator
{
  public:
    bool operator()(Position const &lhs, Position const &rhs) const
    {
        return lhs == rhs;
    }
};

typedef std::unordered_set<Position, PositionHasher, PositionComparator> PositionSet;
} // namespace SampleEngine

#endif
