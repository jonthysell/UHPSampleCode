using System;

namespace SampleEngine
{
    struct Position
    {
        public int Q;
        public int R;
        public int Stack;

        public static Position OriginPosition = new Position()
        {
            Q = 0,
            R = 0,
            Stack = 0,
        };

        private static readonly int[][] NeighborDeltas = new int[][]
        {
            new int[] { 0, 1, -1 },
            new int[] { 1, 0, -1 },
            new int[] { 1, -1, 0 },
            new int[] { 0, -1, 1 },
            new int[] { -1, 0, 1 },
            new int[] { -1, 1, 0 },
        };

        public Position GetNeighborAt(Direction direction) => new Position()
        {
            Q = Q + NeighborDeltas[(int)direction][0],
            R = R + NeighborDeltas[(int)direction][1],
            Stack = Stack,
        };

        public Position GetAbove() => new Position()
        {
            Q = Q,
            R = R,
            Stack = Stack + 1,
        };

        public Position GetBelow() => new Position()
        {
            Q = Q,
            R = R,
            Stack = Stack - 1,
        };

        public Position GetBottom() => new Position()
        {
            Q = Q,
            R = R,
            Stack = 0,
        };

        public override bool Equals(object? obj)
        {
            return obj is Position pos && this == pos;
        }

        public override int GetHashCode()
        {
            int value = 17;
            value = value * 31 + Q;
            value = value * 31 + R;
            value = value * 31 + Stack;
            return value;
        }

        public static bool operator ==(Position lhs, Position rhs)
        {
            return lhs.Q == rhs.Q && lhs.R == rhs.R && lhs.Stack == rhs.Stack;
        }

        public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);
    }
}