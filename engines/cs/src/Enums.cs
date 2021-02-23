using System;
using System.ComponentModel;

namespace SampleEngine
{
    enum Color
    {
        White = 0,
        Black,
        NumColors,
    };

    enum BoardState
    {
        NotStarted = 0,
        InProgress,
        Draw,
        WhiteWins,
        BlackWins,
    };

    [DefaultValue(INVALID)]
    enum PieceName
    {
        INVALID = -1,
        wQ = 0,
        wS1,
        wS2,
        wB1,
        wB2,
        wG1,
        wG2,
        wG3,
        wA1,
        wA2,
        wA3,
        bQ,
        bS1,
        bS2,
        bB1,
        bB2,
        bG1,
        bG2,
        bG3,
        bA1,
        bA2,
        bA3,
        NumPieceNames
    };

    enum Direction
    {
        Up = 0,
        UpRight,
        DownRight,
        Down,
        DownLeft,
        UpLeft,
        NumDirections,
    };

    [DefaultValue(INVALID)]
    enum BugType
    {
        INVALID = -1,
        QueenBee = 0,
        Spider,
        Beetle,
        Grasshopper,
        SoldierAnt,
        NumBugTypes,
    };

    static class Enums
    {
        public static Color GetColor(PieceName value)
        {
            switch (value)
            {
                case PieceName.wQ:
                case PieceName.wS1:
                case PieceName.wS2:
                case PieceName.wB1:
                case PieceName.wB2:
                case PieceName.wG1:
                case PieceName.wG2:
                case PieceName.wG3:
                case PieceName.wA1:
                case PieceName.wA2:
                case PieceName.wA3:
                    return Color.White;
                case PieceName.bQ:
                case PieceName.bS1:
                case PieceName.bS2:
                case PieceName.bB1:
                case PieceName.bB2:
                case PieceName.bG1:
                case PieceName.bG2:
                case PieceName.bG3:
                case PieceName.bA1:
                case PieceName.bA2:
                case PieceName.bA3:
                    return Color.Black;
            }

            return Color.NumColors;
        }

        public static Direction LeftOf(Direction value)
        {
            return (Direction)(((int)value + (int)Direction.NumDirections - 1) % (int)Direction.NumDirections);
        }

        public static Direction RightOf(Direction value)
        {
            return (Direction)(((int)value + 1) % (int)Direction.NumDirections);
        }

        public static BugType GetBugType(PieceName value)
        {
            switch (value)
            {
                case PieceName.wQ:
                case PieceName.bQ:
                    return BugType.QueenBee;
                case PieceName.wS1:
                case PieceName.wS2:
                case PieceName.bS1:
                case PieceName.bS2:
                    return BugType.Spider;
                case PieceName.wB1:
                case PieceName.wB2:
                case PieceName.bB1:
                case PieceName.bB2:
                    return BugType.Beetle;
                case PieceName.wG1:
                case PieceName.wG2:
                case PieceName.wG3:
                case PieceName.bG1:
                case PieceName.bG2:
                case PieceName.bG3:
                    return BugType.Grasshopper;
                case PieceName.wA1:
                case PieceName.wA2:
                case PieceName.wA3:
                case PieceName.bA1:
                case PieceName.bA2:
                case PieceName.bA3:
                    return BugType.SoldierAnt;
                default:
                    return BugType.INVALID;
            }
        }
    }
}