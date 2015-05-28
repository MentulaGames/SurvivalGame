using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class IncreaseHealth : Command
    {
        private Action<string> callback;

        public IncreaseHealth(Action<string> onIncrease)
            : base("Increase Health")
        {
            callback = onIncrease;
        }

        public override void Call(string[] args)
        {
            if (callback != null)
            {
                if (args.Length > 0) callback(args[0]);
                else NIMT.ErrorMessage.WriteLine("The Increase health command requires index player name!", null);
            }
        }
    }
}