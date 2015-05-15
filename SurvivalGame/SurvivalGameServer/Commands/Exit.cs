using System;

namespace Mentula.Commands
{
    public class Exit : Command
    {
        private Action callback;

        public Exit(Action onExit)
            : base("Exit")
        {
            callback = onExit;
        }

        public override void Call(string[] args)
        {
            if (callback != null) callback.Invoke();
        }
    }
}