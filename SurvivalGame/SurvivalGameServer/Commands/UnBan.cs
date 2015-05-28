using Mentula.SurvivalGameServer;
using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class UnBan : Command
    {
        private Action<string> callback;

        public UnBan(Action<string> onUnBan)
            : base("UnBan")
        {
            callback = onUnBan;
        }

        public override void Call(string[] args)
        {
            if (callback != null)
            {
                if (args.Length > 0) callback(args[0]);
                else NIMT.ErrorMessage.WriteLine("The UnBan command requires index player name!", null);
            }
        }
    }
}