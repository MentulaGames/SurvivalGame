using Mentula.SurvivalGameServer;
using Lidgren.Network;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Teleport : Command
    {
        private NetServer server;
        private Dictionary<long, Creature> players;

        public Teleport(ref NetServer server, ref Dictionary<long, Creature> players)
            : base("Teleport")
        {
            this.server = server;
            this.players = players;
        }

        public override void Call(string[] args)
        {
            if (args.Length < 3) NIMT.ErrorMessage.WriteLine("The teleport command requires a player name and a total poition (Teleport bob 12 45)");
            else
            {
                float x = 0;
                float y = 0;

                if (!(float.TryParse(args[1], out x) & float.TryParse(args[2], out y))) NIMT.ErrorMessage.WriteLine("The teleport command requires two floats as arguments!");
                else
                {
                    Vector2 pos = new Vector2(x, y);
                    bool result = false;
                    for (int i = 0; i < players.Count; i++)
                    {
                        KeyValuePair<long, Creature> k_P = players.ElementAt(i);

                        if (k_P.Value.Name == args[0])
                        {
                            players[k_P.Key].SetTilePos(pos);
                            result = true;

                            NOM nom = server.CreateMessage();
                            nom.Write((byte)DataType.PlayerRePosition_SSend);
                            nom.Write(players[k_P.Key].ToPlayer());
                            server.SendMessage(nom, server.Connections.Find(c => c.RemoteUniqueIdentifier == k_P.Key), NetDeliveryMethod.ReliableOrdered);
                            break;
                        }
                    }

                    MentulaExtensions.WriteLine(result ? NIMT.Data : NIMT.ErrorMessage, "{0} player {1}", result ? "Teleported" : "Failed to teleport", result ? string.Format("{0} to {1}", args[0], pos) : args[0]);
                }
            }
        }
    }
}