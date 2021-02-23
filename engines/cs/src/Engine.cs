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
            var space = line.IndexOf(' ');
            var args = space >= 0 ? line.Substring(space + 1) : "";

            if (line == Constants.CommandString_Info)
            {
                Info();
            }
            else if (line.StartsWith(Constants.CommandString_NewGame))
            {
                NewGame(args);
            }
            else if (line == Constants.CommandString_ValidMoves)
            {
                ValidMoves();
            }
            else if (line.StartsWith(Constants.CommandString_BestMove))
            {
                BestMove();
            }
            else if (line.StartsWith(Constants.CommandString_Play))
            {
                Play(args);
            }
            else if (line == Constants.CommandString_Pass)
            {
                Pass();
            }
            else if (line.StartsWith(Constants.CommandString_Undo))
            {
                Undo(args);
            }
            else if (line == Constants.CommandString_Options)
            {
                Options();
            }
            else if (line == Constants.CommandString_Exit)
            {
                Exit();
            }
            else
            {
                WriteError(Constants.ErrorMessage_InvalidCommand);
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