using System;
using Mentula.Network.Xna;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.Commands
{
    public class Kick : Command
    {
        private Action<string> callback;

        public Kick(Action<string> OnKick)
            : base("Kick")
        {
            callback = OnKick;
        }

        public override void Call(string[] args)
        {
            if (callback != null)
            {
                if (args.Length > 0) callback(args[0]);
                else NIMT.ErrorMessage.WriteLine("The kick command requires a player name!", null);
            }
        }
    }
}