using System;

namespace Mentula.SurvivalGameServer.Commands
{
    public abstract class Command
    {
        public string m_Command;

        private int c_words;

        public Command(string command)
        {
            m_Command = command.ToUpper();
            c_words = command.Split(' ').Length;
        }

        public bool IsThis(string raw, out string[] args)
        {
            string[] com = m_Command.Split(' ');
            string[] split = raw.Split(' ');
            args = new string[0];

            if (split.Length >= c_words)
            {
                for (int i = 0; i < c_words; i++)
                {
                    if (com[i] != split[i].ToUpper()) return false;
                }

                Array.Resize(ref args, split.Length - c_words);
                Array.Copy(split, c_words, args, 0, split.Length - c_words);
                return true;
            }

            return false;
        }

        public abstract void Call(string[] args);
    }
}