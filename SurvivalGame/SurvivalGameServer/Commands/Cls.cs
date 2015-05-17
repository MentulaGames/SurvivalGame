using System;

namespace Mentula.Commands
{
    public class Cls : Command
    {
        public Cls()
            : base("Cls")
        {

        }

        public override void Call(string[] args)
        {
            Console.Clear();
        }
    }
}