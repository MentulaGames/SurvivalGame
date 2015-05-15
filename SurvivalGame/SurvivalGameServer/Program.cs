﻿using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Commands;
using Mentula.General;
using Mentula.General.Res;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.SurvivalGameServer
{
    public class Program
    {
        private static NetServer server;
        private static CommandHandler commHand;
        private static bool Exit;
        private static Map map;
        private static Dictionary<long, Player> players;

        static void Main(string[] args)
        {
            players = new Dictionary<long, Player>();
            InitConsole();
            InitCommands();
            InitServer();
            server.Start();
            InitMap();

            while (!Exit)
            {
                commHand.Update();

                NetIncomingMessage msg;

                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case (NIMT.DiscoveryRequest):
                            server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            msg.MessageType.WriteLine("{0} discovered the service.", msg.SenderEndPoint);
                            break;
                        case (NIMT.ConnectionApproval):
                            players.Add(msg.GetId(), new Player(msg.ReadString(), IntVector2.Zero, Vector2.Zero));
                            msg.SenderConnection.Approve();
                            break;
                        case (NIMT.VerboseDebugMessage):
                        case (NIMT.DebugMessage):
                        case (NIMT.WarningMessage):
                        case (NIMT.ErrorMessage):
                            msg.MessageType.WriteLine("{0}", msg.ReadString());
                            break;
                        case (NIMT.StatusChanged):
                            NCS status = msg.ReadEnum<NCS>();
                            long id = msg.GetId();

                            switch (status)
                            {
                                case (NCS.Connected):
                                    msg.MessageType.WriteLine("{0}({1}) connected!", NetUtility.ToHexString(id), players[id].Name);
                                    break;
                                case (NCS.Disconnected):
                                    msg.MessageType.WriteLine("{0}({1}) disconnected!", NetUtility.ToHexString(id), players[id].Name);
                                    players.Remove(msg.GetId());
                                    break;
                            }

                            break;
                        case (NIMT.Data):
                            switch (msg.ReadEnum<DataType>())
                            {
                                case (DataType.InitialMap):
                                    IntVector2 chunkPos = msg.ReadVector();
                                    map.Generate(chunkPos);
                                    map.LoadChunks(chunkPos);

                                    NOM nom = server.CreateMessage();
                                    nom.Write((byte)DataType.InitialMap);
                                    Chunk[] chunks = map.GetChunks(chunkPos);

                                    nom.Write(chunks.Length);
                                    for (int i = 0; i < chunks.Length; i++)
                                    {
                                        nom.Write((C_Tile[])chunks[i]);
                                    }

                                    for (int i = 0; i < chunks.Length; i++)
                                    {
                                        nom.Write((C_Destrucible[])chunks[i]);
                                    }

                                    server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    break;
                                case (DataType.ChunkRequest):
                                    chunkPos = msg.ReadVector();
                                    IntVector2 oldPos = msg.ReadVector();

                                    nom = server.CreateMessage();
                                    nom.Write((byte)DataType.ChunkRequest);

                                    List<Chunk> chunk = map.GetChunks(oldPos, chunkPos);
                                    nom.Write(chunk.Count);

                                    for (int i = 0; i < chunk.Count; i++)
                                    {
                                        nom.Write((C_Tile[])chunk[i]);
                                    }

                                    for (int i = 0; i < chunk.Count; i++)
                                    {
                                        nom.Write((C_Destrucible[])chunk[i]);
                                    }

                                    server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableSequenced);
                                    break;
                                case (DataType.PlayerUpdate):
                                    chunkPos = msg.ReadVector();
                                    Vector2 pos = msg.ReadVector2();
                                    players[msg.GetId()].ReSet(chunkPos, pos);

                                    if (map.Generate(chunkPos)) msg.MessageType.WriteLine("Generated at: {0}.", chunkPos);
                                    map.LoadChunks(chunkPos);

                                    nom = server.CreateMessage();
                                    nom.Write((byte)DataType.PlayerUpdate);
                                    nom.Write(players.Values.ToArray());
                                    server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
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
            Console.SetBufferSize(1080, 1920);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = string.Format("{0}_Sever", Resources.AppName);
            Console.CancelKeyPress += (sender, e) => e.Cancel = e.SpecialKey == ConsoleSpecialKey.ControlC ? true : false;
        }

        private static void InitServer()
        {
            NPConf config = new NPConf(Resources.AppName)
                {
                    Port = Ips.PORT,
                    EnableUPnP = true
                };
            config.EnableMessageType(NIMT.DiscoveryRequest);
            config.EnableMessageType(NIMT.ConnectionApproval);
            server = new NetServer(config);
        }

        private static void InitMap()
        {
            map = new Map();
            map.Generate(IntVector2.Zero);
            NIMT.Data.WriteLine("Generated at: {0}.", IntVector2.Zero);
            map.LoadChunks(IntVector2.Zero);
        }

        private static void InitCommands()
        {
            commHand = new CommandHandler(
                new Exit(() => Exit = true),
                new Forward(port =>
                    {
                        bool forward = server.UPnP.ForwardPort(port, Resources.AppName);
                        MentulaExtensions.WriteLine(forward ? NIMT.DebugMessage : NIMT.WarningMessage, "{0} to forward port: {1}!", forward ? "Succeted" : "Failed", port);
                    }),
                new Kick(name =>
                    {
                        bool result = false;
                        for (int i = 0; i < players.Count; i++)
                        {
                            KeyValuePair<long, Player> k_P = players.ElementAt(i);

                            if (k_P.Value.Name == name)
                            {
                                server.Connections.Find(c => c.RemoteUniqueIdentifier == k_P.Key).Disconnect("You have been kicked!");
                                result = true;
                            }

                            MentulaExtensions.WriteLine(result ? NIMT.StatusChanged : NIMT.ErrorMessage, "{0} player: {1}", result ? "Kicked" : "Failed to kick", name);
                        }
                    }));
        }
    }
}