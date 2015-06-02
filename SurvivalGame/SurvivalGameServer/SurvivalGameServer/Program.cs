using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Content;
using Mentula.General;
using Mentula.General.Res;
using Mentula.Network.Xna;
using Mentula.SurvivalGameServer.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.SurvivalGameServer
{
    public class Program
    {
        private static ContentManager Content;
        private static NetServer server;
        private static CommandHandler commHand;
        private static bool Exit;
        private static Map map;
        private static Dictionary<long, Creature> players;
        private static Dictionary<string, IPAddress> banned;

        static Program()
        {
            players = new Dictionary<long, Creature>();
            banned = new Dictionary<string, IPAddress>();

            Content = new ContentManager(new ServiceContainer(), "Content");

            InitConsole();
            InitServer();
            InitCommands();
            InitMap();
        }

        static void Main(string[] args)
        {
            server.Start();

            while (!Exit)
            {
                //commHand.Update();

                NetIncomingMessage msg;

                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case (NIMT.DiscoveryRequest):
                            server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            msg.MessageType.WriteLine("{0} discovered the service.", msg.SenderEndPoint.Address);
                            break;
                        case (NIMT.ConnectionApproval):
                            long id = msg.GetId();
                            string name = msg.ReadString();

                            if (string.IsNullOrEmpty(name) | name.Length > 16)
                            {
                                msg.SenderConnection.Deny("Your name must between 1 and 16 characters!");
                                break;
                            }
                            else if (banned.Values.Contains(msg.SenderConnection.RemoteEndPoint.Address))
                            {
                                msg.SenderConnection.Deny("You have been banned from this server!");
                                break;
                            }
                            else if (players.ContainsKey(id))
                            {
                                msg.SenderConnection.Deny("You are still connected to the service!\nPlease wait some time before trying again.");
                                break;
                            }
                            else if (players.FirstOrDefault(p => p.Value.Name == name).Value != null)
                            {
                                msg.SenderConnection.Deny(string.Format("The name: {0} is already in use!", name));
                                break;
                            }

                            players.Add(id, new Creature(new Creature(name, new Stats(10), 100, Color.Purple, -1), IntVector2.Zero, Vector2.Zero));
                            msg.SenderConnection.Approve();
                            break;
                        case (NIMT.VerboseDebugMessage):
                        case (NIMT.DebugMessage):
                        case (NIMT.WarningMessage):
                        case (NIMT.ErrorMessage):
                            msg.MessageType.WriteLine("{0}", msg.ReadString());
                            break;
                        case (NIMT.StatusChanged):
                            id = msg.GetId();
                            NCS status = msg.ReadEnum<NCS>();

                            switch (status)
                            {
                                case (NCS.Connected):
                                    msg.MessageType.WriteLine("{0}({1}) connected!", NetUtility.ToHexString(id), players[id].Name);
                                    break;
                                case (NCS.Disconnected):
                                    msg.MessageType.WriteLine("{0}({1}) disconnected!", NetUtility.ToHexString(id), players[id].Name);
                                    players.Remove(id);
                                    break;
                            }

                            break;
                        case (NIMT.Data):
                            switch (msg.ReadEnum<DataType>())
                            {
                                case (DataType.InitialMap_Both):
                                    id = msg.GetId();
                                    IntVector2 chunkPos = players[id].ChunkPos;
                                    map.Generate(chunkPos);
                                    map.LoadChunks(chunkPos);

                                    NOM nom = server.CreateMessage();
                                    nom.Write((byte)DataType.InitialMap_Both);
                                    Chunk[] chunks = map.GetChunks(chunkPos);

                                    nom.Write((Int16)chunks.Length);
                                    for (int i = 0; i < chunks.Length; i++)
                                    {
                                        nom.Write((C_Tile[])chunks[i]);
                                        nom.Write((C_Destrucible[])chunks[i]);
                                        nom.Write((C_Creature[])chunks[i]);
                                    }

                                    server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    break;
                                case (DataType.ChunkRequest_Both):
                                    id = msg.GetId();
                                    chunkPos = msg.ReadVector();
                                    IntVector2 oldPos = msg.ReadVector();

                                    if (players[id].ChunkPos == chunkPos | players[id].ChunkPos == oldPos)
                                    {
                                        nom = server.CreateMessage();
                                        nom.Write((byte)DataType.ChunkRequest_Both);

                                        List<Chunk> chunk = map.GetChunks(oldPos, chunkPos);
                                        nom.Write((Int16)chunk.Count);

                                        for (int i = 0; i < chunk.Count; i++)
                                        {
                                            nom.Write((C_Tile[])chunk[i]);
                                            nom.Write((C_Destrucible[])chunk[i]);
                                            nom.Write((C_Creature[])chunk[i]);
                                        }

                                        server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered);
                                    }
                                    break;
                                case (DataType.PlayerUpdate_Both):
                                    id = msg.GetId();
                                    chunkPos = msg.ReadVector();
                                    Vector2 pos = msg.ReadVector2();

                                    players[id].ReSet(chunkPos, pos);

                                    if (map.Generate(chunkPos)) msg.MessageType.WriteLine("Generated at: {0}.", chunkPos);
                                    map.LoadChunks(chunkPos);

                                    nom = server.CreateMessage();
                                    nom.Write((byte)DataType.PlayerUpdate_Both);
                                    nom.Write(players.Where(p => p.Key != id).Select(p => p.Value.ToPlayer()).ToArray());
                                    server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    break;
                                case (DataType.Attack_CSend):
                                    id = msg.GetId();
                                    float rot = msg.ReadFloat();
                                    int chunkIndex = map.LoadedChunks.FindIndex(ch => ch.Pos == players[id].ChunkPos);

                                    List<Creature> crs = ((Creature[])map.LoadedChunks[chunkIndex].Creatures.ToArray().Clone()).ToList();
                                    crs.AddRange(players.Values);

                                    List<Creature> t = Combat.AttackCreatures(players[id], crs.ToArray(), rot, 120, 2);
                                    if (t.Count != crs.Count)
                                    {
                                        Creature c = crs.FirstOrDefault(ch => !t.Contains(ch));

                                        if (players.ContainsValue(c))
                                        {
                                            commHand.Commands.First(cr => cr.m_Command == "KICK").Call(new string[1] { c.Name });
                                            break;
                                        }

                                        for (int i = 0; i < server.Connections.Count; i++)
                                        {
                                            NetConnection conn = server.Connections[i];

                                            nom = server.CreateMessage();
                                            nom.Write((byte)DataType.CreatureChange_SSend);
                                            nom.Write(c.ChunkPos);
                                            nom.Write(c.GetTilePos());
                                            nom.Write(c.Health);
                                            server.SendMessage(nom, conn, NetDeliveryMethod.ReliableUnordered);
                                        }
                                    }
                                    map.LoadedChunks[chunkIndex].Creatures = t.Where(c => !players.Values.Contains(c)).ToList();
                                    break;
                            }
                            break;
                    }

                    Thread.Sleep(1);
                }
            }

            server.Shutdown(string.Format("{0} Exiting", Resources.AppName));
        }

        private static void InitConsole()
        {
            //Console.SetBufferSize(1080, 1920);
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.Title = string.Format("{0}_Sever", Resources.AppName);
            //Console.CancelKeyPress += (sender, e) => e.Cancel = e.SpecialKey == ConsoleSpecialKey.ControlC ? true : false;
        }

        private static void InitServer()
        {
            NPConf config = new NPConf(Resources.AppName) { Port = Ips.PORT, EnableUPnP = true };
            config.EnableMessageType(NIMT.DiscoveryRequest);
            config.EnableMessageType(NIMT.ConnectionApproval);
            server = new NetServer(config);
        }

        private static void InitMap()
        {
            map = new Map();
            if (map.Generate(IntVector2.Zero)) NIMT.Data.WriteLine("Generated at: {0}.", IntVector2.Zero);
            map.LoadChunks(IntVector2.Zero);
        }

        private static void InitCommands()
        {
            commHand = new CommandHandler(
                new Exit(() => Exit = true),
                new Status(ref server),
                new Forward(ref server),
                new Kick(ref server, ref players),
                new Ban(ref server, ref players, ref banned),
                new UnBan(ref banned),
                new IncreaseHealth(ref players),
                new Teleport(ref server, ref players));
        }
    }
}