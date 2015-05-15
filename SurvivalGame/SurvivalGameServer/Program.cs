using Lidgren.Network;
using Lidgren.Network.Xna;
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
        private static Map map;
        private static Dictionary<long, Player> players;

        static void Main(string[] args)
        {
            players = new Dictionary<long, Player>();
            InitConsole();
            InitServer();
            server.Start();
            bool forward = server.UPnP.ForwardPort(Ips.PORT, Resources.AppName);
            MentulaExtensions.WriteLine(forward ? NIMT.DebugMessage : NIMT.WarningMessage, "{0} to forward port: {1}!", forward ? "Succeted" : "Failed", Ips.PORT);
            InitMap();

            while (!Console.KeyAvailable || Console.ReadLine() != "Exit")
            {
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
                            players.Add(msg.SenderConnection.RemoteUniqueIdentifier, new Player(msg.ReadString(), IntVector2.Zero, Vector2.Zero));
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

                            switch (status)
                            {
                                case (NCS.Connected):
                                    msg.MessageType.WriteLine("{0}({1}) connected!", NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier), players[msg.SenderConnection.RemoteUniqueIdentifier].Name);
                                    break;
                                case (NCS.Disconnected):
                                    msg.MessageType.WriteLine("{0} disconnected!", NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier));
                                    players.Remove(msg.SenderConnection.RemoteUniqueIdentifier);
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
                                    players[msg.SenderConnection.RemoteUniqueIdentifier].ReSet(chunkPos, pos);

                                    if (map.Generate(chunkPos)) msg.MessageType.WriteLine("Generated for: {0}.", chunkPos);
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
    }
}