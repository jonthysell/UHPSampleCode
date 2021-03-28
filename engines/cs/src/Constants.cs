using System;

namespace SampleEngine
{
    static class Constants
    {
        public const string IdString = "id SampleEngine 1.0";

        public const string OkString = "ok";

        public const string CommandString_Info = "info";
        public const string CommandString_NewGame = "newgame";
        public const string CommandString_ValidMoves = "validmoves";
        public const string CommandString_BestMove = "bestmove";
        public const string CommandString_Play = "play";
        public const string CommandString_Pass = "pass";
        public const string CommandString_Undo = "undo";
        public const string CommandString_Options = "options";

        public const string CommandString_Exit = "exit";

        public const string ErrString = "err";
        public const string ErrorMessage_InvalidCommand = "Invalid command.";
        public const string ErrorMessage_NoGameInProgress = "No game in progress. Try 'newgame' to start a new game.";
        public const string ErrorMessage_GameIsOver = "The game is over. Try 'newgame' to start a new game.";
        public const string ErrorMessage_UnableToUndo = "Unable to undo that many moves.";
        public const string ErrorMessage_Unknown = "An unknown error has occured";

        public const string InvalidMoveString = "invalidmove";
        public const string InvalidMoveMessage_Generic = "Unable to play that move at this time.";

        public const string PassMoveString = "pass";
    }
}