using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Content;
using Mentula.General;
using Mentula.General.Resources;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Threading;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;


namespace Mentula.SurvivalGameServer
{
    public class Main
    {
        public event PlayerListChanged TryConnectPlayer;
        public event PlayerListChanged TryRemovePlayer;
        public event InfoMessage SimpleMessage;
        public event NIMTMessage CustomMessage;

        internal NetServer server;
        internal Map map;
        internal Content content;

        private ContentManager Content;
        private Dictionary<long, Creature> players;
        private Dictionary<string, IPAddress> banned;
        private Dictionary<long, string> addQueue;

        public Main()
        {
            players = new Dictionary<long, Creature>();
            banned = new Dictionary<string, IPAddress>();
            addQueue = new Dictionary<long, string>();

            Content = new ContentManager(new ServiceContainer(), "Content");
        }

        public void Initialize()
        {
            InitServer();
            InitMap();
            content = new Content(ref Content, "Metals");
        }

        public void Start()
        {
            server.Start();
        }

        public void Stop()
        {
            players = new Dictionary<long, Creature>();
            addQueue = new Dictionary<long, string>();
            server.Shutdown(string.Format("{0} Exiting", Res.AppName));
        }

        public void Update()
        {
            NetIncomingMessage msg;

            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case (NIMT.DiscoveryRequest):
                        server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                        if (CustomMessage != null) CustomMessage(NIMT.DiscoveryRequest, "{0} discovered the service.", msg.SenderEndPoint.Address);
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

                        addQueue.Add(id, name);
                        msg.SenderConnection.Approve();
                        break;
                    case (NIMT.VerboseDebugMessage):
                    case (NIMT.DebugMessage):
                    case (NIMT.WarningMessage):
                    case (NIMT.ErrorMessage):
                        if (CustomMessage != null) CustomMessage(msg.MessageType, msg.ReadString());
                        break;
                    case (NIMT.StatusChanged):
                        id = msg.GetId();
                        NCS status = msg.ReadEnum<NCS>();

                        switch (status)
                        {
                            case (NCS.Connected):
                                if (!addQueue.ContainsKey(id)) break;

                                bool result = true;
                                if (TryConnectPlayer != null) result = TryConnectPlayer(msg.SenderEndPoint.Address, addQueue[id]);

                                if (result)
                                {
                                    players.Add(id, new Creature(new Creature(addQueue[id], new Stats(10), 100, Color.Purple, -1), IntVector2.Zero, Vector2.Zero));
                                    addQueue.Remove(id);
                                    if (CustomMessage != null) CustomMessage(NIMT.StatusChanged, "{0}({1}) connected!", NetUtility.ToHexString(id), players[id].Name);
                                }
                                else
                                {
                                    if (CustomMessage != null) CustomMessage(NIMT.WarningMessage, "{0}({1}) is attacking the service!", NetUtility.ToHexString(id), msg.SenderEndPoint.Address);
                                }
                                break;
                            case (NCS.Disconnected):
                                if (CustomMessage != null) CustomMessage(NIMT.StatusChanged, "{0}({1}) disconnected!", NetUtility.ToHexString(id), players[id].Name);

                                result = true;
                                if (TryRemovePlayer != null) result = TryRemovePlayer(msg.SenderEndPoint.Address, players[id].Name);

                                if (result)
                                {
                                    if (CustomMessage != null) CustomMessage(NIMT.StatusChanged, "{0} left the server", players[id].Name);
                                    players.Remove(id);
                                }
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

                                if (map.Generate(chunkPos) && SimpleMessage != null) SimpleMessage("Generated at: {0}.", chunkPos);
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
                                        long playerId = players.First(p => p.Value.Name == c.Name).Key;
                                        server.Connections.First(conn => conn.RemoteUniqueIdentifier == playerId).Disconnect("You have been kicked!");
                                        break;
                                    }

                                    for (int i = 0; i < server.Connections.Count; i++)
                                    {
                                        NetConnection conn = server.Connections[i];

                                        nom = server.CreateMessage();
                                        nom.Write((byte)DataType.CreatureChange_SSend);
                                        nom.Write(c.ChunkPos);
                                        nom.Write(c.GetTilePos());
                                        
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

        private void InitServer()
        {
            NPConf config = new NPConf(Res.AppName) { Port = Ips.PORT, EnableUPnP = true };
            config.EnableMessageType(NIMT.DiscoveryRequest);
            config.EnableMessageType(NIMT.ConnectionApproval);
            server = new NetServer(config);
        }

        private void InitMap()
        {
            map = new Map();
            if (map.Generate(IntVector2.Zero) && SimpleMessage != null) SimpleMessage("Generated at: {0}.", IntVector2.Zero);
            map.LoadChunks(IntVector2.Zero);
        }
    }
}