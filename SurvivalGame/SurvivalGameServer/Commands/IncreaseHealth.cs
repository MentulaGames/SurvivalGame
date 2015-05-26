using System.Collections.Generic;
using System.Linq;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class IncreaseHealth : Command
    {
        private Dictionary<long, Creature> players;

        public IncreaseHealth(ref Dictionary<long, Creature> players)
            : base("Increase Health")
        {
            this.players = players;
        }

        public override void Call(string[] args)
        {
            if (args.Length < 0) NIMT.ErrorMessage.WriteLine("The Increase health command requires a player name!", null);
            else
            {
                bool result = false;
                for (int i = 0; i < players.Count; i++)
                {
                    KeyValuePair<long, Creature> k_P = players.ElementAt(i);

                    if (k_P.Value.Name == args[0])
                    {
                        k_P.Value.MaxHealth = float.MaxValue;
                        k_P.Value.Health = float.MaxValue;
                        result = true;
                        break;
                    }
                }

                MentulaExtensions.WriteLine(result ? NIMT.StatusChanged : NIMT.ErrorMessage, "{0} player health: {1}", result ? "Increased" : "Failed to increase", args[0]);
            }
        }
    }
}