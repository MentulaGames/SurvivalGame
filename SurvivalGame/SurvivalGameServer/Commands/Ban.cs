using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Ban : Command
    {
        private NetServer server;
        private Dictionary<long, Creature> players;
        private Dictionary<string, IPAddress> banned;

        public Ban(ref NetServer server, ref Dictionary<long, Creature> players, ref Dictionary<string, IPAddress> banned)
            : base("Ban")
        {
            this.server = server;
            this.players = players;
            this.banned = banned;
        }

        public override void Call(string[] args)
        {
            if (args.Length <= 0) NIMT.ErrorMessage.WriteLine("The ban command requires a player name!");
            else
            {
                bool result = false;
                for (int i = 0; i < players.Count; i++)
                {
                    KeyValuePair<long, Creature> k_P = players.ElementAt(i);

                    if (k_P.Value.Name == args[0])
                    {
                        NetConnection end = server.Connections.Find(c => c.RemoteUniqueIdentifier == k_P.Key);
                        banned.Add(args[0], end.RemoteEndPoint.Address);
                        end.Disconnect("You have been banned!");
                        result = true;
                        break;
                    }
                }

                MentulaExtensions.WriteLine(result ? NIMT.StatusChanged : NIMT.ErrorMessage, "{0} player: {1}", result ? "Banned" : "Failed to ban", args[0]);
            }
        }
    }
}