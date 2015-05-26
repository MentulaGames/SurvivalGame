using System.Collections.Generic;
using System.Linq;
using System.Net;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class UnBan : Command
    {
        private Dictionary<string, IPAddress> banned;

        public UnBan(ref Dictionary<string, IPAddress> banned)
            : base("UnBan")
        {
            this.banned = banned;
        }

        public override void Call(string[] args)
        {
            if (args.Length < 0) NIMT.ErrorMessage.WriteLine("The UnBan command requires a player name!", null);
            else
            {
                bool result = false;
                for (int i = 0; i < banned.Count; i++)
                {
                    KeyValuePair<string, IPAddress> k_P = banned.ElementAt(i);

                    if (k_P.Key == args[0])
                    {
                        banned.Remove(args[0]);
                        result = true;
                        break;
                    }
                }

                MentulaExtensions.WriteLine(result ? NIMT.StatusChanged : NIMT.ErrorMessage, "{0} player: {1}", result ? "UnBanned" : "Failed to unBan", args[0]);
            }
        }
    }
}