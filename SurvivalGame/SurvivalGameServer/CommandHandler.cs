using Mentula.Commands;
using Mentula.SurvivalGameServer;
using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula
{
    public class CommandHandler
    {
        public Command[] Commands;

        private string raw;

        public CommandHandler(params Command[] commands)
        {
            Commands = commands;
            AddRange(new Help(this), new Cls());
            raw = "";
        }

        public void Add(Command command)
        {
            int old = Commands.Length;
            Array.Resize(ref Commands, old + 1);
            Commands[old] = command;
        }

        public void AddRange(params Command[] commands)
        {
            int old = Commands.Length;
            Array.Resize(ref Commands, old + commands.Length);

            for (int i = 0; i < commands.Length; i++)
            {
                Commands[old + i] = commands[i];
            }
        }

        public void Update()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);

                if (info.Key != ConsoleKey.Enter)
                {
                    ClearCurrentLine();

                    if (info.Key == ConsoleKey.Backspace) raw = raw.Remove(raw.Length - 1, 1);
                    else if (info.KeyChar.GetHashCode() != 0) raw += info.KeyChar;

                    Console.Write(raw);
                }
                else
                {
                    Console.Write(Console.Out.NewLine);

                    bool called = false;

                    for (int i = 0; i < Commands.Length; i++)
                    {
                        Command c = Commands[i];
                        string[] args;

                        if (c.IsThis(raw, out args))
                        {
                            called = true;
                            c.Call(args);
                        }
                    }

                    if (!called) MentulaExtensions.WriteLine(NIMT.ErrorMessage, "Unknown command!", null);
                    raw = "";
                }
            }
        }

        private void ClearCurrentLine()
        {
            int curPos = Console.CursorTop;
            Console.SetCursorPosition(0, curPos);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, curPos);
        }
    }
}