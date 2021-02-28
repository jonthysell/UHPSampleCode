using System;
using System.Linq;
using System.Text;

namespace SampleEngine
{
    class Engine
    {
        public bool ExitRequested { get; private set; }

        private readonly Action<string> m_writeLine;

        private Board? m_board = null;

        public Engine(Action<string> writeLine)
        {
            m_writeLine = writeLine;
        }

        public void Start()
        {
            Info();
        }

        public void ReadLine(string line)
        {
            var split = (line?.Trim() ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var command = split.Length > 0 ? split[0] : "";
            var args = split.Length > 1 ? string.Join(" ", split, 1, split.Length - 1) : "";

            switch (command)
            {
                case Constants.CommandString_Info:
                    Info();
                    break;
                case Constants.CommandString_NewGame:
                    NewGame(args);
                    break;
                case Constants.CommandString_ValidMoves:
                    ValidMoves();
                    break;
                case Constants.CommandString_BestMove:
                    BestMove();
                    break;
                case Constants.CommandString_Play:
                    Play(args);
                    break;
                case Constants.CommandString_Pass:
                    Pass();
                    break;
                case Constants.CommandString_Undo:
                    Undo(args);
                    break;
                case Constants.CommandString_Options:
                    Options();
                    break;
                case Constants.CommandString_Exit:
                    Exit();
                    break;
                default:
                    WriteError(Constants.ErrorMessage_InvalidCommand);
                    break;
            }
        }

        private void WriteLine(string line)
        {
            m_writeLine(line);
        }

        private void WriteError(string message)
        {
            WriteLine($"{Constants.ErrString} {message}");
            WriteLine(Constants.OkString);
        }

        private void WriteError()
        {
            WriteError(Constants.ErrorMessage_Unknown);
        }

        private void Info()
        {
            WriteLine(Constants.IdString);
            WriteLine(Constants.OkString);
        }

        private void NewGame(string args)
        {
            m_board = new Board();

            string[] splitArgs = args.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitArgs.Length; i++)
            {
                if (i > 2)
                {
                    if (!m_board.TryParseMove(splitArgs[i], out Move? move, out string? moveString) || !m_board.TryPlayMove(move ?? Move.PassMove, moveString ?? ""))
                    {
                        WriteError(Constants.ErrorMessage_Unknown);
                        return;
                    }
                }
            }

            WriteLine(m_board.GetGameString());
            WriteLine(Constants.OkString);
        }

        private void ValidMoves()
        {
            if (m_board is null)
            {
                WriteError(Constants.ErrorMessage_NoGameInProgress);
                return;
            }

            if (m_board.GameIsOver)
            {
                WriteError(Constants.ErrorMessage_GameIsOver);
                return;
            }

            var validMoves = m_board.GetValidMoves();

            var sb = new StringBuilder();
            bool first = true;
            foreach (var validMove in validMoves)
            {
                if (m_board.TryGetMoveString(validMove, out string? result))
                {
                    sb.Append(first ? result : $";{result}");
                }
                first = false;
            }

            WriteLine(sb.ToString());
            WriteLine(Constants.OkString);
        }

        private void BestMove()
        {
            if (m_board is null)
            {
                WriteError(Constants.ErrorMessage_NoGameInProgress);
                return;
            }

            if (m_board.GameIsOver)
            {
                WriteError(Constants.ErrorMessage_GameIsOver);
                return;
            }

            var validMoves = m_board.GetValidMoves();

            if (validMoves.Count > 0 && m_board.TryGetMoveString(validMoves.First(), out string? result))
            {
                WriteLine(result ?? "");
                WriteLine(Constants.OkString);
            }
            else
            {
                WriteError();
            }
        }

        private void Play(string args)
        {
            if (m_board is null)
            {
                WriteError(Constants.ErrorMessage_NoGameInProgress);
                return;
            }

            if (m_board.GameIsOver)
            {
                WriteError(Constants.ErrorMessage_GameIsOver);
                return;
            }

            if (m_board.TryParseMove(args, out Move? move, out string? moveString) && m_board.TryPlayMove(move ?? Move.PassMove, moveString ?? ""))
            {
                WriteLine(m_board.GetGameString());
            }
            else
            {
                WriteLine($"{Constants.InvalidMoveString} {Constants.InvalidMoveMessage_Generic}");
            }

            WriteLine(Constants.OkString);
        }

        private void Pass()
        {
            Play(Constants.PassMoveString);
        }

        private void Undo(string args)
        {
            if (m_board is null)
            {
                WriteError(Constants.ErrorMessage_NoGameInProgress);
                return;
            }

            if (!int.TryParse(args, out int movesToUndo))
            {
                movesToUndo = 1;
            }

            if (movesToUndo < 1 || movesToUndo > m_board.CurrentTurn)
            {
                WriteError(Constants.ErrorMessage_UnableToUndo);
                return;
            }

            for (int i = 0; i < movesToUndo; i++)
            {
                m_board.TryUndoLastMove();
            }

            WriteLine(m_board.GetGameString());
            WriteLine(Constants.OkString);
        }

        private void Options()
        {
            WriteLine(Constants.OkString);
        }

        private void Exit()
        {
            ExitRequested = true;
        }
    }
}