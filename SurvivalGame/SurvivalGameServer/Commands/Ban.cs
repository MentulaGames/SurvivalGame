using Mentula.SurvivalGameServer;
using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Ban : Command
    {
        private Action<string> callback;

        public Ban(Action<string> onBan)
            : base("Ban")
        {
            callback = onBan;
        }

        public override void Call(string[] args)
        {
            if (callback != null)
            {
                if (args.Length > 0) callback(args[0]);
                else NIMT.ErrorMessage.WriteLine("The Ban command requires index player name!", null);
            }
        }
    }
}