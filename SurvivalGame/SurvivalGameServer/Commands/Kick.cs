using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Kick : Command
    {
        private NetServer server;
        private Dictionary<long, Creature> players;

        public Kick(ref NetServer server, ref Dictionary<long, Creature> players)
            : base("Kick")
        {
            this.server = server;
            this.players = players;
        }

        public override void Call(string[] args)
        {
            if (args.Length <= 0) NIMT.ErrorMessage.WriteLine("The kick command requires a player name!", null);
            else
            {
                bool result = false;
                for (int i = 0; i < players.Count; i++)
                {
                    KeyValuePair<long, Creature> k_P = players.ElementAt(i);

                    if (k_P.Value.Name == args[0])
                    {
                        server.Connections.Find(c => c.RemoteUniqueIdentifier == k_P.Key).Disconnect("You have been kicked!");
                        result = true;
                        break;
                    }
                }

                MentulaExtensions.WriteLine(result ? NIMT.StatusChanged : NIMT.ErrorMessage, "{0} player: {1}", result ? "Kicked" : "Failed to kick", args[0]);
            }
        }
    }
}